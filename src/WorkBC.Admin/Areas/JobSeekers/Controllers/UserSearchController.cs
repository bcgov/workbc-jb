using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Admin.Areas.JobSeekers.Services;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Models;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.JobSeekers.Controllers
{
    [Authorize(Roles = MinAccessLevel.Admin)]
    [Area("JobSeekers")]
    [Route("jobseekers/[controller]/[action]")]
    public class UserSearchController : AdminControllerBase
    {
        private readonly UserService _service;

        public UserSearchController(JobBoardContext jobBoardContext, UserManager<JobSeeker> userManager, IMapper mapper, IConfiguration configuration)
        {
            _service = new UserService(jobBoardContext, userManager, mapper, configuration);
        }

        public IActionResult Index()
        {
            return View("UserSearch");
        }

        [HttpGet]
        public async Task<JsonResult> JobSeekerSearch(DataTablesModel model)
        {
            // action inside a standard controller
            (IList<JobSeekerSearchViewModel> result,
                int filteredResultsCount,
                int totalResultsCount,
                int totalActive,
                int totalDeleted,
                int totalDeactivated,
                int totalPending) = await _service.Search(model, CurrentAdminUserId);

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.Draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                totalActive,
                totalDeleted,
                totalDeactivated,
                totalWaiting = totalPending,
                data = result
            });
        }
    }
}