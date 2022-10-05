using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class JobDetailQuery
    {
        private readonly IConfiguration _configuration;

        public JobDetailQuery(IConfiguration configuration)
        {
            //Set default values
            _configuration = configuration;
        }

        /// <summary>
        ///     Get results by querying Elastic Search
        /// </summary>
        public async Task<ElasticSearchResponse> GetSearchResults(long jobId, string language)
        {
            // Create a request to the elastic search server 
            var server = _configuration.GetValue<string>("ConnectionStrings:ElasticSearchServer");

            string url = $"{server}/jobs_{language}/_doc/_search";

            string jsonQuery = ToJson(jobId);

            string jsonResponse = await new ElasticHttpHelper(_configuration).QueryElasticSearch(jsonQuery, url);

            return JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResponse);
        }

        /// <summary>
        ///     Build JSON string that will be used to query Elastic Search
        /// </summary>
        private string ToJson(long jobId)
        {
            string json = ResourceFileHelper.ReadFile("jobdetail.json");
            json = json.Replace("##JOBID##", jobId.ToString());
            return json;
        }
    }
}