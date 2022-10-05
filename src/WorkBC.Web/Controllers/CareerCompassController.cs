using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Services;

namespace WorkBC.Web.Controllers

{
    /// <summary>
    ///     API for the getting job posting for the WorkBC Career Compass "Explore BC Regions" Map.
    /// </summary>
    [Route("api/career-compass")]
    [ApiController]
    public class CareerCompassController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _context;
        private readonly IGeocodingService _geocodingService;

        public CareerCompassController(JobBoardContext context, IConfiguration configuration,
            IGeocodingService geocodingService)
        {
            _context = context;
            _configuration = configuration;
            _geocodingService = geocodingService;
        }

        [HttpGet("ExploreBCRegions/{regionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopJobsByRegion(int regionId)
        {
            string regionName = (await _context.Regions.FirstOrDefaultAsync(r => r.Id == regionId))?.Name;

            if (regionName == null)
            {
                return BadRequest("Region not found");
            }

            // Create a search filter to get the newest 50 jobs for a specific Region 
            var filters = new JobSearchFilters
            {
                Page = 1,
                PageSize = 50,
                SortOrder = 1, // Posted date newest first
                SearchJobSource = "1", // Federal jobs only
                SearchLocations = new List<LocationField>
                {
                    new LocationField
                    {
                        Region = regionName,
                        City = "",
                        Postal = ""
                    }
                }
            };

            // run the query
            Source[] jobs = await RunElasticQuery(filters);

            if (!jobs.Any())
            {
                return new JsonResult(new object[] { });
            }

            List<CareerCompassJobMapLocation> locations =
                JobSearchQuery
                    .GetMapPins<CareerCompassJobMapLocation>(jobs, BuildContent);

            // return the result as an array to match what was being done previously in the Kentico control
            return new JsonResult(new[] { locations });
        }

        private async Task<Source[]> RunElasticQuery(JobSearchFilters filters)
        {
            //Search object that we will use to search Elastic Search
            var query = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results from Elastic search
            ElasticSearchResponse results = await query.GetSearchResults();

            return results?.Hits?.HitsHits != null
                ? results.Hits.HitsHits.Select(hit => hit.Source).ToArray()
                : new Source[] { };
        }

        private string BuildContent(Source job)
        {
            string jbSearchUrl = _configuration["AppSettings:JbSearchUrl"];
            string jobPosting = $"{jbSearchUrl}#/job-details/{job.JobId}";

            // don't indent this code!!  It's a verbatim string
            return $@"<div class='infowindow'>
 <p class='jobtitle'>{job.Title}</p>
 <p class='location'>{job.City}</p>
 <p class='more'><a href='{jobPosting}' target='_blank'>Find out more<span></span></a></p>
 <div class='sharejob'>
  <p>Share this job</p>
  <ul class='share-links'>
   <li class='share-twitter'><a target='_blank' title='Follow Us on Twitter @WorkBC' href='{ShareToTwitterRelativeUrl(jobPosting)}'><span></span></a></li>
   <li class='share-facebook'><a target='_blank' title='Like Us on Facebook' href='{ShareToFaceBookRelativeUrl(jobPosting)}'><span></span></a></li>
  </ul>
 </div>
</div>";
        }

        private static string ShareToFaceBookRelativeUrl(string url)
        {
            string sharedUrl = HttpUtility.UrlEncode(url);
            return sharedUrl == ""
                ? ""
                : $"https://www.facebook.com/sharer/sharer.php?u={sharedUrl}";
        }

        private static string ShareToTwitterRelativeUrl(string url)
        {
            string sharedUrl = HttpUtility.UrlEncode(url);
            return sharedUrl == ""
                ? ""
                : $"https://twitter.com/intent/tweet?url={sharedUrl}";
        }
    }
}