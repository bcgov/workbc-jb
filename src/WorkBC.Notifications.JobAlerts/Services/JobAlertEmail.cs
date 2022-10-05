using System;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.SystemSettings;

namespace WorkBC.Notifications.JobAlerts.Services
{
    /// <summary>
    ///     Used to store the information that is sent to the email for the notification
    /// </summary>
    public class JobAlertEmail : IComparable<JobAlertEmail>
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;


        public JobAlertEmail(JobAlert jobAlert, IConfiguration configuration, EmailSettings emailSettings)
        {
            EmailAddress = jobAlert.JobSeeker.Email;
            JobAlert = jobAlert;
            _configuration = configuration;
            _emailSettings = emailSettings;
        }

        /// <summary>
        ///     Returns the base template for the email subject
        /// </summary>
        public string EmailSubject => string.Format(_emailSettings.JobAlert.Subject, JobAlert.Title);

        /// <summary>
        ///     Returns the notification url, reads in the base url (without a trailing /)
        ///     from the config file
        /// </summary>
        private string NotificationUrl
        {
            get
            {
                int jobAlertId = JobAlert.Id;
                string userId = JobAlert.JobSeeker.Id;
                string jbSearchUrl = _configuration["AppSettings:JbSearchUrl"];
                return $"{jbSearchUrl}#/job-search/r;nid={jobAlertId};jsid={userId}";
            }
        }

        /// <summary>
        ///     Returns the email message for the email
        /// </summary>
        public string TextMessage
        {
            get
            {
                string jobAlertTitle = JobAlert.Title;
                string frequency = GetFrequencyStringOfNotification();
                string firstName = JobAlert.JobSeeker.FirstName;
                string template = _emailSettings.JobAlert.BodyText;
                return string.Format(template, firstName, frequency, jobAlertTitle, NotificationUrl);
            }
        }

        /// <summary>
        ///     returns the email message for the email
        /// </summary>
        public string HtmlMessage
        {
            get
            {
                string jobAlertTitle = JobAlert.Title;
                string frequency = GetFrequencyStringOfNotification();
                string firstName = JobAlert.JobSeeker.FirstName;
                string template = _emailSettings.JobAlert.BodyHtml;
                return string.Format(template, firstName, frequency, jobAlertTitle, NotificationUrl, EmailSubject);
            }
        }

        public JobAlert JobAlert { get; }

        public string EmailAddress { get; }

        public string EmailDomain => EmailAddress.Substring(EmailAddress.IndexOf("@", StringComparison.Ordinal) + 1);

        #region IComparable<NotificationSenderTaskEmail> Members

        /// <summary>
        ///     Used to override the default sorting so that the sorting is done by email domain.
        /// </summary>
        public int CompareTo(JobAlertEmail other)
        {
            return string.Compare(EmailDomain, other.EmailDomain, StringComparison.Ordinal);
        }

        #endregion

        private string GetFrequencyStringOfNotification()
        {
            switch (JobAlert.AlertFrequency)
            {
                case JobAlertFrequency.Daily:
                    return "daily";
                case JobAlertFrequency.Weekly:
                    return "weekly";
                case JobAlertFrequency.BiWeekly:
                    return "biweekly";
                case JobAlertFrequency.Monthly:
                    return "monthly";
                default:
                    return "unscheduled";
            }
        }
    }
}