using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Areas.Reports.Extensions;
using WorkBC.Admin.Areas.Reports.Models;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Services;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class JobsByIndustryController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public JobsByIndustryController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new JobsByIndustryViewModel
            {
                RegionId = 0,
                StartDate = startDate,
                EndDate = endDate,
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("JobsByIndustry", model);
        }

        public async Task<IActionResult> Results(JobsByIndustryViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

            if (!ModelState.IsValid)
            {
                return View("JobsByIndustry", model);
            }

            model.Results = await _dapperContext.JobsByIndustry.RunAsync(
                model.StartDate ?? DateTime.MinValue,
                model.EndDate ?? DateTime.MaxValue,
                model.RegionId);

            return View("JobsByIndustry", model);
        }
    }
}