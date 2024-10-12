using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.ElasticSearch.Indexing.Settings;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Settings;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class ReIndexingService
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly IndexSettings _indexSettings;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ReIndexingService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;

            _logger = logger;

            _connectionSettings = new ConnectionSettings();

            configuration.GetSection("ConnectionStrings").Bind(_connectionSettings);

            _indexSettings = new IndexSettings();
            configuration.GetSection("IndexSettings").Bind(_indexSettings);
        }

        public async Task DoMaintenanceTasks(CommandLineOptions commandLineOptions)
        {
            // Re-Create the index - this should only need to happen when the index fields are changed
            // or xml parsers are modified
            if (commandLineOptions.ReIndex)
            {
                // Drop index, create index again and mark all jobs to be re-indexed
                _logger.Information("Recreating the job indexes");
                ReCreateIndex();
                _logger.Information("done.");
            }

            if (commandLineOptions.ReOpen)
            {
                _logger.Information("Closing and reopening the job indexes to update synonyms");
                await ReCloseAndReOpenIndexes();
                _logger.Information("done.");
            }
        }

        public void ReCreateIndex()
        {
            _logger.Information($"Removing Index(s) {string.Join(',', IndexSettings.Indexes)}");

            //Delete index in Elastic search
            var di = new DeleteIndexService(_configuration);
            di.Delete();

            _logger.Information($"Creating Index(s) {string.Join(',', IndexSettings.Indexes)}");

            //Create index in Elastic search
            string[] indexes = IndexSettings.Indexes;
            foreach (string index in indexes)
            {
                var ci = new CreateIndexService(index, _configuration);
                ci.Create(false);
                _logger.Information("Index created - " + index);
            }

            _logger.Information("Updating jobs to re-index...");

            FlagAllJobsForReIndexing();

            _logger.Information("Done. Indexing will continue with the new index");
        }

        /// <summary>
        ///     Closes and re-opens the index to trigger config changes (like editing the synonym.txt)
        /// </summary>
        public async Task ReCloseAndReOpenIndexes()
        {
            string server = _connectionSettings.ElasticSearchServer;
            const string englishIndex = General.EnglishIndex;
            const string frenchIndex = General.FrenchIndex;

            var elasticHttp = new ElasticHttpHelper(_indexSettings.ElasticUser, _indexSettings.ElasticPassword);

            await elasticHttp.PostToElasticSearch(string.Empty, $"{server}/{englishIndex}/_close");
            await elasticHttp.PostToElasticSearch(string.Empty, $"{server}/{frenchIndex}/_close");
            await elasticHttp.PostToElasticSearch(string.Empty, $"{server}/{englishIndex}/_open");
            await elasticHttp.PostToElasticSearch(string.Empty, $"{server}/{frenchIndex}/_open");
        }

        private void FlagAllJobsForReIndexing()
        {
            //Create SQL Connection object
            using (var cn = new NpgsqlConnection(_connectionSettings.DefaultConnection))
            {
                //open connection to SQL
                cn.Open();

                using (var cmd = new NpgsqlCommand("UPDATE \"ImportedJobsWanted\" SET \"ReIndexNeeded\" = true", cn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("UPDATE \"ImportedJobsFederal\" SET \"ReIndexNeeded\" = true", cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}