using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using Serilog;
using SendGrid.Helpers.Mail;
using WorkBC.Shared.Settings;
using EmailSettings = WorkBC.Shared.Settings.EmailSettings;

namespace WorkBC.Shared.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ProxySettings _proxySettings;
        private readonly ILogger _logger;

        public SendGridEmailSender(IOptions<EmailSettings> emailSettings, IOptions<ProxySettings> proxySettings, ILogger logger)
        {
            _emailSettings = emailSettings.Value;
            _proxySettings = proxySettings.Value;
            _logger = logger;
        }

        public SendGridEmailSender(EmailSettings emailSettings, ProxySettings proxySettings, ILogger logger)
        {
            _emailSettings = emailSettings;
            _proxySettings = proxySettings;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string textMessage)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(subject))
            {
                return;
            }

            string fromEmail = _emailSettings.SendGridFromEmail ?? "noreply@gov.bc.ca";
            string fromName = _emailSettings.FromName ?? "WorkBC.ca No Reply";

            var msg = new SendGridMessage
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                PlainTextContent = textMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            string apiKey = _emailSettings.SendGridKey ?? "";

            HttpClientHandler handler = GetHandler();
            using (var httpClient = new HttpClient(handler))
            {
                var client = new SendGridClient(httpClient, apiKey);
                var response = await client.SendEmailAsync(msg);
                _logger.Debug(JsonConvert.SerializeObject(response));
            }
        }

        private HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();

            if (_proxySettings.UseProxy)
            {
                handler.Proxy = new WebProxy(_proxySettings.ProxyHost, _proxySettings.ProxyPort)
                {
                    BypassProxyOnLocal = true
                };
            }

            if (_proxySettings.IgnoreSslErrors)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true;
            }

            return handler;
        }
    }
}