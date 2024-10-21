using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.Data;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.Shared.Services;

namespace EFMigrationRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Set Npgsql to avoid complaining about Datetimes.
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(fileInfo.DirectoryName)
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            Logger logger = new LoggingService(configuration, "EFMigrationRunner").Logger;
            bool applyDatabaseMigration = bool.Parse(configuration.GetSection("AppSettings").GetValue<string>("ApplyMigrations"));

            if (applyDatabaseMigration)
            {
                string defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

                // The only way DefaultConnection ever get a value is from user secrets.
                // Otherwise MigrationConnnection comes from the appsetting.json file.
                string connectionString = string.IsNullOrEmpty(defaultConnectionString)
                    ? configuration.GetConnectionString("MigrationRunnerConnection")
                    : defaultConnectionString;

                var dbContext = new JobBoardContext(connectionString);

                var service = new MigrationService(dbContext, logger);

                logger.Information("EF MIGRATION RUNNER STARTED");

                if (!service.RunDbMigrations())
                {
                    logger.Information(
                        "Once you have corrected the data issue, you can run the migrations manually with the following command:");

                    logger.Information(
                        @"E:\Scheduled Tasks\WorkBC.Indexers.Federal\WorkBC.Indexers.Federal.exe --migrate");

                    logger.Information(
                        "If you are on a government server, then your IDIR account must be a member of the db_owner role on the JobBoard database to run the migrations manually using this method.");

                    logger.Error("MIGRATIONS FAILED");
                    logger.Error(
                        $@"A log has been written to {configuration["Serilog:WorkBCLogPath"]}\EFMigrationRunner");
                }
            }
            else
            {
                logger.Information(@"Migrations are disabled. The appsettings ApplyMigrations is set to ""false""");
            }

            logger.Information("EF MIGRATION RUNNER FINISHED");
        }
    }
}