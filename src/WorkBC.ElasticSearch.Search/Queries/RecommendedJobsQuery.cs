using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;
using Boost = WorkBC.ElasticSearch.Search.Boosts.RecommendedJobsBoost;

namespace WorkBC.ElasticSearch.Search.Queries
{
    public class RecommendedJobsQuery : PageableJobsQueryBase
    {
        private readonly IConfiguration _configuration;
        private readonly RecommendedJobsFilters _filters;

        public RecommendedJobsQuery(IConfiguration configuration, RecommendedJobsFilters filters) : base(filters)
        {
            //Set default values
            _configuration = configuration;
            _filters = filters;
        }

        /// <summary>
        ///     Build JSON string that will be used to query Elastic Search for Recommended Jobs
        /// </summary>
        public string ToJson(string jsonFileName)
        {
            // read the resource file
            string json = ResourceFileHelper.ReadFile(jsonFileName);
            var shouldList = new List<string>();
            var filterList = new List<string>();
            var cityFilterList = new List<string>();

            // set Sorting and Pagination
            json = SetPagingAndSorting(json);

            // Set 'should' filters in the ElasticSearch query

            // Apprentices
            if (_filters.IsApprentice)
            {
                shouldList.Add(@"{""term"":{""IsApprentice"":{""value"":true,""boost"":" + Boost.Apprentice + "}}}");
            }

            if (_filters.FilterIsApprentice)
            {
                filterList.Add(@"{""term"":{""IsApprentice"":{""value"":true}}}");
            }

            // Indigenous people
            if (_filters.IsIndigenous)
            {
                shouldList.Add(@"{""term"":{""IsAboriginal"":{""value"":true,""boost"":" + Boost.Aboriginal + "}}}");
            }

            if (_filters.FilterIsIndigenous)
            {
                filterList.Add(@"{""term"":{""IsAboriginal"":{""value"":true}}}");
            }

            // Mature workers
            if (_filters.IsMatureWorkers)
            {
                shouldList.Add(@"{""term"":{""IsMatureWorker"":{""value"":true,""boost"":" + Boost.Mature + "}}}");
            }

            if (_filters.FilterIsMatureWorkers)
            {
                filterList.Add(@"{""term"":{""IsMatureWorker"":{""value"":true}}}");
            }

            // Newcomers
            if (_filters.IsNewcomers)
            {
                shouldList.Add(@"{""term"":{""IsNewcomer"":{""value"":true,""boost"":" + Boost.Newcomer + "}}}");
            }

            if (_filters.FilterIsNewcomers)
            {
                filterList.Add(@"{""term"":{""IsNewcomer"":{""value"":true}}}");
            }

            // People with disabilities
            if (_filters.IsPeopleWithDisabilities)
            {
                shouldList.Add(@"{""term"":{""IsDisability"":{""value"":true,""boost"":" + Boost.Disability + "}}}");
            }

            if (_filters.FilterIsPeopleWithDisabilities)
            {
                filterList.Add(@"{""term"":{""IsDisability"":{""value"":true}}}");
            }

            // Students
            if (_filters.IsStudents)
            {
                shouldList.Add(@"{""term"":{""IsStudent"":{""value"":true,""boost"":" + Boost.Student + "}}}");
            }

            if (_filters.FilterIsStudents)
            {
                filterList.Add(@"{""term"":{""IsStudent"":{""value"":true}}}");
            }

            // Veterans
            if (_filters.IsVeterans)
            {
                shouldList.Add(@"{""term"":{""IsVeteran"":{""value"":true,""boost"":" + Boost.Veteran + "}}}");
            }

            if (_filters.FilterIsVeterans)
            {
                filterList.Add(@"{""term"":{""IsVeteran"":{""value"":true}}}");
            }

            // Visible minority
            if (_filters.IsVisibleMinority)
            {
                shouldList.Add(@"{""term"":{""IsVismin"":{""value"":true,""boost"":" + Boost.Minority + "}}}");
            }

            if (_filters.FilterIsVisibleMinority)
            {
                filterList.Add(@"{""term"":{""IsVismin"":{""value"":true}}}");
            }

            // Youth
            if (_filters.IsYouth)
            {
                shouldList.Add(@"{""term"":{""IsYouth"":{""value"":true,""boost"":" + Boost.Youth + "}}}");
            }

            if (_filters.FilterIsYouth)
            {
                filterList.Add(@"{""term"":{""IsYouth"":{""value"":true}}}");
            }

            // City
            if (!string.IsNullOrEmpty(_filters.City))
            {
                shouldList.Add(@"{""terms"":{""City.normalize"":[""" + _filters.City + "\"],\"boost\":" + Boost.City +
                               "}}");
                shouldList.Add("{\"term\":{\"WorkplaceType.Id\":{\"value\":15141,\"boost\": 0}}}");
            }

            if (_filters.FilterJobSeekerCity && !string.IsNullOrEmpty(_filters.City))
            {
                cityFilterList.Add(@"{""terms"":{""City.normalize"":[""" + _filters.City + "\"]}}");
                cityFilterList.Add("{\"term\":{\"WorkplaceType.Id\":{\"value\":15141}}}");
            }

            // Noc Codes
            foreach (KeyValuePair<short, int> item in _filters.NocCodes)
            {
                decimal boost = Boost.NocCodes + Boost.NocCountBonus * Convert.ToDecimal(item.Value);
                int term = item.Key;
                shouldList.Add(@"{""term"":{""Noc"":{""value"":" + term + ",\"boost\":" + boost + "}}}");
            }

            // Noc Codes 2021
            foreach (KeyValuePair<int, int> item in _filters.NocCodes2021)
            {
                decimal boost = Boost.NocCodes2021 + Boost.NocCountBonus * Convert.ToDecimal(item.Value);
                int term = item.Key;
                shouldList.Add(@"{""term"":{""Noc2021"":{""value"":" + term + ",\"boost\":" + boost + "}}}");
            }

            if (_filters.FilterSavedJobNocs && _filters.NocCodes2021.Count > 0)
            {
                string list = string.Join(',', _filters.NocCodes2021.Select(n => "\"" + n.Key + "\""));
                filterList.Add(@"{""terms"":{""Noc2021"":[" + list + "]}}");
            }

            // Employer Names
            foreach (KeyValuePair<string, int> item in _filters.Employers)
            {
                decimal boost = Boost.Employers + Boost.EmployerCountBonus * Convert.ToDecimal(item.Value);
                string term = item.Key;
                shouldList.Add(@"{""term"":{""EmployerName.normalize"":{""value"":""" + term + @""",""boost"":" + boost + "}}}");
            }

            if (_filters.FilterSavedJobEmployers && _filters.Employers.Count > 0)
            {
                string list = string.Join(',', _filters.Employers.Select(n => "\"" + n.Key + "\""));
                filterList.Add(@"{""terms"":{""EmployerName.normalize"":[" + list + "]}}");
            }

            // Titles
            foreach (KeyValuePair<string, int> item in _filters.Titles)
            {
                decimal boost = Boost.Titles + Boost.TitleCountBonus * Convert.ToDecimal(item.Value);
                string term = item.Key;
                shouldList.Add(@"{""term"":{""Title.normalize"":{""value"":""" + term + @""",""boost"":" + boost + "}}}");
            }

            if (_filters.FilterSavedJobTitles && _filters.Titles.Count > 0)
            {
                string list = string.Join(',', _filters.Titles.Select(n => "\"" + n.Key + "\""));
                filterList.Add(@"{""terms"":{""Title.normalize"":[" + list + "]}}");
            }

            // add the should filters to the json file
            string shouldFiltersJson = string.Join(",\n", shouldList);
            json = json.Replace("##SHOULD##", shouldFiltersJson);

            // add the must filters to the json file
            if (filterList.Any())
            {
                string filterJson = string.Join(",\n", filterList);
                json = json.Replace("##FILTER##", "," + filterJson);
            }
            else
            {
                json = json.Replace("##FILTER##", "");
            }

            // add the city filters to the json file
            if (cityFilterList.Any())
            {
                string filterJson = string.Join(",\n", cityFilterList);
                json = json.Replace("##CITYFILTER##", filterJson);
            }
            else
            {
                json = json.Replace("##CITYFILTER##", "");
            }

            // update minimum_should_match
            json = json.Replace("##MINIMUM_SHOULD_MATCH##", Boost.MinimumShouldMatch.ToString());

            // set excluded job id's
            var ignore = string.Empty;
            if (_filters.IgnoreJobIdList.Length > 0)
            {
                ignore = string.Join(',', _filters.IgnoreJobIdList);
            }

            json = json.Replace("##IGNOREJOBS##", ignore);

            // Return query
            return json;
        }

        /// <summary>
        ///     Get results by querying Elastic Search
        /// </summary>
        public async Task<ElasticSearchResponse> GetSearchResults()
        {
            // Create a request to the elastic search server 
            string server = _configuration["ConnectionStrings:ElasticSearchServer"];
            string index = _configuration["IndexSettings:DefaultIndex"];
            string docType = General.ElasticDocType;

            string url = $"{server}/{index}/{docType}/_search";

            // Apply filter logic 
            SetSortFilters();
            SetFilters();

            string json = ToJson("recommendedjobs.json");

            string jsonResult = await new ElasticHttpHelper(_configuration).QueryElasticSearch(json, url);
            return JsonConvert.DeserializeObject<ElasticSearchResponse>(jsonResult);
        }

        /// <summary>
        ///     Set the filter properties based on the values received in the constructor
        /// </summary>
        private void SetFilters()
        {
            RequestedPageSize = _filters.PageSize;
            PageSize = _filters.PageSize;
            PageNumber = _filters.Page;
        }
    }
}