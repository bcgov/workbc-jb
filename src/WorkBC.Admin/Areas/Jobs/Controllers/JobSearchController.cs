using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Services;
using WorkBC.Data;
using System.Linq;
using WorkBC.Admin.Areas.Jobs.Services;
using WorkBC.Admin.Areas.Jobs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Admin.Models;

namespace WorkBC.Admin.Areas.Jobs.Controllers
{
    [Authorize(Roles = MinAccessLevel.Admin)]
    [Area("Jobs")]
    [Route("jobs/[controller]/[action]")]
    public class JobSearchController : AdminControllerBase
    {
        private readonly JobBoardContext _jobBoardContext;
        private readonly JobService _service;
        private readonly IConfiguration _configuration;

        public JobSearchController(JobBoardContext jobBoardContext, IConfiguration configuration)
        {
            _jobBoardContext = jobBoardContext;
            _configuration = configuration;

            _service = new JobService(_jobBoardContext, _configuration);
        }

        public IActionResult Index()
        {
            return View("JobSearch");
        }

        [HttpGet]
        public async Task<JsonResult> JobsSearch(DataTablesModel model)
        {
            // action inside a standard controller
            (IList<JobSearchViewModel> tempResult,
                int filteredResultsCount) = await _service.Search(model);

            var result = new List<JobSearchViewModel>(tempResult.Count);

            foreach (JobSearchViewModel s in tempResult)
            {
                result.Add(s);
            }

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsFiltered = filteredResultsCount,
                data = result
            });
        }
    }
}