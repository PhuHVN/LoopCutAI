using LoopCut.Application.Interfaces;
using LoopCut.Application.Options;
using LoopCut.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using LoopCut.Domain.Entities;

namespace LoopCut.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IUnitOfWork unitOfWork, IOptions<EmailSettings> settings)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailSettings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _emailSettings.Host,
                _emailSettings.Port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendSubscriptionExpiredEmailAsync(Subscriptions subscription)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(subscription.AccountId);

                if (account == null)
                {
                    _logger.LogWarning("Account not found for subscription {SubscriptionId}", subscription.Id);
                    return;
                }

                var servicePlan = subscription.ServicePlan;
                var planName = servicePlan?.PlanName ?? subscription.SubscriptionsName;

                var subject = "Subscription Expired";
                var body = $@"
                 <html>
             <body style='font-family: Arial, sans-serif;'>
           <h2>Subscription Expired</h2>
               <p>Dear {account.FullName},</p>
           <p>Your subscription <strong>{planName}</strong> has expired on <strong>{subscription.EndDate:yyyy-MM-dd}</strong>.</p>
                    <p>To continue using our services, please renew your subscription.</p>
                    <p>If you have any questions, please contact our support team.</p>
            <br/>
                 <p>Best regards,</p>
            <p><strong>LoopCut Team</strong></p>
          </body>
              </html>";

                await SendEmailAsync(account.Email, subject, body);
                _logger.LogInformation("Subscription expired email sent to {Email} for subscription {SubscriptionId}", account.Email, subscription.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending subscription expired email for subscription {SubscriptionId}", subscription.Id);
                throw;
            }
        }

        public async Task SendSubscriptionReminderEmailAsync(Subscriptions subscription)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(subscription.AccountId);

                if (account == null)
                {
                    _logger.LogWarning("Account not found for subscription {SubscriptionId}", subscription.Id);
                    return;
                }

                var servicePlan = subscription.ServicePlan;
                var planName = servicePlan?.PlanName ?? subscription.SubscriptionsName;
                var daysRemaining = subscription.EndDate.HasValue
              ? (subscription.EndDate.Value - DateTime.UtcNow).Days
               : 0;

                var subject = "Subscription Renewal Reminder";
                var body = $@"
     <html>
             <body style='font-family: Arial, sans-serif;'>
     <h2>Subscription Renewal Reminder</h2>
       <p>Dear {account.FullName},</p>
   <p>This is a reminder that your subscription <strong>{planName}</strong> will expire in <strong>{daysRemaining} days</strong> on <strong>{subscription.EndDate:yyyy-MM-dd}</strong>.</p>
          <p>To ensure uninterrupted service, please renew your subscription before it expires.</p>
           <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
 <br/>
     <p>Best regards,</p>
            <p><strong>LoopCut Team</strong></p>
        </body>
          </html>";

                await SendEmailAsync(account.Email, subject, body);
                _logger.LogInformation("Subscription reminder email sent to {Email} for subscription {SubscriptionId}", account.Email, subscription.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending subscription reminder email for subscription {SubscriptionId}", subscription.Id);
                throw;
            }
        }
    }
}
