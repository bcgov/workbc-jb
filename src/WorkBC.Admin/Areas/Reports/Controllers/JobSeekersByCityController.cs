using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Extensions;
using WorkBC.Admin.Areas.Reports.Models;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Services;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class JobSeekersByCityController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public JobSeekersByCityController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new JobSeekersByCityViewModel
            {
                RegionId = 0,
                ReportType = "top20",
                StartDate = startDate,
                EndDate = endDate,
                DateTypeToggle = "any",
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("JobSeekersByCity", model);
        }

        public async Task<IActionResult> Results(JobSeekersByCityViewModel model)
        {
            if (model.DateTypeToggle == "range")
            {
                ModelState.ValidateDateRangePicker(model);
            }

            if (model.ReportType == "region")
            {
                ModelState.ValidateRegion(model.RegionId);
            }
            else
            {
                model.RegionId = 0;
            }

            if (!ModelState.IsValid)
            {
                return View("JobSeekersByCity", model);
            }

            IList<JobSeekersByCityResult> results = await _dapperContext.JobSeekersByCity.RunAsync(
                model.StartDate ?? DateTime.MinValue,
                model.EndDate ?? DateTime.MaxValue,
                model.RegionId ?? 0,
                model.DateTypeToggle);

            if (model.ReportType == "top20")
            {
                results = results.Where(r => r.Region != "N/A").Take(20).ToList();
            }

            model.Results = results;

            return View("JobSeekersByCity", model);
        }
    }
}