using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.ElasticSearch.Models.Results;
using WorkBC.Web.Models.Search;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    public class ShareController : Controller
    {
        private readonly JobBoardContext _dbContext;
        private readonly IConfiguration _configuration;

        public ShareController(JobBoardContext dbContext, IConfiguration configuration, CacheService cacheService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieve all the data to share a job
        /// </summary>
        /// <param name="jobId">Federal Job ID</param>
        [HttpGet]
        public async Task<IActionResult> Job([FromQuery(Name="jobid")] long jobId)
        {
            //create new job service
            JobDetailService js = new JobDetailService(_configuration, null);

            //get the job detail
            ElasticSearchResponse results = await js.GetJobDetail(jobId, _dbContext, null, "en", true);

            //Object that we will return to the client
            SearchResultsModel sr = new SearchResultsModel();

            if (results != null)
            {
                if (results.Hits != null && results.Hits.HitsHits != null)
                {
                    sr.Result = results.Hits.HitsHits.Select(hit => hit.Source).ToArray();
                    sr.Count = results.Hits.Total.Value ?? 0;
                    //set language headers
                    sr.TextHeaders = new JobDetailsPageLabels();
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

            // set some viewbag variables
            ViewBag.IsVirtualJob = sr.Result[0].City.StartsWith("Virtual");

            //return to client
            return View(sr);
        }
    }
}