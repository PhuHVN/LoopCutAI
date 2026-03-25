using LoopCut.Application.Interfaces;
using LoopCut.Application.Options;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

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

        public async Task SendPaymentSuccessEmailAsync(string orderId)
        {
            try
            {
                var payment = await _unitOfWork.GetRepository<Payment>().FindAsync(p => p.OrderCode == orderId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for order {OrderId}", orderId);
                    return;
                }

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(payment.UserId);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for payment {PaymentId}", payment.Id);
                    return;
                }

                var subject = $"[LoopCutAI] Invoice for {payment.Membership.Name} Membership - {payment.OrderCode}";

                var body = $@"
<html>
<body style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; margin: 0; padding: 0; background-color: #f0f2f5;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <div style='max-width: 550px; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.08); border: 1px solid #e6ebf1;'>
                    
                    <div style='background-color: #1a1f36; padding: 40px 30px; text-align: center; color: #ffffff;'>
                        <div style='background: rgba(255,255,255,0.1); width: 60px; height: 60px; border-radius: 50%; margin: 0 auto 20px; line-height: 60px; font-size: 30px;'>✓</div>
                        <h1 style='margin: 0; font-size: 24px; letter-spacing: -0.5px;'>Payment Received</h1>
                        <p style='margin: 10px 0 0; opacity: 0.8; font-size: 16px;'>Thank you for choosing LoopCut</p>
                    </div>

                    <div style='padding: 40px 35px;'>
                        <p style='font-size: 18px; color: #1a1f36; margin-bottom: 20px;'>Hi <strong>{account.FullName}</strong>,</p>
                        <p style='font-size: 15px; color: #4f566b; line-height: 1.6; margin-bottom: 30px;'>
                            Your payment for the <span style='color: #5469d4; font-weight: 600;'>{payment.Membership.Name}</span> plan was successful. 
                            Your subscription is now active and all premium features are unlocked.
                        </p>

                        <div style='padding: 25px; border-radius: 12px; background-color: #f8fafc; border: 1px solid #edf2f7;'>
                            <table width='100%'>
                                <tr>
                                    <td style='font-size: 11px; color: #a3acb9; text-transform: uppercase; letter-spacing: 1px; padding-bottom: 8px;'>Order ID</td>
                                    <td align='right' style='font-size: 11px; color: #a3acb9; text-transform: uppercase; letter-spacing: 1px; padding-bottom: 8px;'>Date</td>
                                </tr>
                                <tr>
                                    <td style='font-size: 15px; color: #1a1f36; font-weight: bold;'>#{payment.OrderCode}</td>
                                    <td align='right' style='font-size: 15px; color: #1a1f36; font-weight: bold;'>{payment.CreatedAt:dd/MM/yyyy}</td>
                                </tr>
                            </table>

                            <div style='margin: 20px 0; border-top: 1px dashed #e2e8f0;'></div>

                            <table width='100%'>
                                <tr>
                                    <td style='font-size: 15px; color: #4f566b;'>{payment.Membership.Name} Membership Plan</td>
                                    <td align='right' style='font-size: 15px; color: #4f566b;'>{payment.Membership.Price:N0} VND</td>
                                </tr>
                                <tr>
                                    <td style='padding-top: 15px; font-size: 18px; color: #1a1f36; font-weight: bold;'>Total Paid</td>
                                    <td align='right' style='padding-top: 15px; font-size: 20px; color: #5469d4; font-weight: 800;'>{payment.Membership.Price:N0} VND</td>
                                </tr>
                            </table>
                        </div>

                        <div style='margin-top: 30px; text-align: center;'>
                            <span style='background-color: #dcfce7; color: #15803d; padding: 8px 16px; border-radius: 30px; font-size: 12px; font-weight: 700; display: inline-block; border: 1px solid #bbf7d0; text-transform: uppercase;'>
                                Status: {payment.Status.ToString().ToUpper()}
                            </span>
                        </div>
                    </div>

                    <div style='background-color: #f7fafc; padding: 30px; text-align: center; border-top: 1px solid #edf2f7;'>
                        <p style='margin: 0; font-size: 13px; color: #718096; line-height: 1.5;'>
                            If you have any questions, reply to this email or visit our <a href='#' style='color: #5469d4; text-decoration: none;'>Help Center</a>.
                        </p>
                        <p style='margin: 10px 0 0; font-size: 12px; color: #a3acb9;'>
                            &copy; 2026 LoopCut Team. All rights reserved.
                        </p>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</body>
</html>";

                await SendEmailAsync(account.Email, subject, body);
                _logger.LogInformation("Payment success email sent to {Email} for order {OrderId}", account.Email, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment success email for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task SendRegistrationOtpEmailAsync(string email, string otp)
        {
            try
            {
                var subject = $"[LoopCutAI] Registration code  ";

                var body = $@"
    <html>
    <body style='font-family: ""Segoe UI"", Tahoma, sans-serif; background-color: #f4f7f9; padding: 20px; margin: 0;'>
        <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); border: 1px solid #e1e8ed;'>
            
            <div style='background-color: #2c3e50; padding: 25px; text-align: center;'>
                <h2 style='color: #ffffff; margin: 0; font-size: 22px; letter-spacing: 1px;'>Verification Code</h2>
            </div>

            <div style='padding: 30px; text-align: center; color: #333;'>
                <p style='font-size: 16px; margin-bottom: 20px;'>Welcome to <strong>LoopCut</strong>!</p>
                <p style='font-size: 14px; color: #666;'>Please use the following One-Time Password (OTP) to complete your registration. This code is valid for <strong>5 minutes</strong>.</p>
                
                <div style='margin: 30px 0; padding: 15px; background-color: #f8f9fa; border: 2px dashed #cbd5e0; border-radius: 8px;'>
                    <span style='font-family: monospace; font-size: 36px; font-weight: bold; color: #2d3748; letter-spacing: 8px;'>{otp}</span>
                </div>

                <p style='font-size: 13px; color: #a0aec0; font-style: italic;'>If you did not request this code, please ignore this email or contact support.</p>
            </div>

            <div style='background-color: #f8fafc; padding: 15px; text-align: center; border-top: 1px solid #edf2f7;'>
                <p style='margin: 0; font-size: 12px; color: #718096;'>&copy; 2026 LoopCut Team. All rights reserved.</p>
            </div>
        </div>
    </body>
    </html>";

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation("Registration OTP email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending registration OTP email to {Email}", email);
                throw;
            }
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
