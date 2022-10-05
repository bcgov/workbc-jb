using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.Importers.Federal.Models;
using WorkBC.Importers.Federal.Services;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Services;

namespace WorkBC.Importers.Federal
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            Logger logger = new LoggingService(configuration, "WorkBC.Importers.Federal").Logger;

            CommandLineOptions options = null;

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => { options = o; })
                .WithNotParsed(o => { Environment.Exit(0); });

            var service = new XmlImportService(configuration, options, logger);

            if (options.Migrate)
            {
                service.MigrateDb();
                return;
            }

            //Start importer
            logger.Information("IMPORTER STARTED");

            try
            {
                if (!options.ReImport)
                {
                    //Fetch all postings
                    logger.Information("Get all job posting items");
                    List<JobPosting> jobs = await service.GetAllJobPostingItems();

                    //ensure we could load jobs
                    if (jobs != null && jobs.Count > 0)
                    {
                        //Save/Update job posts in DB
                        logger.Information($"{jobs.Count} federal jobbank jobs found in BC or virtual");
                        logger.Information("Importing XML data");
                        await service.ProcessJobs(jobs);
                    }
                }

                //create service
                var syncService = new JobsTableSyncService(configuration, logger, options);

                //Import jobs
                if (options.ReImport)
                {
                    logger.Information("Skipping import to Jobs table (reimport in progress)...");
                }
                else
                {
                    logger.Information("Importing to Jobs table...");
                    await syncService.ImportJobs();
                }

                //Update jobs
                logger.Information("Updating Jobs table...");
                await syncService.UpdateJobs();

                //Remove deleted jobs
                logger.Information("Deactivating old jobs in the Jobs table...");
                await syncService.DeactivateJobs(JobSource.Federal);

                logger.Information($"{await syncService.GetActiveJobsCount()} active federal jobs in the Jobs table after import");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            //Done
            logger.Information("IMPORTER FINISHED");
        }
    }
}