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
using WorkBC.Shared.Constants;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class JobsByCityController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public JobsByCityController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new JobsByCityViewModel
            {
                RegionId = 0,
                ReportType = "top20",
                JobSourceId = JobSource.All,
                StartDate = startDate,
                EndDate = endDate,
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("JobsByCity", model);
        }

        public async Task<IActionResult> Results(JobsByCityViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

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
                return View("JobsByCity", model);
            }

            IEnumerable<JobsByCityResult> results = await _dapperContext.JobsByCity.RunAsync(
                model.StartDate ?? DateTime.MinValue,
                model.EndDate ?? DateTime.MaxValue,
                model.RegionId ?? 0,
                model.JobSourceId);

            if (model.ReportType == "top10byRegion" || model.ReportType == "top20")
            {
                results = results.Where(
                    x =>
                        x.City != "Multiple Locations" &&
                        x.City != "Unavailable" &&
                        x.City != "Virtual Jobs");
            }

            if (model.ReportType == "top20")
            {
                results = results.Take(20);
            }

            model.Results = results;

            return View("JobsByCity", model);
        }
    }
}