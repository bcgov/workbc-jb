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
using WorkBC.Shared.Constants;
using WorkBC.Shared.Settings;
using WorkBC.Shared.Utilities;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Indexers.Federal.Services
{
    public class FederalIndexService
    {
        private readonly CommandLineOptions _options;
        private readonly IConfiguration _configuration;
        private readonly ConnectionSettings _connectionSettings;
        private readonly JobBoardContext _dbContext;
        private readonly IndexSettings _indexSettings;
        private readonly ILogger _logger;
        private readonly List<long> _lstIds = new List<long>();
        private readonly XmlParsingServiceFederal _xmlService;
        private readonly IndexCheckerService _indexChecker;

        //Constructor
        public FederalIndexService(IConfiguration configuration, CommandLineOptions options, ILogger logger)
        {
            //Get settings
            _indexSettings = new IndexSettings();
            _connectionSettings = new ConnectionSettings();
            _options = options ?? new CommandLineOptions();
            _configuration = configuration;
            _indexChecker = new IndexCheckerService(logger, configuration);

            configuration.GetSection("IndexSettings").Bind(_indexSettings);
            configuration.GetSection("ConnectionStrings").Bind(_connectionSettings);

            _logger = logger;

            _dbContext = new JobBoardContext(_connectionSettings.DefaultConnection);

            if (!((options?.Migrate ?? false) || (options?.SkipReIndex ?? false) || (options?.Debug ?? false)))
            {
                _xmlService = new XmlParsingServiceFederal(configuration);
            }
        }

        /// <summary>
        ///     Process records for Elastic Search
        /// </summary>
        public async Task MainTask()
        {
            // Run sql migrations - this must be run alone
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

            if (!_options.SkipReIndex)
            {
                //get a list of all records
                List<ImportedJobFederal> jobs =
                    await _dbContext.ImportedJobsFederal.Where(j => j.ReIndexNeeded).ToListAsync();

                //loop through each one
                foreach (ImportedJobFederal job in jobs)
                {
                    //get job object - english
                    ElasticSearchJob esjEnglish =
                        _xmlService.ConvertToElasticJob(job.JobPostEnglish);

                    //get job object - french
                    ElasticSearchJob esjFrench =
                        _xmlService.ConvertToElasticJob(job.JobPostFrench, true);

                    //process model to JSON object
                    string jsonEnglish = JsonConvert.SerializeObject(esjEnglish, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    string jsonFrench = JsonConvert.SerializeObject(esjFrench, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    if (esjEnglish != null)
                    {
                        //POST English JSON to ElasticSearch
                        bool englishOk = await CallCreateJob(General.EnglishIndex, esjEnglish.JobId, jsonEnglish);

                        if (esjFrench != null)
                        {
                            //POST French JSON to ElasticSearch
                            if (englishOk && await CallCreateJob(General.FrenchIndex, esjEnglish.JobId, jsonFrench))
                            {
                                //add job to list of ID's to update "ReIndexNeeded" to "false"
                                _lstIds.Add(job.JobId);
                            }
                        }
                    }

                    //If we processed 100 records, update the database
                    if (_lstIds.Count > 100)
                    {
                        UpdateIds();
                        Console.Write("DB_UPDATED_100-");
                    }
                }

                //Update left over Ids that is less than the threshold (100)
                UpdateIds();
                Console.WriteLine();
            }
        }

        /// <summary>
        ///     Wrapper around CreateJob so it returns true or false based on the response
        /// </summary>
        private async Task<bool> CallCreateJob(string index, string jobId, string json)
        {
            string response = await CreateJob(index, jobId, json);
            if (index == General.EnglishIndex)
            {
                Console.Write($"{response}-E-");
            }

            if (index == General.FrenchIndex)
            {
                Console.Write($"{response}-F-");
            }

            return response.ToLower().Equals("ok") || response.ToLower().Equals("created");
        }

        /// <summary>
        ///     Wrapper for PostToElasticSearch.  Detects a System.Net.WebException,
        ///     waits 15 seconds, and tries again.  I think ElasticSearch stops accepting requests
        ///     when it is doing Java garbage collection.
        /// </summary>
        private async Task<string> CreateJob(string index, string jobId, string json)
        {
            try
            {
                if (json == null || json == "null")
                {
                    // We found a few jobs that didn't have a french translation. Just ignore it because we
                    // mostly care about the English for WorkBC.
                    if (index == General.FrenchIndex)
                    {
                        Console.Write($"[Missing French XML {jobId}]-");
                        return "OK";
                    }

                    Console.Write($"[Missing Or Corrupt English XML {jobId}]-");
                    return "Error";
                }

                return await PostToElasticSearch(json, jobId, "PUT", index);
            }
            catch (WebException)
            {
                Console.Write("WAIT_15_SECONDS-");
                await Task.Delay(15000);
                return await PostToElasticSearch(json, jobId, "PUT", index);
            }
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

                    var sql = @"UPDATE ""ImportedJobsWanted"" SET ""ReIndexNeeded"" = false WHERE ""JobId"" = ANY(@ids)";

                    //Create command
                    using (var cmd = new NpgsqlCommand(sql, cn))
                    {
                        //Add parameter
                        cmd.Parameters.AddWithValue("@ids", _lstIds.ToArray());

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
        ///     Delete a job from the Elastic search index
        /// </summary>
        /// <returns>Response from Elastic Search</returns>
        public async Task PurgeJobs()
        {
            //get all jobs that we still need to process.
            List<ExpiredJob> jobsToPurge = await _dbContext.ExpiredJobs
                .Where(j => !j.RemovedFromElasticsearch).ToListAsync();
            var counter = 0;

            //loop through jobs
            foreach (ExpiredJob job in jobsToPurge)
            {
                //Remove from Elastic search
                await PostToElasticSearch(string.Empty, job.JobId.ToString(), "DELETE", _configuration["IndexSetting:DefaultIndex"]);
                await PostToElasticSearch(string.Empty, job.JobId.ToString(), "DELETE", General.FrenchIndex);

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

            //Use another approach to get a list of jobs that we still need to remove
            List<long> sqlJobIds = await _dbContext.ImportedJobsFederal.Select(j => j.JobId).ToListAsync();

            // ENGLISH
            List<long> englishElasticJobIds = await _indexChecker.GetIndexedEnglishFederalJobIds();
            List<long> moreEnglishJobsToPurge = englishElasticJobIds.Where(j => !sqlJobIds.Contains(j)).ToList();

            //loop through jobs
            foreach (long jobId in moreEnglishJobsToPurge)
            {
                //Remove from Elastic search
                await PostToElasticSearch(string.Empty, jobId.ToString(), "DELETE", General.EnglishIndex);

                //Deleted
                Console.Write("[D2E]");
            }

            // FRENCH
            List<long> frenchElasticJobIds = await _indexChecker.GetIndexedFrenchFederalJobIds();
            List<long> moreFrenchJobsToPurge = frenchElasticJobIds.Where(j => !sqlJobIds.Contains(j)).ToList();

            //loop through jobs
            foreach (long jobId in moreFrenchJobsToPurge)
            {
                //Remove from Elastic search
                await PostToElasticSearch(string.Empty, jobId.ToString(), "DELETE", General.FrenchIndex);

                //Deleted
                Console.Write("[D2F]");
            }

            if (jobsToPurge.Any() || moreEnglishJobsToPurge.Any() || moreFrenchJobsToPurge.Any())
            {
                Console.WriteLine();
            }
        }

        /// <summary>
        ///     POST data to ElasticSearch
        /// </summary>
        private async Task<string> PostToElasticSearch(string json, string id, string action, string index)
        {
            string elasticDocType = _indexSettings.ElasticDocType;
            string url = $"{_connectionSettings.ElasticSearchServer}/{index}/{elasticDocType}/{id}";

            return await new ElasticHttpHelper(_indexSettings.ElasticUser, _indexSettings.ElasticPassword)
                .PostToElasticSearch(json, url, action);
        }

        /// <summary>
        ///     Gets information for figuring out why the Job Board Admin doesn't match Elasticsearch
        /// </summary>
        public async Task Debug()
        {
            List<long> elasticJobIds =
                await new IndexCheckerService(_logger, _configuration).GetActiveEnglishFederalJobIds();
            List<long> sqlJobIds = await _dbContext.Jobs
                .Where(j => j.JobSourceId == JobSource.Federal && j.ExpireDate > DateTime.Now && j.IsActive)
                .Select(j => j.JobId)
                .ToListAsync();

            Console.WriteLine($"Active jobs found in Elasticsearch: {elasticJobIds.Count}");
            Console.WriteLine($"Active jobs found in the Jobs table: {sqlJobIds.Count}");
            Console.WriteLine();
            Console.WriteLine("JobIds found in Elasticsearch but not in the Jobs table:");
            foreach (long jobId in elasticJobIds.Except(sqlJobIds))
            {
                Console.WriteLine(jobId);
            }

            Console.WriteLine();
            Console.WriteLine("JobIds found in the Jobs table but not in Elasticsearch:");
            foreach (long jobId in sqlJobIds.Except(elasticJobIds))
            {
                Console.WriteLine(jobId);
            }
        }
    }
}