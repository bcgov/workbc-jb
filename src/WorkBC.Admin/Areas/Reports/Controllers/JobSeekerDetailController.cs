using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Extensions;
using WorkBC.Admin.Areas.Reports.Models;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Services;
using WorkBC.Data.Enums;

namespace WorkBC.Admin.Areas.Reports.Controllers
{
    [Authorize(Roles = MinAccessLevel.Reporting)]
    [Area("reports")]
    public class JobSeekerDetailController : AdminControllerBase
    {
        private readonly DapperContext _dapperContext;

        public JobSeekerDetailController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            DateTime startDate = DateTime.Now.AddDays(-29).Date;
            DateTime endDate = DateTime.Now.Date;

            var model = new JobSeekerDetailViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DateTypeToggle = "any",
                DateRangeType = DateRangeHelper2.DefaultDateRangeType
            };

            return View("JobSeekerDetail", model);
        }

        public async Task<IActionResult> Results(JobSeekerDetailViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

            if (!ModelState.IsValid)
            {
                return View("JobSeekerDetail", model);
            }

            model.Results = await _dapperContext.JobSeekerDetail.RunAsync(
                model.StartDate,
                model.EndDate,
                model.DateTypeToggle,
                2000);

            model.Count = await _dapperContext.JobSeekerDetail.CountAsync(
                model.StartDate,
                model.EndDate,
                model.DateTypeToggle);

            return View("JobSeekerDetail", model);
        }

        public async Task<IActionResult> Excel(JobSeekerDetailViewModel model)
        {
            ModelState.ValidateDateRangePicker(model);

            if (!ModelState.IsValid)
            {
                string message = string.Join("\n", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(message);
            }


            IList<JobSeekerDetailResult> results = await _dapperContext.JobSeekerDetail.RunAsync(
                model.StartDate,
                model.EndDate,
                model.DateTypeToggle,
                1000000);

            // Using EPPlus in a noncommercial context according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //Creates a blank workbook. Use the using statement, so the package is disposed when we are done.
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                ExcelWorksheet ws = p.Workbook.Worksheets.Add("Sheet1");

                // Title row
                ws.Cells["A1"].Value = "Job Seeker Detail Report: " + DateTime.Now;
                ws.Cells["A1:L1"].Merge = true;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // Message row
                ws.Cells["A2"].Value = model.GetDateRegisteredHeader().ToExcelParams();
                ws.Cells["A2:L2"].Merge = true;
                ws.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // Headers Row
                ws.Row(3).Style.Font.Bold = true;

                ws.Cells["A3"].Value = "Status";
                ws.Column(1).Width = 14;

                ws.Cells["B3"].Value = "First Name";
                ws.Column(2).Width = 20;

                ws.Cells["C3"].Value = "Last Name";
                ws.Column(3).Width = 20;

                ws.Cells["D3"].Value = "Email";
                ws.Column(4).Width = 30;

                ws.Cells["E3"].Value = "Location";
                ws.Column(5).Width = 44;
                ws.Column(5).Style.WrapText = true;

                ws.Cells["F3"].Value = "Date Registered";
                ws.Column(6).Width = 22;
                ws.Column(6).Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";

                ws.Cells["G3"].Value = "Last Updated Date";
                ws.Column(7).Width = 22;
                ws.Column(7).Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";

                ws.Cells["H3"].Value = "Employment Groups";
                ws.Column(8).Width = 25;

                ws.Cells["I3"].Value = "Job Alerts";
                ws.Column(9).Width = 20;

                ws.Cells["J3"].Value = "Saved Jobs";
                ws.Column(10).Width = 20;

                ws.Cells["K3"].Value = "Saved Career Profiles";
                ws.Column(11).Width = 20;

                ws.Cells["L3"].Value = "Saved Industry Profiles";
                ws.Column(12).Width = 20;

                // wrap text in the Employment Groups column
                ws.Column(8).Style.WrapText = true;

                for (var i = 0; i < results.Count; i++)
                {
                    int row = i + 4;
                    ws.SetValue(row, 1, ((AccountStatus) results[i].AccountStatus).ToString());
                    ws.SetValue(row, 2, results[i].FirstName);
                    ws.SetValue(row, 3, results[i].LastName);
                    ws.SetValue(row, 4, results[i].Email);
                    ws.SetValue(row, 5, results[i].Location);
                    ws.SetValue(row, 6, results[i].DateRegistered);
                    ws.SetValue(row, 7, results[i].LastModified);
                    ws.SetValue(row, 8, results[i].EmploymentGroupsText);
                    ws.SetValue(row, 9, results[i].HasJobAlerts.YesNo());
                    ws.SetValue(row, 10, results[i].HasSavedJobs.YesNo());
                    ws.SetValue(row, 11, results[i].HasSavedCareerProfiles.YesNo());
                    ws.SetValue(row, 12, results[i].HasSavedIndustryProfiles.YesNo());
                }

                string now = $"{DateTime.Now:yyyy-MM-dd hmmss tt}";
                string filename = $"Job Seeker Detail {now}.xlsx";

                Response.Cookies.Append("show-download-spinner", model.Cookie.ToString(), new CookieOptions
                {
                    Secure = false,
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                });
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(await p.GetAsByteArrayAsync(), contentType, filename);
            }
        }
    }
}