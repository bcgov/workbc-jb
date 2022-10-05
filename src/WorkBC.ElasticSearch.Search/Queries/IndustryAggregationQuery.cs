using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class IndustryAggregationQuery
    {
        public string ToJson()
        {
            string json = ResourceFileHelper.ReadFile("industry_aggregation.json");

            //one record for a job ID

            return json;
        }

        public async Task<dynamic> GetResultsFromElasticSearch(IConfiguration configuration)
        {
            string server = configuration["ConnectionStrings:ElasticSearchServer"];
            string index = configuration["IndexSettings:DefaultIndex"];
            string docType = General.ElasticDocType;

            string url = $"{server}/{index}/{docType}/_search";

            string jsonQuery = ToJson();

            string jsonResponse = await new ElasticHttpHelper(configuration).QueryElasticSearch(jsonQuery, url);

            //set JSON to variable
            return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
        }
    }
}