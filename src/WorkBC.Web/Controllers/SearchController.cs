using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Services;
using WorkBC.Web.Models.Search;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    public class SearchController : JobsControllerBase
    {
        private readonly CacheService _cacheService;
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _dbContext;
        private readonly IGeocodingService _geocodingService;
        private readonly GoogleMapsService _googleMapsService;
        private readonly ILogger<SearchController> _logger;
        private readonly INocSearchService _nocSearchService;
        private readonly ViewCountService _viewCountService;

        public SearchController(JobBoardContext dbContext, IConfiguration configuration,
            IGeocodingService geocodingService, CacheService cacheService, INocSearchService nocSearchService,
            SystemSettingsService systemSettingsService, ILogger<SearchController> logger) : base(systemSettingsService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _googleMapsService = new GoogleMapsService();
            _geocodingService = geocodingService;
            _viewCountService = new ViewCountService(cacheService, _dbContext);
            _nocSearchService = nocSearchService;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        ///     Main search function that would perform a search on the database for jobs
        /// </summary>
        [EnableCors("_API")]
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Route("api/[controller]/[action]/{language}")]
        public async Task<IActionResult> JobSearch([FromBody] JobSearchFilters filters, string language = "")
        {
            string index = language != "fr" 
                ? _configuration["IndexSettings:DefaultIndex"] 
                : General.FrenchIndex;

            //Search object that we will use to search Elastic Search
            var esq = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results from Elastic search
            ElasticSearchResponse results = await esq.GetSearchResults(index: index);

            //Build the object that we will return to the client
            var sr = new SearchResultsModel
            {
                PageNumber = filters.Page,
                PageSize = filters.PageSize
            };

            if (results != null)
            {
                if (results.Hits?.HitsHits != null)
                {
                    sr.Result = results.Hits.HitsHits.Select(hit => hit.Source).ToArray();
                    sr.Count = results.Hits.Total.Value ?? 0;

                    //Send the jobs to the cache service 
                    //Get the number of views for each job and get the data back
                    sr.Result = await _viewCountService.GetJobViews(sr.Result);

                    // set the IsNew bit on new jobs
                    await SetNewJobs(sr.Result);
                }
                else
                {
                    sr.Result = Array.Empty<Source>();
                }
            }

            //convert to JSON and return to the client
            return Ok(sr);
        }

        /// <summary>
        ///     Get the jobs for a specific PIN on the map. Can return multiple jobs.
        /// </summary>
        /// <param name="jobIds">Comma separated list of Job ID's</param>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> GetLocationInformation([FromBody] string jobIds)
        {
            return Ok(await _googleMapsService.GetJobsForLocation(jobIds, _configuration));
        }

        /// <summary>
        ///     Get all longitude and latitude for the jobs in the current search result that will be used to render Google Maps
        /// </summary>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> GetMapData([FromBody] JobSearchFilters filters)
        {
            //Search object that we will use to search Elastic Search
            var esq = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results for google maps
            List<GoogleMapsPinLocation> results = await esq.GetGoogleMapResults();

            //return results to the client
            return Ok(
                new
                {
                    UniqueJobCount = results.Select(r => r.JobId).Distinct().Count(),
                    Results = results
                });
        }

        /// <summary>
        ///     Retrieve all the data that will be displayed on the job-details page
        /// </summary>
        /// <param name="jobId" example="34550350">The Job Id</param>
        /// <param name="language" example="en">The language to return ('en' or 'fr')</param>
        /// <param name="isToggle" example="true">Do not increase view count when the user toggles between English and French</param>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> GetJobDetail(long jobId, string language = "en", bool isToggle = false)
        {
            //create new job service
            var js = new JobDetailService(_configuration, _logger);

            //get the job detail
            ElasticSearchResponse results =
                await js.GetJobDetail(jobId, _dbContext, _viewCountService, language, isToggle);

            //Object that we will return to the client
            var sr = new SearchResultsModel();

            if (results != null)
            {
                if (results.Hits != null && results.Hits.HitsHits != null)
                {
                    sr.Result = results.Hits.HitsHits.Select(hit => hit.Source).ToArray();
                    sr.Count = results.Hits.Total.Value ?? 0;

                    //Send the jobs to the cache service 
                    //Get the number of views for each job and get the data back
                    sr.Result = await _viewCountService.GetJobViews(sr.Result);

                    // flag new jobs
                    await SetNewJobs(sr.Result);

                    //set language headers
                    sr.TextHeaders = new JobDetailsPageLabels();
                    sr.TextHeaders = js.SetTextHeader(language);
                }
                else
                {
                    sr.Result = Array.Empty<Source>();
                }
            }

            //return to client
            return Ok(sr);
        }

        /// <summary>
        ///     NOC Code Autocomplete
        /// </summary>
        /// <param name="startsWith"></param>
        [HttpGet]
        [Route("api/[controller]/[action]/{startsWith?}")]
        public async Task<IActionResult> SearchNocCodes(string startsWith = "")
        {
            if (startsWith == "")
            {
                // return an empty array if no query was specified
                return Ok(new string[] { });
            }

            return Ok(await _nocSearchService.SearchNocCodes(startsWith));
        }

        /// <summary>
        ///     Gets the source XML for a job posting
        /// </summary>
        /// <param name="jobId"></param>
        [EnableCors("_API")]
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public JsonResult GetJobXml(long jobId)
        {
            //try and load the data from the database
            if (jobId.ToString().Length == 8)
            {
                //Federal
                var federalJob = _dbContext.ImportedJobsFederal.FirstOrDefault(job => job.JobId == (int)jobId);

                //If federal, return federal XML
                if (federalJob != null)
                {
                    return new JsonResult(federalJob.JobPostEnglish);
                }
            }
            else
            {
                //Wanted
                ImportedJobWanted wantedJob = _dbContext.ImportedJobsWanted.FirstOrDefault(job => job.JobId == jobId);

                //If wanted, return wanted XML
                if (wantedJob != null)
                {
                    return new JsonResult(wantedJob.JobPostEnglish);
                }
            }

            //If we could not find it anywhere
            return new JsonResult("<xml>XML NOT FOUND</xml>");
        }

        /// <summary>
        ///     Returns the total number of jobs as an integer.
        ///     This is used on the WorkBC Homepage.
        /// </summary>
        /// <param name="language">Which index to search ('en' or 'fr')</param>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Route("api/[controller]/[action]/{language}")]
        public async Task<int> GetTotalJobs(string language = "")
        {
            long? jobs = await GetTotalJobsInternal(language);
            return (int)(jobs ?? 0);
        }

        /// <summary>
        ///     Gets the total number of jobs from ElasticSearch or Redis
        /// </summary>
        private async Task<long?> GetTotalJobsInternal(string language)
        {
            string cacheKey = language != "fr" 
                ? "TotalJobCount" 
                : "TotalFrenchJobCount";
            
            var cacheMinutes = 10;

            long? jobCount = await _cacheService.GetLongAsync(cacheKey);

            if (jobCount != null)
            {
                return jobCount;
            }

            var totalJobQuery = new TotalJobsQuery(_configuration);
            ElasticSearchResponse response = await totalJobQuery.GetTotalJobs(language);
            await _cacheService.SaveLongAsync(cacheKey, response.Count, cacheMinutes * 60);
            return response.Count;
        }
    }
}