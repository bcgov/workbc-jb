using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;
using ILogger = Serilog.ILogger;

namespace WorkBC.Tests.Helpers
{
    public class SearchTestHelpers
    {
        //Properties
        private readonly IConfiguration _configuration;
        private readonly IGeocodingService _geocodingService;

        //Constructor
        public SearchTestHelpers(IGeocodingService geocodingService, IConfiguration configuration)
        {
            _geocodingService = geocodingService;
            _configuration = configuration;
        }

        public async Task<List<Source>> GetJobsById(long jobId)
        {
            //Create an instance with the filters required
            JobSearchQuery esq = new JobSearchQuery(_geocodingService, _configuration, GetFiltersForJobId(jobId));

            //return results
            return await QueryElasticSearch(esq);
        }

        public async Task<List<Source>> GetJobsWithCity(string city)
        {
            //Create an instance with the filters required
            JobSearchQuery esq = new JobSearchQuery(_geocodingService, _configuration, GetFiltersForJobByCity(city));

            //return results
            return await QueryElasticSearch(esq);
        }

        public async Task<List<Source>> GetJobsWithEmploymentGroup(bool isApprentice, bool isIndigenous, bool isMatureWorker, bool isNewcomer, bool isPeopleWithDisabilities, bool isStudents, bool isVeterans, bool isVisibleMinority, bool isYouth)
        {
            //Create an instance with the filters required
            var filters = GetFiltersForJobByEmployerGroup(isApprentice, isIndigenous, isMatureWorker, isNewcomer,
                isPeopleWithDisabilities, isStudents, isVeterans, isVisibleMinority, isYouth);
            var esq = new JobSearchQuery(_geocodingService, _configuration, filters);

            //return results
            return await QueryElasticSearch(esq);
        }

        public async Task<List<Source>> GetJobsByLanguagePreference(bool englishOnly, bool englishAndFrench)
        {
            //Create an instance with the filters required
            JobSearchQuery esq = new JobSearchQuery(_geocodingService, _configuration, GetFiltersForJobByLanguagePreference(englishOnly, englishAndFrench));

            //return results
            return await QueryElasticSearch(esq);
        }

        private async Task<List<Source>> QueryElasticSearch(JobSearchQuery esq)
        {
            //Get search results from Elastic search
            ElasticSearchResponse results = await esq.GetSearchResults();

            //Read results from Elastic
            if (results != null)
            {
                if (results.Hits != null && results.Hits.HitsHits != null)
                {
                    //Return results to test
                    return results.Hits.HitsHits.Select(hit => hit.Source).ToArray().ToList();
                }
            }

            //If no results, return empty result set
            return new List<Source>();
        }

        #region Filter setup

        private JobSearchFilters GetFiltersForJobId(long jobId)
        {
            JobSearchFilters esjsf = new JobSearchFilters()
            {
                Page = 1,
                PageSize = 20,
                Keyword = jobId.ToString(),
                SearchInField = "jobid",
                SearchLocations = new List<LocationField>()
            };

            return esjsf;
        }

        private JobSearchFilters GetFiltersForJobByCity(string city)
        {
            JobSearchFilters esjsf = new JobSearchFilters()
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchLocationDistance = 10,
            };

            LocationField lf = new LocationField()
            {
                City = city,
                Postal = string.Empty,
                Region = string.Empty
            };

            esjsf.SearchLocations.Add(lf);

            return esjsf;
        }

        private JobSearchFilters GetFiltersForJobByLanguagePreference(bool englishOnly, bool englishAndFrench)
        {
            JobSearchFilters esjsf = new JobSearchFilters()
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchLocationDistance = 10,
                SearchIsPostingsInEnglish = englishOnly,
                SearchIsPostingsInEnglishAndFrench = englishAndFrench
            };

            return esjsf;
        }

        private JobSearchFilters GetFiltersForJobByEmployerGroup(bool isApprentice, bool isIndigenous, bool isMatureWorker, bool isNewcomer, bool isPeopleWithDisabilities, bool isStudents, bool isVeterans, bool isVisibleMinority, bool isYouth)
        {
            JobSearchFilters esjsf = new JobSearchFilters()
            {
                Page = 1,
                PageSize = 20,
                SearchLocations = new List<LocationField>(),
                SearchLocationDistance = 10,
                SearchIsApprentice = isApprentice,
                SearchIsIndigenous = isIndigenous,
                SearchIsMatureWorkers = isMatureWorker,
                SearchIsNewcomers = isNewcomer,
                SearchIsPeopleWithDisabilities = isPeopleWithDisabilities,
                SearchIsStudents = isStudents,
                SearchIsVeterans = isVeterans,
                SearchIsVisibleMinority = isVisibleMinority,
                SearchIsYouth = isYouth
            };

            return esjsf;
        }

        #endregion
    }
}
