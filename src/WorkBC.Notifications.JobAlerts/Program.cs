using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.Notifications.JobAlerts.Services;
using WorkBC.Shared.Services;

namespace WorkBC.Notifications.JobAlerts
{
    internal class Program
    {
        /// <summary>
        ///     Entry point to the notification sending scheduled task. creates the service
        ///     that contains the code that gets all the notifications to send and sends
        ///     the emails out to the user.
        /// </summary>
        private static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            Logger logger = new LoggingService(configuration, "WorkBC.Notifications.JobAlerts").Logger;

            logger.Information("JOB ALERT TASK STARTED");

            try
            {
                var service = new JobAlertSenderService(configuration, logger);
                await service.RunJobAlertSender();
            }
            catch (Exception exc)
            {
                logger.Error($"Failed sending notifications:\n{exc}");
            }

            logger.Information("JOB ALERT TASK COMPLETED");
        }
    }
}