using FluentValidation;
using LoopCut.Application.Interfaces;
using LoopCut.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using PayOS;

namespace LoopCut.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Application service registrations go here
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IUserMembershipService, UserMembershipService>();
            services.AddScoped<IServiceDefinitionManager, ServiceDefinitionManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IServicePlanManager, ServicePlanManager>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<PayOSClient>();
            services.AddScoped<IGeminiService, GeminiService>();
            services.AddScoped<IEmailService, EmailService>();


            services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        }


    }
}
