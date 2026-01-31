using LoopCut.Domain.Entities;

namespace LoopCut.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);

        Task SendSubscriptionReminderEmailAsync(Subscriptions subscription);

        Task SendSubscriptionExpiredEmailAsync(Subscriptions subscription);
    }
}
