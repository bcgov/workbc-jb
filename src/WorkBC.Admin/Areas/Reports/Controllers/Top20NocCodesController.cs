using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Areas.Reports.Data;
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
    public class Top20NocCodesController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public Top20NocCodesController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new Top20NocCodesViewModel
            {
                JobSourceId = JobSource.All,
                StartDate = startDate,
                EndDate = endDate,
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("Top20NocCodes", model);
        }

        public async Task<IActionResult> Results(Top20NocCodesViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

            if (!ModelState.IsValid)
            {
                return View("Top20NocCodes", model);
            }

            model.Results = (await _dapperContext.JobsByNocCode.RunAsync(
                model.StartDate ?? DateTime.MinValue,
                model.EndDate ?? DateTime.MaxValue,
                jobSourceId: model.JobSourceId)).Take(20).Where(j => j.Vacancies > 0).ToList();

            return View("Top20NocCodes", model);
        }
    }
}