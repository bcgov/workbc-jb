using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WorkBC.Shared.Settings;

namespace WorkBC.Shared.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly Serilog.ILogger _logger;

        public SmtpEmailSender(IOptions<EmailSettings> emailSettings, Serilog.ILogger logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public SmtpEmailSender(EmailSettings emailSettings, Serilog.ILogger logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(subject))
            {
                return;
            }

            string fromEmail = _emailSettings.FromEmail ?? "noreply@gov.bc.ca";
            string fromName = _emailSettings.FromName ?? "WorkBC.ca No Reply";

            try
            {
                var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Body = textMessage,
                    Subject = subject
                };

                mailMessage.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, "text/html"));

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SmtpEmailSender.SendEmailAsync failed");
                throw;
            }
        }
    }
}