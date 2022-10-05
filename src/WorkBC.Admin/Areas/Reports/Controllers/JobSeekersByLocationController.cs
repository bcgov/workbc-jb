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
    public class JobSeekersByLocationController : AdminControllerBase
    {
        private const string FooterTotalLabel = "TOTAL ALL LOCATIONS";
        private readonly DapperContext _dapperContext;
        private readonly JobSeekersByLocationReportService _jobSeekersByLocationReportService;
        private readonly MatrixReportService _matrixReportService;
        private readonly JobBoardContext _jobBoardContext;

        public JobSeekersByLocationController(DapperContext dapperContext, JobBoardContext jobBoardContext)
        {
            _dapperContext = dapperContext;
            _jobBoardContext = jobBoardContext;
            _matrixReportService = new MatrixReportService(jobBoardContext);
            _jobSeekersByLocationReportService = new JobSeekersByLocationReportService(jobBoardContext, dapperContext);
        }

        public IActionResult Index()
        {
            var model = new JobSeekersByLocationViewModel();

            MatrixReportService.SetDatePickerModelDefaults(model);

            return View("JobSeekersByLocation", model);
        }

        public async Task<IActionResult> Results(JobSeekersByLocationViewModel model)
        {
            ModelState.ValidateMatrixDateRangeParams(model);

            if (!ModelState.IsValid)
            {
                return View("JobSeekersByLocation", model);
            }

            DateTime startDate = model.GetStartDate();
            DateTime endDate = model.GetEndDate();

            // make sure the periods exists
            await _matrixReportService.EnsureWeeklyPeriodsExist(startDate, endDate);

            // make sure the is data generated for the period
            await _jobSeekersByLocationReportService.GenerateJobSeekersByLocationData(startDate, endDate);

            // run the report
            IList<JobSeekersByLocationResult> results;

            switch (model.DateTypeToggle)
            {
                case "yearly":
                    results = await _dapperContext.JobSeekersByLocation.RunYearlyAsync(model.FiscalYearFrom, model.FiscalYearTo);
                    model.TableHeadings = MatrixReportService.GetYearlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "monthly":
                    results = await _dapperContext.JobSeekersByLocation.RunMonthlyAsync(startDate, endDate);
                    model.TableHeadings = MatrixReportService.GetMonthlyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                case "weekly":
                    results = await _dapperContext.JobSeekersByLocation.RunWeeklyAsync(model.WeeklyYear, model.WeeklyMonth);
                    model.TableHeadings = MatrixReportService.GetWeeklyTableHeadings(results.ToList<IMatrixResult>());
                    break;
                default:
                    results = new List<JobSeekersByLocationResult>();
                    model.TableHeadings = new List<HtmlString>();
                    break;
            }

            model.PersistentDataTimestamp =
                _jobBoardContext.ReportPersistenceControl.FirstOrDefault(x =>
                    x.TableName == "JobSeekerStats" && x.IsTotalToDate == true)?.DateCalculated;

            if (endDate > DateTime.Now)
            {
                model.IsPeriodToDate = true;
                endDate = model.PersistentDataTimestamp ?? DateTime.Now;
            }

            model.TotalJobSeekerCount = await _dapperContext.JobSeekersByLocation.GetTotalJobSeekerCount(endDate);

            model.Users = _jobSeekersByLocationReportService.GroupUsers(results);
            model.Users.FooterTotalLabel = FooterTotalLabel;

            return View("JobSeekersByLocation", model);
        }
    }
}