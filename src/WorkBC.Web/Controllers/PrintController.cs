using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Web.Models.Search;
using WorkBC.Web.Services;
using ILogger = Serilog.ILogger;

namespace WorkBC.Web.Controllers
{
    public class PrintController : Controller
    {
        private readonly JobBoardContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ViewCountService _viewCountService;

        public PrintController(JobBoardContext dbContext, IConfiguration configuration, CacheService cacheService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _viewCountService = new ViewCountService(cacheService, _dbContext);
        }

        /// <summary>
        /// Retrieve all the data to print a job
        /// </summary>
        /// <param name="jobId">Federal Job ID</param>
        /// <param name="language">'en' or 'fr'</param>
        [HttpGet]
        public async Task<IActionResult> Job([FromQuery(Name="jobid")] long jobId, [FromQuery(Name="lang")] string language = "en")
        {
            //create new job service
            JobDetailService js = new JobDetailService(_configuration, null);

            //get the job detail
            ElasticSearchResponse results = await js.GetJobDetail(jobId, _dbContext, _viewCountService, language, false);

            //Object that we will return to the client
            SearchResultsModel sr = new SearchResultsModel();

            if (results != null)
            {
                if (results.Hits != null && results.Hits.HitsHits != null)
                {
                    sr.Result = results.Hits.HitsHits.Select(hit => hit.Source).ToArray();
                    sr.Count = results.Hits.Total.Value ?? 0;

                    //Send the jobs to the cache service 
                    //Get the number of views for each job and get the data back
                    sr.Result = await _viewCountService.GetJobViews(sr.Result);

                    //set language headers
                    sr.TextHeaders = new JobDetailsPageLabels();
                    sr.TextHeaders = js.SetTextHeader(language);
                }
                else
                {
                    sr.Result = new Source[0];
                }
            }

            if (sr.Result.Length == 0)
            {
                return BadRequest("Invalid jobid");
            }

            //return to client
            return View(sr);
        }
    }
}