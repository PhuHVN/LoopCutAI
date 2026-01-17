using AutoMapper;
using System.Security.Authentication;
using System.Text.Json;

namespace LoopCut.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this._next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = exception switch
            {
                AuthenticationException => StatusCodes.Status401Unauthorized,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                // Validation
                System.ComponentModel.DataAnnotations.ValidationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                FluentValidation.ValidationException => StatusCodes.Status400BadRequest,

                KeyNotFoundException => StatusCodes.Status404NotFound,
                TimeoutException => StatusCodes.Status408RequestTimeout,
                NotImplementedException => StatusCodes.Status501NotImplemented,

                // AutoMapper
                AutoMapperMappingException ame => ame.InnerException is KeyNotFoundException
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };
            context.Response.StatusCode = statusCode;
            object response;

            if (exception is FluentValidation.ValidationException ve)
            {
                var errors = ve.Errors.Select(e => e.ErrorMessage).ToArray();
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    IsSuccess = false,
                    Message = errors
                };
            }
            else
            {
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    IsSuccess = false,
                    Message = exception.Message,
                };
            }

            var result = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(result);
        }
    }
}
