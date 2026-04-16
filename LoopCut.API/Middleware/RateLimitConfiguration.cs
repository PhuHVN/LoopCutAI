using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace LoopCut.API.Middleware
{
    public static class RateLimitConfiguration
    {
        public static void AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Send OTP via Email Rate Limit (1 per 1 minutes) 
                options.AddPolicy("SendOTP", context =>
                {
                    var email = string.Empty;
                    if (context.Request.HasFormContentType)
                    {
                        email = context.Request.Form["email"].ToString();
                    }
                    else if (context.Request.Query.ContainsKey("email"))
                    {
                       email = context.Request.Query["email"].ToString();
                    }
                    var partitionKey = string.IsNullOrEmpty(email)
                        ? context.Connection.RemoteIpAddress?.ToString()
                        : $"send_otp_{email}";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: partitionKey!,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        });
                });                                         
                // Create Payment Rate Limit (5 per minute) - Prevent payment link spam/DoS
                options.AddPolicy("CreatePayment", context =>
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                 context.Connection.RemoteIpAddress?.ToString();
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: $"create_payment_{userId}",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        });
                });

                // Webhook Rate Limit (100 per minute) - PayOS sends webhooks, keep relaxed but monitored
                options.AddPolicy("WebhookLimit", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: $"webhook_{ip}",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        });
                });                              

                //// Chat Message Rate Limit (10 per minute) - Gemini API has costs, prevent spam
                //options.AddPolicy("ChatMessage", context =>
                //{
                //    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                //                 context.Connection.RemoteIpAddress?.ToString();
                //    return RateLimitPartition.GetSlidingWindowLimiter(
                //        partitionKey: $"chat_message_{userId}",
                //        factory: _ => new SlidingWindowRateLimiterOptions
                //        {
                //            PermitLimit = 10,
                //            Window = TimeSpan.FromMinutes(1),
                //            SegmentsPerWindow = 4,
                //            QueueLimit = 0
                //        });
                //});           

                //Global RateLimit (60 per minute for authenticated, 20 for unauthenticated)
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if(!string.IsNullOrEmpty(userId))
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            partitionKey: $"user_{userId}",
                            factory: _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = 60,
                                Window = TimeSpan.FromMinutes(1),
                                SegmentsPerWindow = 4,
                                QueueLimit = 0
                            });
                    }
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: $"ip_{ip}",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        });
                });
                options.OnRejected = async (context,token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Too many requests. Please try again later.",
                        statusCode = 429
                    });
                };
            });
        }
    }
}
   
