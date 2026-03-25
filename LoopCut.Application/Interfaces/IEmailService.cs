using LoopCut.Domain.Entities;

namespace LoopCut.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        //registration otp email
        Task SendRegistrationOtpEmailAsync(string email, string otp);

        //payment success email
        Task SendPaymentSuccessEmailAsync(string orderId);

        //subscription reminder email
        Task SendSubscriptionReminderEmailAsync(Subscriptions subscription);

        Task SendSubscriptionExpiredEmailAsync(Subscriptions subscription);
    }
}
