using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;
using Location = WorkBC.ElasticSearch.Models.JobAttributes.Location;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class GoogleMapsInfoWindowQuery
    {
        public string ToJson(Location location)
        {
            string json = ResourceFileHelper.ReadFile("googlemapsinfowindow.json");

            var jobContent = $"{{ \"term\": {{ \"Location.Lat.keyword\": \"{location.Lat}\" }} }},{{ \"term\": {{ \"Location.Lon.keyword\": \"{location.Lon}\" }} }}";
            json = json.Replace(@"""should""", @"""must""");

            //update Elastic Search query with newly created list of job IDs
            json = json.Replace("##JOBROWS##", jobContent);

            return json;
        }

        public string ToJson(string jobIds)
        {
            string json = ResourceFileHelper.ReadFile("googlemapsinfowindow.json");

            //one record for a job ID
            var jobRow = "{ \"term\": { \"JobId.keyword\": \"##JOBID##\" } },";
            var jobContent = "";

            //loop through comma seperated string with job id's
            foreach (string jobId in jobIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                jobContent += jobRow.Replace("##JOBID##", jobId);
            }

            //remove last comma
            jobContent = jobContent.Substring(0, jobContent.Length - 1);

            //update Elastic Search query with newly created list of job IDs
            json = json.Replace("##JOBROWS##", jobContent);

            return json;
        }

        public async Task<ElasticSearchResponse> GetResultsFromElasticSearch(IConfiguration configuration,
            string jobIds, Location location = null)
        {
            string server = configuration["ConnectionStrings:ElasticSearchServer"];
            string index = configuration["IndexSettings:DefaultIndex"];
            string docType = General.ElasticDocType;

            string url = $"{server}/{index}/{docType}/_search";

            string jsonQuery = location == null 
                ? ToJson(jobIds) 
                : ToJson(location);

            string jsonResponse = await new ElasticHttpHelper(configuration).QueryElasticSearch(jsonQuery, url);

            //set JSON to variable
            return JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResponse);
        }
    }
}