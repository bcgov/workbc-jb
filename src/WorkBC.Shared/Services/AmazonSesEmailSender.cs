using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using WorkBC.Shared.Settings;
using EmailSettings = WorkBC.Shared.Settings.EmailSettings;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using System.Collections.Generic;

namespace WorkBC.Shared.Services
{
    public class AmazonSesEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ProxySettings _proxySettings;
        private readonly ILogger _logger;

        public AmazonSesEmailSender(IOptions<EmailSettings> emailSettings, IOptions<ProxySettings> proxySettings, ILogger logger)
        {
            _emailSettings = emailSettings.Value;
            _proxySettings = proxySettings.Value;
            _logger = logger;
        }

        public AmazonSesEmailSender(EmailSettings emailSettings, ProxySettings proxySettings, ILogger logger)
        {
            _emailSettings = emailSettings;
            _proxySettings = proxySettings;
            _logger = logger;

            //AmazonSimpleEmailServiceConfig config = new AmazonSimpleEmailServiceConfig();
            //client = new AmazonSimpleEmailServiceClient((this._amazonAccessKey, this._amazonSecretKey, config);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(subject))
            {
                return;
            }

            string fromEmail = "noreply@gov.bc.ca";
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

            using (IAmazonSimpleEmailService client = new AmazonSimpleEmailServiceClient("", "",
                Amazon.RegionEndpoint.CACentral1))
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