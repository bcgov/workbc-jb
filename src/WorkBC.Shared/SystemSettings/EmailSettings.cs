using System.Collections.Generic;

namespace WorkBC.Shared.SystemSettings
{
    public class EmailSettings
    {
        public EmailClass PasswordReset = new EmailClass();
        public EmailClass Registration = new EmailClass();

        public EmailSettings(Dictionary<string, string> settings)
        {
            PasswordReset.BodyText = settings["email.passwordReset.bodyText"];
            PasswordReset.BodyHtml = settings["email.passwordReset.bodyHtml"];
            PasswordReset.Subject = settings["email.passwordReset.subject"];

            Registration.BodyText = settings["email.registration.bodyText"];
            Registration.BodyHtml = settings["email.registration.bodyHtml"];
            Registration.Subject = settings["email.registration.subject"];
        }

        public class EmailClass
        {
            public string Subject { get; set; }
            public string BodyText { get; set; }
            public string BodyHtml { get; set; }
        }
    }
}