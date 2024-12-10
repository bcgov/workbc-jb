using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.Indexers.Federal.Services;
using WorkBC.Shared.Services;

namespace WorkBC.Indexers.Federal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set Npgsql to avoid complaining about Datetimes.
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            CultureInfo.CurrentCulture = new CultureInfo("en-CA", false);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            Logger logger = new LoggingService(configuration, "WorkBC.Indexers.Federal").Logger;

            CommandLineOptions options = null;

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => { options = o; })
                .WithNotParsed(o => { Environment.Exit(0); });

            //setup configuration
            var service = new FederalIndexService(configuration, options, logger);

            if (options.Debug)
            {
                await service.Debug();
                return;
            }

            try
            {
                if (options.SkipReIndex || options.Migrate)
                {
                    await service.MainTask();
                }
                else
                {
                    logger.Information("INDEXER STARTED");

                    //Index jobs
                    logger.Information("Indexing jobs - start");
                    try
                    {
                        await service.MainTask();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        logger.Error("ERROR OCCURED RUNNING MAIN TASK... SKIPPING AHEAD TO THE NEXT TASK");
                    }

                    logger.Information("Indexing jobs - done.");

                    //Purge jobs
                    if (!options.ReIndex)
                    {
                        logger.Information("Purging expired jobs from Elasticsearch");
                        await service.PurgeJobs();
                    }

                    logger.Information("INDEXER FINISHED");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
    }
}
