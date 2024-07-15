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
using WorkBC.Data.Enums;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class NocCodeSummaryController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public NocCodeSummaryController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new NocCodeSummaryViewModel
            {
                NocCategoryLevel = NocCategoryLevel.All,
                JobSourceId = JobSource.All,
                StartDate = startDate,
                EndDate = endDate,
                ShowZeroes = false,
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("NocCodeSummary", model);
        }

        public async Task<IActionResult> Results(NocCodeSummaryViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

            if (model.NocCategoryLevel == NocCategoryLevel.UnitGroup && string.IsNullOrEmpty(model.UnitGroup))
            {
                ModelState.AddModelError("UnitGroup", "Select an option");
            }

            if (model.NocCategoryLevel == NocCategoryLevel.MinorGroup && string.IsNullOrEmpty(model.MinorGroup))
            {
                ModelState.AddModelError("MinorGroup", "Select an option");
            }

            if (model.NocCategoryLevel == NocCategoryLevel.MajorGroup && string.IsNullOrEmpty(model.MajorGroup))
            {
                ModelState.AddModelError("MajorGroup", "Select an option");
            }

            if (model.NocCategoryLevel == NocCategoryLevel.SubMajorGroup && string.IsNullOrEmpty(model.SubMajorGroup))
            {
                ModelState.AddModelError("SubMajorGroup", "Select an option");
            }

            if (model.NocCategoryLevel == NocCategoryLevel.BroadOccupationalCategory && string.IsNullOrEmpty(model.BroadCategory))
            {
                ModelState.AddModelError("BroadCategory", "Select an option");
            }



            if (!ModelState.IsValid)
            {
                return View("NocCodeSummary", model);
            }

            var nocCodeStartsWith = "";
            switch (model.NocCategoryLevel)
            {
                case NocCategoryLevel.BroadOccupationalCategory:
                    nocCodeStartsWith = model.BroadCategory;
                    break;
                case NocCategoryLevel.MajorGroup:
                    nocCodeStartsWith = model.MajorGroup;
                    break;
                case NocCategoryLevel.SubMajorGroup:
                    nocCodeStartsWith = model.SubMajorGroup;
                    break;
                case NocCategoryLevel.MinorGroup:
                    nocCodeStartsWith = model.MinorGroup;
                    break;
                case NocCategoryLevel.UnitGroup:
                    nocCodeStartsWith = model.UnitGroup;
                    break;
            }

            model.Results = (
                    await _dapperContext.JobsByNocCode.RunAsync(
                        model.StartDate ?? DateTime.MinValue,
                        model.EndDate ?? DateTime.MaxValue,
                        nocCodeStartsWith,
                        model.JobSourceId)
                )
                .Where(r => model.NocCategoryLevel == NocCategoryLevel.UnitGroup || model.ShowZeroes || r.Vacancies > 0)
                .ToList();

            return View("NocCodeSummary", model);
        }
    }
}
