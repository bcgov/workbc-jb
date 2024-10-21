using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.Indexers.Wanted.Services;
using WorkBC.Shared.Services;

namespace WorkBC.Indexers.Wanted
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
            Logger logger = new LoggingService(configuration, "WorkBC.Indexers.Wanted").Logger;

            CommandLineOptions options = null;

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => { options = o; })
                .WithNotParsed(o => { Environment.Exit(0); });

            var service = new WantedIndexService(configuration, options, logger);

            if (options.Debug)
            {
                await service.Debug();
                return;
            }

            logger.Information("INDEXER STARTED");
            logger.Information("Creating and updating indexes");

            try
            {
                await service.MainTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                logger.Error("ERROR PROCESSING RECORDS... SKIPPING AHEAD TO THE NEXT TASK");
            }

            if (!options.ReIndex)
            {
                try
                {
                    logger.Information("Purging expired jobs from Elasticsearch");
                    await service.PurgeJobs();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
            }

            logger.Information("INDEXER FINISHED");
        }
    }
}
