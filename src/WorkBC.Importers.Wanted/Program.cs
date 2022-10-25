using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.Importers.Wanted.Services;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Services;

namespace WorkBC.Importers.Wanted
{
    internal class Program
    {
        private const string InsertHelpLegend = "I = Inserted  U = Updated  S = Skipped (already imported)  F = Skipped (federal job)  H = Duplicate hash";

        private static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-CA", false);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            Logger logger = new LoggingService(configuration, "WorkBC.Importers.Wanted").Logger;

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

            logger.Information("IMPORTER STARTED");
            logger.Information("Importing XML data");

            try
            {
                if (!options.ReImport)
                {
                    if (service.DoBulkImport)
                    {
                        //Bulk import
                        logger.Information($"Starting with bulk import for the past {service.BulkImportDays} days");
                        Console.WriteLine(InsertHelpLegend);

                        //import bulk data
                        await service.ImportBulkData();
                    }
                    else
                    {
                        //Normal import
                        var totalRecords = await service.GetTotalRecords();
                        logger.Information($"{totalRecords} records returned by the Wanted API");
                        Console.WriteLine(InsertHelpLegend);

                        try
                        {
                            await service.ImportXmlData(totalRecords);
                            Console.WriteLine();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            logger.Error("ERROR OCCURED IMPORTING WANTED XML DATA... SKIPPING AHEAD TO THE NEXT TASK");
                        }
                    }

                    logger.Information("Purging jobs");
                    await service.PurgeJobs();
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
                logger.Information("Deactivating old jobs in Jobs table...");
                await syncService.DeactivateJobs(JobSource.Wanted);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            logger.Information("IMPORTER FINISHED");
        }
    }
}