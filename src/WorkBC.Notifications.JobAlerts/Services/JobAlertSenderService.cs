using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;
using EmailSettings = WorkBC.Shared.SystemSettings.EmailSettings;

namespace WorkBC.Notifications.JobAlerts.Services
{
    /// <summary>
    ///     This is the service that is used to handle the job alert scheduled
    ///     task and handles the checking of which ones to send and then sends
    ///     the emails out
    /// </summary>
    public class JobAlertSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly JobBoardContext _dbContext;
        private readonly JobAlertSearchService _jobAlertSearchService;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;

        public JobAlertSenderService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);
            _jobAlertSearchService = new JobAlertSearchService(configuration, _dbContext);
            var emailSettings = configuration.GetSection("EmailSettings").Get<Shared.Settings.EmailSettings>();
            var proxySettings = configuration.GetSection("ProxySettings").Get<ProxySettings>();

            _emailSender = emailSettings.UseSmtp
                ? (IEmailSender) new SmtpEmailSender(emailSettings, _logger)
                : new SendGridEmailSender(emailSettings, proxySettings, _logger);

            Dictionary<string, string> emailSettingsDict = _dbContext.SystemSettings
                .Where(s => EF.Functions.Like(s.Name, "email.%"))
                .ToDictionary(k => k.Name, v => v.Value);

            _emailSettings = new EmailSettings(emailSettingsDict);
        }

        /// <summary>
        ///     Called to run the task that checks the date against the type of job
        ///     alert frequencies and then calls the methods that generate the list
        ///     of emails to send
        /// </summary>
        public async Task RunJobAlertSender()
        {
            var emails = new List<JobAlertEmail>();
            var jobAlerts = new List<JobAlert>();

            DateTime dateTime = DateTime.Now;

            if (dateTime.DayOfWeek == DayOfWeek.Monday)
            {
                // weekly reports
                _logger.Information("Getting weekly job alerts");

                jobAlerts.AddRange(await GetJobAlertsAsync(JobAlertFrequency.Weekly));
            }

            if (dateTime.Day == 1)
            {
                // monthly reports
                _logger.Information("Getting monthly job alerts");

                jobAlerts.AddRange(await GetJobAlertsAsync(JobAlertFrequency.Monthly));
            }

            if (dateTime.Day == 15 || dateTime.Day == LastDayOfMonthFromDateTime(dateTime).Day)
            {
                // biweekly reports
                _logger.Information("Getting biweekly job alerts");

                jobAlerts.AddRange(await GetJobAlertsAsync(JobAlertFrequency.BiWeekly));
            }

            // daily reports
            _logger.Information("Getting daily job alerts");

            jobAlerts.AddRange(await GetJobAlertsAsync(JobAlertFrequency.Daily));

            // get the job alerts out into their email format
            foreach (JobAlert jobAlert in jobAlerts)
            {
                try
                {
                    long resultCount = await _jobAlertSearchService.GetJobAlertSearchResultCount(jobAlert);

                    if (resultCount > 0)
                    {
                        var email = new JobAlertEmail(jobAlert, _configuration, _emailSettings);
                        emails.Add(email);
                    }
                }
                catch (Exception exc)
                {
                    _logger.Error($"Failed to get jobs for job alert id: {jobAlert.Id}\n{exc}");
                }
            }

            await SendJobAlertEmails(emails);
        }

        /// <summary>
        ///     This method is called to send the emails once all the job alerts that have to be
        ///     sent out in the current batch are in a list. This method then sorts the emails by
        ///     the domain in order to prevent multiple open and send connections to speed up email sending.
        /// </summary>
        private async Task SendJobAlertEmails(List<JobAlertEmail> emails)
        {
            emails.Sort();
            string testingEmail = _configuration["AppSettings:SendEmailTestingTo"];

            var count = 0;

            foreach (JobAlertEmail email in emails)
            {
                var sendToEmail = "";
                try
                {
                    sendToEmail = email.EmailAddress;
                    if (!string.IsNullOrWhiteSpace(testingEmail))
                    {
                        sendToEmail = testingEmail;
                    }

                    await _emailSender.SendEmailAsync(sendToEmail, email.EmailSubject, email.HtmlMessage, email.TextMessage);
                    ;
                    //now we update the database or log the successful send?? Maybe not.
                    _logger.Information($"Send email to:{sendToEmail}");

                    count++;
                }
                catch (FormatException)
                {
                    // The e-mail address was invalid.
                    // This is typically caused by the data being cleansed by ISB and replaced with a string of 'x' characters.
                    // We don't log these as they result in the log files being full of these errors.
                }
                catch (Exception exc)
                {
                    _logger.Error($"Error sending email to: {sendToEmail}\n{exc}");
                }
            }

            _logger.Information($"Sent {count} job alert emails");
        }

        /// <summary>
        ///     For the last day of the month we get the first day of the month, add 1 month, then
        ///     subtract 1 day.  Returns the last day of the month for the passed in DateTime object.
        /// </summary>
        private static DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            var firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        ///     Gets all the job alerts in the system by the frequency passed into it (not user specific)
        ///     Used during the scheduled tasks.
        /// </summary>
        private async Task<IList<JobAlert>> GetJobAlertsAsync(JobAlertFrequency frequency)
        {
            return await _dbContext.JobAlerts
                .Where(a => a.AlertFrequency == frequency && a.JobSeeker.AccountStatus == AccountStatus.Active && !a.IsDeleted)
                .Select(s => new JobAlert
                {
                    Id = s.Id,
                    AlertFrequency = s.AlertFrequency,
                    AspNetUserId = s.AspNetUserId,
                    JobSearchFilters = s.JobSearchFilters,
                    JobSearchFiltersVersion = s.JobSearchFiltersVersion,
                    Title = s.Title,
                    JobSeeker = new JobSeeker
                    {
                        Id = s.JobSeeker.Id,
                        FirstName = s.JobSeeker.FirstName,
                        Email = s.JobSeeker.Email,
                    }
                })
                .ToListAsync();
        }
    }
}