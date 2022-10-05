using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.Importers.Wanted.Settings;
using WorkBC.Shared.Settings;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Importers.Wanted.Services
{
    public class XmlImportService
    {
        private readonly JobBoardContext _dbContext;
        private readonly ILogger _logger;
        private readonly ProxySettings _proxySettings;
        private readonly WantedSettings _settings;
        private readonly string _connectionString;

        public XmlImportService(IConfiguration configuration, CommandLineOptions options, ILogger logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(_connectionString);

            _logger = logger;

            // get API settings
            _settings = new WantedSettings();
            configuration.GetSection("WantedSettings").Bind(_settings);

            //This will need to be changed later
            //For now this importer will only update jobs for today
            _settings.DateFrom = DateTime.Now.AddDays(-_settings.DaysPerImport).ToString("yyyy-MM-dd");
            _settings.DateEnd = DateTime.Now.ToString("yyyy-MM-dd");

            //set property
            DoBulkImport = options.Bulk;
            BulkImportDays = _settings.JobExpiryDays;

            //get Proxy settings
            _proxySettings = new ProxySettings();
            configuration.GetSection("ProxySettings").Bind(_proxySettings);
        }

        public bool DoBulkImport { get; }
        public int BulkImportDays { get; }

        /// <summary>
        ///     Import jobs into DB
        /// </summary>
        public async Task ImportXmlData(int totalRecords)
        {
            decimal totalPages = Math.Ceiling(totalRecords / _settings.PageSize);
            List<long> jobIdsSeen = new List<long>();

            for (var i = 0; i < totalPages; i++)
            {
                //Max of 2000 results can be processed
                if ((i + 1) * _settings.PageSize <= 2000)
                {
                    //set page number
                    _settings.PageIndex = i + 1;

                    //build URL
                    string url = BuildApiUrl();

                    //make web request
                    XmlDocument xmlResult = await GetWebResponse(url);

                    if (_settings.PageIndex == 1)
                    {
                        _logger.Information("GET " + url);
                    }
                    else
                    {
                        Console.Write($"[pageindex={i + 1}]");
                    }

                    if (xmlResult != null)
                    {
                        //Get the root element
                        XmlElement root = xmlResult.DocumentElement;

                        //Find all documents
                        XmlNodeList nodes = root.SelectNodes("/response/jobs/job");

                        //process jobs
                        jobIdsSeen.AddRange(await ProcessJobs(nodes));
                    }
                    else
                    {
                        //stop loop - there is a problem with the server
                        break;
                    }
                }
                else
                {
                    _logger.Warning(
                        "WARNING - ONLY THE FIRST 2000 RESULTS WERE IMPORTED DUE TO WANTED API RESTRICTIONS!");
                    break;
                }
            }

            // record the fact that we saw these jobs
            // (to be use for smarter expiration later)
            SetJobsSeen(jobIdsSeen);
        }

        /// <summary>
        ///     Only call this the first time when there is no data in the database
        /// </summary>
        public async Task ImportBulkData()
        {
            //Import each day on its own since we don't know before-hand how many results each day there will be.
            //Start with the OLDEST job, and work our way up to the latest. This way we update the jobs correctly.
            for (int k = BulkImportDays - 1; k >= 0; k--)
            {
                //from date
                _settings.DateFrom = DateTime.Now.AddDays(-(k + 1)).ToString("yyyy-MM-dd");

                //to date
                //_settings.DateEnd = DateTime.Now.AddDays(-k).ToString("yyyy-MM-dd");  
                _settings.DateEnd = _settings.DateFrom; // only import 1 day, not 2

                //total records for specific date
                int records = await GetTotalRecords();

                _logger.Information($"{_settings.DateFrom} - {_settings.DateEnd} - {records} records to import");

                //Import for date
                await ImportXmlData(records);
                Console.WriteLine();
            }
        }

        /// <summary>
        ///     Process jobs - save them in the DB
        /// </summary>
        private async Task<List<long>> ProcessJobs(XmlNodeList jobs)
        {
            var jobIds = new List<long>();

            //loop through jobs
            foreach (XmlNode node in jobs)
            {
                //job id
                var id = Convert.ToInt64(node.Attributes["id"].InnerText);
                jobIds.Add(id);
                // hash id
                var hashId = Convert.ToInt64(node.Attributes["hash"].InnerText);

                //load job
                ImportedJobWanted wantedJob =
                    await _dbContext.ImportedJobsWanted.FirstOrDefaultAsync(j => j.JobId == id);

                ImportedJobWanted hashMatch = null;

                if (wantedJob == null)
                {
                    //matching hash
                    hashMatch =
                        await _dbContext.ImportedJobsWanted.FirstOrDefaultAsync(
                            j => j.JobId != id && j.HashId == hashId);
                    if (hashMatch != null)
                    {
                        // When a job is found with the same hash, we end up keeping the old JobId and updating the
                        // old record. This is better than creating a new job id because it doesn't inflate the
                        // number of external jobs in the reports.
                        node.Attributes["id"].InnerText = hashMatch.JobId.ToString();
                        wantedJob = hashMatch;
                    }
                }

                // keep a record of what the "id" was before we changed it
                var newAttribute = node.OwnerDocument.CreateAttribute("gartnerid");
                newAttribute.Value = id.ToString();
                node.Attributes.Append(newAttribute);

                //if the job exists, update it
                if (wantedJob != null)
                {
                    if (wantedJob.ApiDate !=
                        Convert.ToDateTime(node.SelectSingleNode("dates").Attributes["refreshed"].InnerText))
                    {
                        //UPDATE
                        wantedJob.ApiDate =
                            Convert.ToDateTime(node.SelectSingleNode("dates").Attributes["refreshed"].InnerText);
                        wantedJob.JobPostEnglish = node.OuterXml;
                        wantedJob.DateLastImported = DateTime.Now;
                        wantedJob.ReIndexNeeded = true;

                        //DB update
                        _dbContext.ImportedJobsWanted.Update(wantedJob);
                        await _dbContext.SaveChangesAsync();

                        Console.Write(hashMatch != null ? "H" : "U");
                    }
                    else
                    {
                        //SKIP
                        Console.Write("S");
                    }
                }
                else
                {
                    //save to db
                    DateTime now = DateTime.Now;
                    JobId existingJobId = _dbContext.JobIds.FirstOrDefault(j => j.Id == id);

                    var wanted = new ImportedJobWanted
                    {
                        JobId = id,
                        JobPostEnglish = node.OuterXml,
                        DateFirstImported = existingJobId?.DateFirstImported ?? now,
                        DateLastImported = now,
                        ApiDate = Convert.ToDateTime(node.SelectSingleNode("dates").Attributes["refreshed"]
                            .InnerText),
                        ReIndexNeeded = true,
                        HashId = hashId
                    };

                    // add the job to the JobIds table
                    if (existingJobId == null)
                    {
                        var jobId = new JobId
                        {
                            Id = id,
                            DateFirstImported = now,
                            JobSourceId = JobSource.Wanted
                        };
                        await _dbContext.JobIds.AddAsync(jobId);
                        await _dbContext.SaveChangesAsync();
                    }

                    await _dbContext.ImportedJobsWanted.AddAsync(wanted);
                    await _dbContext.SaveChangesAsync();
                    Console.Write("I");
                }
            }

            return jobIds;
        }

        /// <summary>
        ///     Purge jobs older than 30 days
        /// </summary>
        public async Task PurgeJobs()
        {
            List<ImportedJobWanted> jobsToPurge;

            if (!DoBulkImport)
            {
                // regular mode:
                //Get jobs older than 30 days
                jobsToPurge = await _dbContext.ImportedJobsWanted
                    .Where(j => DateTime.Now > j.ApiDate.AddDays(_settings.JobExpiryDays + 1)).ToListAsync();
            }
            else
            {
                // bulk mode:
                // Get jobs older than 30 days and jobs not seen in the past 2 days.
                // Limit to 1250 jobs per run (5000 per day), in case something goes wrong
                // with the Wanted API we don't want to accidentally delete all the jobs
                // at once.
                jobsToPurge = await _dbContext.ImportedJobsWanted
                    .Where(
                        j => DateTime.Now > j.ApiDate.AddDays(_settings.JobExpiryDays + 1) ||
                             j.DateLastSeen < DateTime.Now.AddDays(-1 * _settings.DaysToKeepJobSinceLastSeen)
                    )
                    .OrderBy(j => j.ApiDate)
                    .Take(_settings.MaximumJobsToExpireAtOnce)
                    .ToListAsync();
            }

            _logger.Information($"{jobsToPurge.Count} jobs found to purge from the ImportedJobsWanted table.");

            var counter = 0;

            //move jobs to expired jobs
            foreach (ImportedJobWanted job in jobsToPurge)
            {
                //check that this job exists in the database 
                //if the job is not in the database, it means this job was a Gov of Canada job
                //and the job was not index, hence we do not try to remove the job 
                ImportedJobWanted jobEntry =
                    await _dbContext.ImportedJobsWanted.FirstOrDefaultAsync(j => j.JobId == job.JobId);

                if (jobEntry != null)
                {
                    //ensure there isn't already a job with this ID in the ExpiredJob table
                    ExpiredJob expiredJob = await _dbContext.ExpiredJobs.FirstOrDefaultAsync(j => j.JobId == job.JobId);

                    if (expiredJob == null)
                    {
                        //Create expire job object
                        expiredJob = new ExpiredJob
                        {
                            JobId = job.JobId,
                            DateRemoved = DateTime.Now,
                            RemovedFromElasticsearch = false
                        };

                        //add to database
                        await _dbContext.ExpiredJobs.AddAsync(expiredJob);

                        //remove job from wanted list
                        _dbContext.ImportedJobsWanted.Remove(job);
                        Console.Write("D");

                        counter++;

                        if (counter >= 500)
                        {
                            //Update database
                            await _dbContext.SaveChangesAsync();

                            counter = 0;

                            Console.Write("[500]");
                        }
                    }
                    else
                    {
                        // make sure the expired table has the latest data
                        expiredJob.DateRemoved = DateTime.Now;

                        //update to process this deleted job again
                        expiredJob.RemovedFromElasticsearch = false;
                        _dbContext.ExpiredJobs.Update(expiredJob);

                        //Remove job from ImportedJobsWanted table
                        _dbContext.ImportedJobsWanted.Remove(job);

                        Console.Write("[DUP]");
                    }
                }
            }

            //Update database
            await _dbContext.SaveChangesAsync();
            Console.WriteLine();
        }

        /// <summary>
        ///     Get total records for specific settings
        /// </summary>
        public async Task<int> GetTotalRecords()
        {
            //build URL
            string url = BuildApiUrl();

            //make web request
            XmlDocument xmlResult = await GetWebResponse(url);

            //Get the root element
            XmlElement root = xmlResult.DocumentElement;

            //read total results to determinate how many calls we need to make
            //number of records
            var numberOfRecords = Convert.ToInt32(root.Attributes["numfound"].InnerText);

            return numberOfRecords;
        }

        /// <summary>
        ///     Build API URL for the Wanted service
        /// </summary>
        private string BuildApiUrl()
        {
            return
                $"{_settings.ApiUrl}&pageindex={_settings.PageIndex}&pagesize={_settings.PageSize}&passkey={_settings.PassKey}&date={_settings.DateFrom}-{_settings.DateEnd}";
        }

        /// <summary>
        ///     Get XML data from URL response
        /// </summary>
        private async Task<XmlDocument> GetWebResponse(string url)
        {
            var xmlData = new XmlDocument();
            var responseFromServer = string.Empty;

            try
            {
                var handler = new HttpClientHandler();

                if (_proxySettings.UseProxy)
                {
                    handler.Proxy = new WebProxy(_proxySettings.ProxyHost, _proxySettings.ProxyPort)
                    {
                        BypassProxyOnLocal = true
                    };
                }

                //ignoring SSL certificate errors
                if (_proxySettings.IgnoreSslErrors)
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => true;
                }

                //Create new web request to URL
                using (var httpClient = new HttpClient(handler))
                {
                    //Read the web response from URL
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    //Save response
                    responseFromServer = await response.Content.ReadAsStringAsync();
                }

                //load web request to xml
                xmlData.LoadXml(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                _logger.Error("URL: " + url);
                _logger.Error("STACKTRACE: " + ex);
                _logger.Error("RESPONSE: " + responseFromServer);
                xmlData = null;
            }

            //return xml
            return xmlData;
        }

        public void MigrateDb()
        {
            new MigrationService(_dbContext, _logger).RunDbMigrations();
        }

        private void SetJobsSeen(List<long> jobIds)
        {
            if (jobIds.Any())
            {
                //Create SQL Connection object
                using (var cn = new SqlConnection(_connectionString))
                {
                    //open connection to SQL
                    cn.Open();

                    using (var cmd = new SqlCommand(
                        $@"UPDATE ImportedJobsWanted 
                                  SET DateLastSeen = GetDate() 
                                  WHERE JobId IN ({string.Join(',', jobIds)})"
                        , cn
                    ))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}