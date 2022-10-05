namespace WorkBC.Shared.Settings
{
    public class EmailSettings
    {
        public bool UseSmtp { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpServer { get; set; }
        public string SendGridKey { get; set; }
        public string SendGridFromEmail { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}