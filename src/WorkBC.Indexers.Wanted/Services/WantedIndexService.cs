using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing;
using WorkBC.ElasticSearch.Indexing.Services;
using WorkBC.ElasticSearch.Indexing.Settings;
using WorkBC.Shared.Settings;
using WorkBC.Shared.Utilities;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Indexers.Wanted.Services
{
    public class WantedIndexService
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionSettings _connectionSettings;
        private readonly JobBoardContext _dbContext;
        private readonly IndexSettings _indexSettings;
        private readonly ILogger _logger;
        private readonly List<long> _lstIds = new List<long>();
        private readonly XmlParsingServiceWanted _xmlService;
        private readonly CommandLineOptions _options;
        private readonly IndexCheckerService _indexChecker;

        public WantedIndexService(IConfiguration configuration, CommandLineOptions options, ILogger logger)
        {
            // get settings
            _indexSettings = new IndexSettings();
            _connectionSettings = new ConnectionSettings();
            _options = options ?? new CommandLineOptions();
            _configuration = configuration;

            configuration.GetSection("IndexSettings").Bind(_indexSettings);
            configuration.GetSection("ConnectionStrings").Bind(_connectionSettings);

            _logger = logger;
            _dbContext = new JobBoardContext(_connectionSettings.DefaultConnection);
            _indexChecker = new IndexCheckerService(_logger, _configuration);

            if (!((options?.Migrate ?? false) || (options?.SkipReIndex ?? false) || (options?.Debug ?? false)))
            {
                _xmlService = new XmlParsingServiceWanted(configuration);
            }
        }

        /// <summary>
        ///     Process records for Elastic Search
        /// </summary>
        public async Task MainTask()
        {
            //Run sql migrations - this must be run alone
            if (_options.Migrate)
            {
                new MigrationService(_dbContext, _logger).RunDbMigrations();
                return;
            }

            // recreate or re-open the index
            if (_options.ReIndex || _options.ReOpen)
            {
                var reIndexingService = new ReIndexingService(_configuration, _logger);
                await reIndexingService.DoMaintenanceTasks(_options);
            }

            //get a list of all records
            List<ImportedJobWanted> jobs =
                await _dbContext.ImportedJobsWanted.Where(j => j.ReIndexNeeded && !j.IsFederalOrWorkBc).ToListAsync();

            if (!_options.SkipReIndex)
            {
                //loop through each one
                foreach (ImportedJobWanted job in jobs)
                {
                    //Get job information
                    ElasticSearchJob esJob = _xmlService.ConvertToElasticJob(job.JobPostEnglish);

                    if (esJob.JobId != null) // jobs from feds have null JobId
                    {
                        //process model to JSON object
                        string json = JsonConvert.SerializeObject(esJob, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                        //POST JSON to ElasticSearch
                        string response = await PostToElasticSearch(json, esJob.JobId, "PUT");
                        if (!response.ToLower().Equals("error"))
                        {
                            Console.Write($"{response}-");

                            //add job to list of ID's to update "ReIndexNeeded" to "false"
                            if (response.ToLower().Equals("ok") || response.ToLower().Equals("created")) {
                                _lstIds.Add(job.JobId);
                            }
                        }
                        else
                        {
                            //break from loop
                            //Problem with elastic search - most likely down.
                            Console.Write("CRITICAL ERROR-");
                            break;
                        }

                        //If we processed 100 records, update the database
                        if (_lstIds.Count > 100)
                        {
                            UpdateIds();
                            Console.Write("DB_UPDATED_100-");
                        }
                    }
                    else
                    {
                        //Job is from feds or WorkBC
                        _lstIds.Add(job.JobId);
                    }
                }

                //Update left over Ids that is less than the threshold (100)
                UpdateIds();
                Console.WriteLine();
            }
        }

        /// <summary>
        ///     Delete a job from the Elastic search index
        /// </summary>
        /// <returns>Response from Elastic Search</returns>
        public async Task PurgeJobs()
        {
            List<ExpiredJob> jobsToPurge =
                await _dbContext.ExpiredJobs.Where(j => !j.RemovedFromElasticsearch).ToListAsync();
            var counter = 0;

            //loop through jobs
            foreach (ExpiredJob job in jobsToPurge)
            {
                //Remove from Elastic search
                await PostToElasticSearch(string.Empty, job.JobId.ToString(), "DELETE");

                //Update indicators
                job.RemovedFromElasticsearch = true;

                _dbContext.ExpiredJobs.Update(job);

                counter++;

                //Deleted
                Console.Write("D");

                if (counter == 100)
                {
                    await _dbContext.SaveChangesAsync();
                    Console.Write("[100]");
                    counter = 0;
                }
            }

            await _dbContext.SaveChangesAsync();

            //use another approach to get a list of jobs that we still need to remove
            List<long> elasticJobsId = await _indexChecker.GetIndexedWantedJobIds();
            List<long> sqlJobIds = await _dbContext.ImportedJobsWanted
                .Where(j => !j.IsFederalOrWorkBc)
                .Select(j => j.JobId)
                .ToListAsync();
            List<long> moreJobsToPurge = elasticJobsId.Except(sqlJobIds).ToList();

            //loop through jobs
            foreach (long jobId in moreJobsToPurge)
            {
                //Remove from Elastic search
                await PostToElasticSearch(string.Empty, jobId.ToString(), "DELETE");

                //Deleted
                Console.Write("[D2]");
            }

            Console.WriteLine();
        }

        /// <summary>
        ///     Update the "ReIndexNeeded" column to "False" for all ID's in the lstIds variable and clear the variable.
        /// </summary>
        public void UpdateIds()
        {
            try
            {
                //Create SQL Connection object
                using (var cn = new NpgsqlConnection(_connectionSettings.DefaultConnection))
                {
                    //open connection to SQL
                    cn.Open();

                    string idList = $";{string.Join(";", _lstIds)};";
                    var sql = @"UPDATE ImportedJobsWanted SET ReIndexNeeded = 0
                                WHERE @IdList LIKE '%;' + Convert(nvarchar(20),JobId) + ';%'";

                    //Create command
                    using (var cmd = new NpgsqlCommand(sql, cn))
                    {
                        //Add parameter
                        cmd.Parameters.AddWithValue("@IdList", idList);

                        //Execute statement
                        cmd.ExecuteNonQuery();
                    }
                }

                //clear list of IDs to update
                _lstIds.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR: Could not update SQL column `ReIndexNeeded`. Reason: " + ex.Message);
            }
        }

        /// <summary>
        ///     POST data to ElasticSearch
        /// </summary>
        private async Task<string> PostToElasticSearch(string json, string id, string action)
        {
            string elasticDocType = _indexSettings.ElasticDocType;
            string server = _connectionSettings.ElasticSearchServer;
            string url = $"{server}/{_indexSettings.DefaultIndex}/{elasticDocType}/{id}";

            try
            {
                return await new ElasticHttpHelper(_indexSettings.ElasticUser, _indexSettings.ElasticPassword)
                    .PostToElasticSearch(json, url, action);
            }
            catch (WebException)
            {
                // Detect a System.Net.WebException, wait 15 seconds, and try again.
                // I suspect that ElasticSearch stops accepting requests when it is doing Java garbage collection.
                Console.Write("WAIT_15_SECONDS-");
                await Task.Delay(15000);
                return await new ElasticHttpHelper(_indexSettings.ElasticUser, _indexSettings.ElasticPassword)
                    .PostToElasticSearch(json, url, action);
            }
        }

        /// <summary>
        ///     Gets information for figuring out why the Job Board Admin doesn't match Elasticsearch
        /// </summary>
        public async Task Debug()
        {
            List<long> elasticJobIds =
                await new IndexCheckerService(_logger, _configuration).GetActiveWantedJobIds();
            List<long> sqlJobIds = await _dbContext.Jobs
                .Where(j => j.JobSourceId == JobSource.Wanted && j.ExpireDate > DateTime.Now && j.IsActive)
                .Select(j => j.JobId)
                .ToListAsync();

            Console.WriteLine($"Active jobs found in Elasticsearch: {elasticJobIds.Count}");
            Console.WriteLine($"Active jobs found in the Jobs table: {sqlJobIds.Count}");
            Console.WriteLine();
            Console.WriteLine("JobIds found in Elasticsearch but not in the Jobs table:");
            foreach (var jobId in elasticJobIds.Except(sqlJobIds))
            {
                Console.WriteLine(jobId);
            }

            Console.WriteLine();
            Console.WriteLine("JobIds found in the Jobs table but not in Elasticsearch:");
            foreach (var jobId in sqlJobIds.Except(elasticJobIds))
            {
                Console.WriteLine(jobId);
            }
        }
    }
}