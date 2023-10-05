using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;

namespace WorkBC.Notifications.JobAlerts.Services
{
    /// <summary>
    ///     Wrapper around Elastic search, used by the Job Alerts scheduled task
    /// </summary>
    public class JobAlertSearchService
    {
        private readonly IConfiguration _configuration;
        private readonly IGeocodingService _geocodingService;

        public JobAlertSearchService(IConfiguration configuration, JobBoardContext dbContext)
        {
            _configuration = configuration;
            var geoService = new GeocodingService(configuration);
            _geocodingService = new GeocodingCachingService(dbContext, geoService);
        }

        /// <summary>
        ///     Searches for jobs matching the filter and posted within the job alert frequency
        /// </summary>
        public async Task<long> GetJobAlertSearchResultCount(JobAlert jobAlert)
        {
            var filters = JsonConvert.DeserializeObject<JobSearchFilters>(jobAlert.JobSearchFilters);
            filters.PageSize = 0;

            DateTime startDate = GetFilterStartDate(jobAlert.AlertFrequency);

            filters.SearchDateSelection = "3"; // custom date range
            filters.StartDate = new DateField(startDate);
            filters.EndDate = new DateField(DateTime.Now);

            //Search object that we will use to search Elastic Search
            var esq = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results from Elastic search
            ElasticSearchResponse results = await esq.GetSearchResults();

            long result = results?.Hits?.Total?.Value ?? 0;
            return result;
        }

        /// <summary>
        ///     Gets the start date for the filter based on frequency
        /// </summary>
        private static DateTime GetFilterStartDate(JobAlertFrequency frequency)
        {
            // set the dates
            TimeSpan removeTimeSpan;

            switch (frequency)
            {
                case JobAlertFrequency.Daily:
                    removeTimeSpan = new TimeSpan(1, 0, 0, 0);
                    break;
                case JobAlertFrequency.Weekly:
                    removeTimeSpan = new TimeSpan(7, 0, 0, 0);
                    break;
                case JobAlertFrequency.BiWeekly:
                    removeTimeSpan = new TimeSpan(15, 0, 0, 0);
                    break;
                default:
                    removeTimeSpan = new TimeSpan(31, 0, 0, 0);
                    break;
            }

            return DateTime.Now.Subtract(removeTimeSpan).Date;
        }
    }
}