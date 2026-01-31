using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoopCut.Infrastructure.BackgroundTasks
{
    public class SubscriptionEmailChecker : BackgroundService
    {
        private readonly ILogger<SubscriptionEmailChecker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionEmailChecker(ILogger<SubscriptionEmailChecker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Subscription Expiration Checker running at: {time}", DateTimeOffset.Now);
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await ProcessExpiringSubscriptionsAsync(dbContext);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SubscriptionEmailChecker ExecuteAsync");
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessExpiringSubscriptionsAsync(AppDbContext dbContext)
        {
            var expiringSubscriptions = await GetExpiringSubscriptionsAsync(dbContext);
            foreach (var subscription in expiringSubscriptions)
            {

                _logger.LogInformation($"Processing subscription {subscription.Id} for account {subscription.AccountId}");
                subscription.Status = SubscriptionEnums.Expired;
                dbContext.Subcriptions.Update(subscription);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating subscription {subscription.Id}");
                }
                _logger.LogInformation($"Subscription {subscription.Id} status updated to Expired");
            }

        }


        private async Task<List<Subscriptions>> GetExpiringSubscriptionsAsync(AppDbContext context)
        {
            var now = DateTime.UtcNow;
            return await context.Subcriptions
                .Where(s => s.EndDate != null && s.Status == SubscriptionEnums.Active && s.EndDate <= now)
                .ToListAsync();
        }
    }
}
