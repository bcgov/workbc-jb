using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class IndexCheckerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public IndexCheckerService(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<long>> GetIndexedEnglishFederalJobIds()
        {
            return await GetIndexedJobIds(false, true, false);
        }

        public async Task<List<long>> GetIndexedFrenchFederalJobIds()
        {
            return await GetIndexedJobIds(true, true, false);
        }

        public async Task<List<long>> GetIndexedWantedJobIds()
        {
            return await GetIndexedJobIds(false, false, false);
        }

        public async Task<List<long>> GetActiveEnglishFederalJobIds()
        {
            return await GetIndexedJobIds(false, true, true);
        }

        public async Task<List<long>> GetActiveFrenchFederalJobIds()
        {
            return await GetIndexedJobIds(true, true, true);
        }

        public async Task<List<long>> GetActiveWantedJobIds()
        {
            return await GetIndexedJobIds(false, false, true);
        }

        /// <summary>
        ///     Scroll query to get the Ids of all the records in Elasticsearch
        /// </summary>
        /// <param name="isFrench"></param>
        /// <param name="isFederal"></param>
        /// <param name="excludeExpired"></param>
        /// <returns>List of Job Ids</returns>
        private async Task<List<long>> GetIndexedJobIds(bool isFrench, bool isFederal, bool excludeExpired)
        {
            var resultList = new List<long>();
            string server = _configuration["ConnectionStrings:ElasticSearchServer"];

            try
            {
                string body1;

                if (!excludeExpired)
                {
                    body1 = @$"{{
                              ""size"": 5000,
                              ""_source"": [""JobId""], 
                              ""query"": {{
                                ""term"": {{
                                  ""IsFederalJob"": {isFederal.ToString().ToLower()}
                                }}
                              }}
                            }}";
                }
                else
                {
                    body1 = @$"{{
                              ""size"": 5000,
                              ""_source"": [""JobId""],
                              ""query"": {{
                                ""bool"": {{
                                  ""must"": [
                                    {{
                                      ""term"": {{
                                        ""IsFederalJob"": {isFederal.ToString().ToLower()}
                                      }}
                                    }}
                                  ],
                                  ""filter"": {{
                                    ""range"": {{
                                      ""ExpireDate"": {{
                                        ""gte"": ""now"",
                                        ""time_zone"": ""America/Vancouver""
                                      }}
                                    }}
                                  }}
                                }}
                              }}
                            }}";
                }

                string index = isFrench
                    ? General.FrenchIndex
                    : General.EnglishIndex;

                var url1 = $"{server}/{index}/_search?scroll=1m";

                string jsonResponse =
                    await new ElasticHttpHelper(_configuration).QueryElasticSearch(body1, url1);

                var result = JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResponse);

                if (result == null || result.Hits.HitsHits.Count == 0)
                {
                    return new List<long>();
                }

                List<long> tempList = result.Hits.HitsHits.Select(r => r.Id ?? 0).ToList();

                var body2 = $@"{{
                              ""scroll"" : ""1m"", 
                              ""scroll_id"" : ""{result.ScrollId}""
                            }}";

                var url2 = $"{server}/_search/scroll";

                while (tempList.Count > 0)
                {
                    resultList.AddRange(tempList);
                    jsonResponse = await new ElasticHttpHelper(_configuration).QueryElasticSearch(body2, url2);
                    result = JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResponse);

                    if (result == null || result.Hits.HitsHits.Count == 0)
                    {
                        tempList = new List<long>();
                    }

                    tempList = result.Hits.HitsHits.Select(r => r.Id ?? 0).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetIndexedJobIds() failed");
                _logger.Error(ex.ToString());
            }

            return resultList;
        }
    }
}