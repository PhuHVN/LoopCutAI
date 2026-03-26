using AutoMapper;
using FluentValidation.AspNetCore;
using LoopCut.API;
using LoopCut.API.Middleware;
using LoopCut.Application.DTOs;
using LoopCut.Application.Interfaces;
using LoopCut.Application.Options;
using LoopCut.Application.Services;
using LoopCut.Domain.Enums.EnumConfig;
using LoopCut.Infrastructure.BackgroundTasks;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Seeder;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

//convert enum to string in json
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new ExclusiveEnumConverterFactory(excludeFromString: new[] { typeof(StatusCodeHelper) }));
}).AddFluentValidation(options =>
{
    options.ImplicitlyValidateChildProperties = true;
}); //for fluent validation

// Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Background Services
builder.Services.AddHostedService<SubscriptionEmailWorker>();
builder.Services.AddHostedService<SubscriptionEmailChecker>();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "LoopCutAI API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    option.SchemaGeneratorOptions.SchemaIdSelector = type => type.FullName;
}
);
//Jwt Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleSection["ClientId"];
    options.ClientSecret = googleSection["ClientSecret"];
    options.CallbackPath = "/auth/google/callback";
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = "Unauthorized or missing token"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnAuthenticationFailed = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = context.Exception?.GetType().Name switch
            {
                "SecurityTokenExpiredException" => "Token expired",
                "SecurityTokenInvalidSignatureException" => "Invalid token signature",
                _ => "Invalid token"
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnForbidden = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = "You do not have permission to access this resource"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});
//Rate Limiting
builder.Services.AddRateLimiting();

//config AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
//Redis Cache
var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var host = redisConfig["Host"];
    var port = redisConfig["Port"] ?? "6379";
    var password = redisConfig["Token"];

    var options = new ConfigurationOptions
    {
        EndPoints = { { host!, int.Parse(port) } },
        Password = password,
        Ssl = true,
        AbortOnConnectFail = false,
        ConnectTimeout = 10000,
        SyncTimeout = 10000,
        KeepAlive = 30
    };

    return ConnectionMultiplexer.Connect(options);
});
//read env
DotNetEnv.Env.Load();
//Register Gemini Service
builder.Services.AddScoped<GeminiService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
// Add services to the container.
builder.Services.AddControllers();
//Add Dependency Injection
builder.Services.AddConfig(builder.Configuration);
builder.Services.AddHttpContextAccessor();
//Entity Framework + SQL Server
builder.Services.AddDbContext<AppDbContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//AddEnvironmentVariables
builder.Configuration.AddEnvironmentVariables();
//Cache
builder.Services.AddMemoryCache();
var app = builder.Build();

//Auto Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error Migrating Database");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeederData>();
    await seeder.SeedAdminAccountAsync();
    await seeder.SeedBasicMembershipAsync();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
