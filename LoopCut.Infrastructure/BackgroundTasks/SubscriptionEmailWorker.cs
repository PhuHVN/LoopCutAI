using LoopCut.Application.Interfaces;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoopCut.Infrastructure.BackgroundTasks
{
    public class SubscriptionEmailWorker : BackgroundService
    {
        private readonly ILogger<SubscriptionEmailWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionEmailWorker(
            ILogger<SubscriptionEmailWorker> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Email Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        // Process Daily Reminder Emails
                        await ProcessSubscriptionEmailsAsync(dbContext, emailService, SubscriptionEmailType.DailyReminder);
                        // Process Expired Notice Emails
                        await ProcessSubscriptionEmailsAsync(dbContext, emailService, SubscriptionEmailType.ExpiredNotice);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SubscriptionEmailWorker ExecuteAsync");
                }

                // Wait for 30 minutes before next check
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }


        private async Task ProcessSubscriptionEmailsAsync(AppDbContext dbContext, IEmailService emailService, SubscriptionEmailType subscriptionEmailType)
        {
            var subscriptions = await GetSubscriptionsByTypeAsync(dbContext, subscriptionEmailType);
            foreach (var sub in subscriptions)
            {
                try
                {
                    var emailLog = new SubscriptionEmailLog
                    {
                        SubscriptionId = sub.Id,
                        AccountId = sub.AccountId,
                        Type = subscriptionEmailType,
                        SentAt = DateTime.UtcNow,
                        IsSuccess = false
                    };

                    // Log email attempt, use unique index to prevent duplicates

                    dbContext.SubscriptionEmailLogs.Add(emailLog);
                    await dbContext.SaveChangesAsync();

                    // Send email based on subscriptionEmailType

                    switch (subscriptionEmailType)
                    {
                        case SubscriptionEmailType.DailyReminder:
                            await emailService.SendSubscriptionReminderEmailAsync(sub);
                            break;
                        case SubscriptionEmailType.ExpiredNotice:
                            await emailService.SendSubscriptionExpiredEmailAsync(sub);
                            break;
                    }

                    emailLog.IsSuccess = true;
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogInformation("Subscription {Id}  is already sent email type {Type}.", sub.Id, subscriptionEmailType);
                    dbContext.Entry(ex.Entries.First().Entity).State = EntityState.Detached;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing subscription email for Subscription ID: {SubscriptionId}", sub.Id);
                }
            }
        }

        private async Task<List<Subscriptions>> GetSubscriptionsByTypeAsync(AppDbContext context, SubscriptionEmailType subscriptionEmailType)
        {
            var now = DateTime.UtcNow;

            switch (subscriptionEmailType)
            {
                case SubscriptionEmailType.DailyReminder:
                    return await context.Subcriptions
                        .Where(s =>
                        s.EndDate != null
                        && s.Status == SubscriptionEnums.Active
                        && s.EndDate > now
                        && s.EndDate <= now.AddDays(s.RemiderDays + 1))
                        .ToListAsync();


                case SubscriptionEmailType.ExpiredNotice:
                    return await context.Subcriptions
                        .Where(s => s.EndDate != null
                        && (s.Status == SubscriptionEnums.Active || s.Status == SubscriptionEnums.Expired)
                        && s.EndDate <= now)

                        .ToListAsync();
                default:
                    return await Task.FromResult(new List<Subscriptions>());
            }
        }

    }
}
