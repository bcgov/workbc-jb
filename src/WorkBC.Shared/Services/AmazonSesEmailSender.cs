using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using EmailSettings = WorkBC.Shared.Settings.EmailSettings;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using System.Collections.Generic;
using Amazon;

namespace WorkBC.Shared.Services
{
    public class AmazonSesEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger _logger;

        public AmazonSesEmailSender(IOptions<EmailSettings> emailSettings, ILogger logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public AmazonSesEmailSender(EmailSettings emailSettings, ILogger logger)
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

            string fromEmail = _emailSettings.FromEmail ?? "noreply@workbc.ca";
            string fromName = _emailSettings.FromName ?? "WorkBC.ca No Reply";

            var msg = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content(textMessage),
                    Html = new Content(htmlMessage)
                }
            };

            using (IAmazonSimpleEmailService client = new AmazonSimpleEmailServiceClient(
                RegionEndpoint.GetBySystemName(_emailSettings.SesRegion)
            ))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = $"{fromName} <{fromEmail}>",
                    Destination = new Destination { ToAddresses = new List<string> { email } },
                    Message = msg
                };

                // Send email using AWS SES
                SendEmailResponse response = await client.SendEmailAsync(sendRequest);

                // log debug info
                _logger.Debug(JsonConvert.SerializeObject(response));
            }
        }
    }
}