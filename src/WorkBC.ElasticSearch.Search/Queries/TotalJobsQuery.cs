using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class TotalJobsQuery
    {
        private readonly IConfiguration _configuration;

        public TotalJobsQuery(IConfiguration configuration)
        {
            //Set default values
            _configuration = configuration;
        }

        /// <summary>
        ///     Get results by querying Elastic Search
        /// </summary>
        public async Task<ElasticSearchResponse> GetTotalJobs(string language = "")
        {
            string index = language != "fr" 
                ? _configuration["IndexSettings:DefaultIndex"] 
                : General.FrenchIndex;

            // Create a request to the elastic search server 
            var server = _configuration.GetValue<string>("ConnectionStrings:ElasticSearchServer");

            string url = $"{server}/{index}/_count";
            string json = ResourceFileHelper.ReadFile("total_jobs.json");

            string jsonResponse = await new ElasticHttpHelper(_configuration).QueryElasticSearch(json, url);

            return JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResponse);
        }
    }
}