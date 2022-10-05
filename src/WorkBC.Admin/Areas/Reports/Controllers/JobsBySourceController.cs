using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Extensions;
using WorkBC.Admin.Areas.Reports.Models;
using WorkBC.Admin.Areas.Reports.Services;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Services;
using WorkBC.Data;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class JobsBySourceController : AdminControllerBase
    {
        private const string FooterTotalLabel = "TOTAL ALL SOURCES";
        private readonly DapperContext _dapperContext;
        private readonly JobsByRegionOrSourceReportService _jobsByRegionOrSourceReportService;
        private readonly MatrixReportService _matrixReportService;

        public JobsBySourceController(DapperContext dapperContext, JobBoardContext jobBoardContext)
        {
            _dapperContext = dapperContext;
            _matrixReportService = new MatrixReportService(jobBoardContext);
            _jobsByRegionOrSourceReportService = new JobsByRegionOrSourceReportService(jobBoardContext, dapperContext);
        }

        public IActionResult Index()
        {
            var model = new JobsBySourceViewModel();

            MatrixReportService.SetDatePickerModelDefaults(model);

            return View("JobsBySource", model);
        }

        public async Task<IActionResult> Results(JobsBySourceViewModel model)
        {
            ModelState.ValidateMatrixDateRangeParams(model);

            if (!ModelState.IsValid)
            {
                return View("JobsBySource", model);
            }

            DateTime startDate = model.GetStartDate();
            DateTime endDate = model.GetEndDate();

            // make sure the periods exists
            await _matrixReportService.EnsureWeeklyPeriodsExist(startDate, endDate);

            // make sure the is data generated for the period
            await _jobsByRegionOrSourceReportService.GenerateJobsByRegionData(startDate, endDate);

            // run the report
            IList<JobStatsResult> results;

            switch (model.DateTypeToggle)
            {
                case "yearly":
                    results = await _dapperContext.JobsBySource.RunYearlyAsync(model.FiscalYearFrom, model.FiscalYearTo);
                    model.TableHeadings = MatrixReportService.GetYearlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "monthly":
                    results = await _dapperContext.JobsBySource.RunMonthlyAsync(startDate, endDate);
                    model.TableHeadings = MatrixReportService.GetMonthlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "weekly":
                    results = await _dapperContext.JobsBySource.RunWeeklyAsync(model.WeeklyYear, model.WeeklyMonth);
                    model.TableHeadings = MatrixReportService.GetWeeklyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                default:
                    results = new List<JobStatsResult>();
                    model.TableHeadings = new List<HtmlString>();
                    break;
            }

            model.Vacancies = _jobsByRegionOrSourceReportService.GroupVacancies(results);
            model.Postings = _jobsByRegionOrSourceReportService.GroupPostings(results);

            model.Vacancies.FooterTotalLabel = FooterTotalLabel;
            model.Postings.FooterTotalLabel = FooterTotalLabel;

            return View("JobsBySource", model);
        }
    }
}