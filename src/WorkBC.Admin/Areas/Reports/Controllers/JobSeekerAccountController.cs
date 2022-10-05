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
    public class JobSeekerAccountController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;
        private readonly JobSeekerAccountReportService _jobSeekerAccountReportService;
        private readonly MatrixReportService _matrixReportService;
        private readonly JobBoardContext _jobBoardContext;

        public JobSeekerAccountController(DapperContext dapperContext, JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
            _dapperContext = dapperContext;
            _matrixReportService = new MatrixReportService(jobBoardContext);
            _jobSeekerAccountReportService = new JobSeekerAccountReportService(jobBoardContext, dapperContext);
        }

        public IActionResult Index()
        {
            var model = new JobSeekerAccountViewModel
            {
                ReportType = "ALL",
                RegionId = null
            };

            MatrixReportService.SetDatePickerModelDefaults(model);

            return View("JobSeekerAccount", model);
        }

        public async Task<IActionResult> Results(JobSeekerAccountViewModel model)
        {
            ModelState.ValidateMatrixDateRangeParams(model);

            if (!ModelState.IsValid)
            {
                return View("JobSeekerAccount", model);
            }

            DateTime startDate = model.GetStartDate();
            DateTime endDate = model.GetEndDate();

            // make sure the periods exists
            await _matrixReportService.EnsureWeeklyPeriodsExist(startDate, endDate);

            // make sure the is data generated for the period
            await _jobSeekerAccountReportService.GenerateJobSeekerStats(startDate, endDate);

            int maxPeriod = await _jobSeekerAccountReportService.GetMaxPeriod() ?? int.MaxValue;

            // run the report
            IList<JobSeekerAccountResult> results;

            switch (model.DateTypeToggle)
            {
                case "yearly":
                    results = await _dapperContext.JobSeekerAccount.RunYearlyAsync(model.FiscalYearFrom, model.FiscalYearTo, maxPeriod, model.RegionId);
                    model.TableHeadings = MatrixReportService.GetYearlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "monthly":
                    results = await _dapperContext.JobSeekerAccount.RunMonthlyAsync(startDate, endDate.Date, maxPeriod, model.RegionId);
                    model.TableHeadings = MatrixReportService.GetMonthlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "weekly":
                    results = await _dapperContext.JobSeekerAccount.RunWeeklyAsync(model.WeeklyYear, model.WeeklyMonth, model.RegionId);
                    model.TableHeadings = MatrixReportService.GetWeeklyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                default:
                    results = new List<JobSeekerAccountResult>();
                    model.TableHeadings = new List<HtmlString>();
                    break;
            }

            model.IsPeriodToDate = endDate > DateTime.Now;

            model.PersistentDataTimestamp =
                _jobBoardContext.ReportPersistenceControl.FirstOrDefault(x =>
                    x.TableName == "JobSeekerStats" && x.IsTotalToDate == true)?.DateCalculated;

            model.Users = _jobSeekerAccountReportService.GroupUsers(results);
 
            return View("JobSeekerAccount", model);
        }
    }
}