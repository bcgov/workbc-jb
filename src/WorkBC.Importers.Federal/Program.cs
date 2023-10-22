using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using WorkBC.Data;
using WorkBC.Importers.Federal.Models;
using WorkBC.Importers.Federal.Services;
using WorkBC.Importers.Federal.Settings;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;

namespace WorkBC.Importers.Federal
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-CA", false);

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
            
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            var dbContext = new JobBoardContext(connectionString);

            //get API settings
            var federalSettings = new FederalSettings();
            configuration.GetSection("FederalSettings").Bind(federalSettings);
            var baseUrl = federalSettings.FederalJobXmlRoot;
            

            var httpClient = CreateHttpClient(configuration);
            var federalApiService = new FederalApiService(httpClient, baseUrl, logger);
            var xmlImportService = new XmlImportService(federalApiService, dbContext, options, logger);

            if (options.Migrate)
            {
                xmlImportService.MigrateDb();
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
                    List<JobPosting> jobs = await federalApiService.GetAllJobPostingItems();

                    //ensure we could load jobs
                    if (jobs != null && jobs.Count > 0)
                    {
                        //Save/Update job posts in DB
                        logger.Information($"{jobs.Count} federal jobbank jobs found in BC or virtual");
                        logger.Information("Importing XML data");
                        await xmlImportService.ProcessJobs(jobs);
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

        /// <summary>
        ///     Create persistent HttpClient
        /// </summary>
        private static HttpClient CreateHttpClient(IConfiguration configuration)
        {
            //get Proxy settings
            var proxySettings = new ProxySettings();
            configuration.GetSection("ProxySettings").Bind(proxySettings);
            
            //get API settings
            var federalSettings = new FederalSettings();
            configuration.GetSection("FederalSettings").Bind(federalSettings);
            
            //create persistent HttpClient
            var handler = new HttpClientHandler();

            if (proxySettings.UseProxy)
            {
                handler.Proxy = new WebProxy(proxySettings.ProxyHost, proxySettings.ProxyPort)
                {
                    BypassProxyOnLocal = true
                };
            }

            if (proxySettings.IgnoreSslErrors)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true;
            }

            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = new TimeSpan(0,0,5);
            httpClient.DefaultRequestHeaders.Add("Cookie", federalSettings.AuthCookie);

            return httpClient;

        }
    }
}