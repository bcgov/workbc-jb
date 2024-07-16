using System;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkBC.Admin.Areas.Reports.Models;
using WorkBC.Admin.Areas.Reports.Models.Partial;
using WorkBC.Admin.Services;
using WorkBC.Data.Enums;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Admin.Areas.Reports.Extensions
{
    public static class HeaderExtensions
    {
        public static string GetDateImportedHeader(this IDateRangePicker model)
        {
            if (model.StartDate == null || model.EndDate == null)
            {
                return string.Empty;
            }

            return $"<strong>Date first imported</strong>: {model.FormatDateRange()}<br>";
        }

        public static string GetReportingPeriodHeader(this IMatrixDateRangeParams model)
        {
            switch (model.DateTypeToggle)
            {
                case "yearly":
                    if (model.FiscalYearFrom != model.FiscalYearTo)
                    {
                        return
                            $"<strong>Reporting period</strong>: From FY-{model.FiscalYearFromFormatted()} to FY-{model.FiscalYearToFormatted()}<br>";
                    }
                    else
                    {
                        return
                            $"<strong>Reporting period</strong>: FY-{model.FiscalYearFromFormatted()}<br>";
                    }
                case "monthly":
                {
                    var startDate = new DateTime(model.MonthlyStartYear, model.MonthlyStartMonth, 1);
                    var endDate = new DateTime(model.MonthlyEndYear, model.MonthlyEndMonth, 1);
                    return $"<strong>Reporting period</strong>: Monthly from {startDate:MMMM yyyy} to {endDate:MMMM yyyy}<br>";
                }
                case "weekly":
                {
                    var date = new DateTime(model.WeeklyYear, model.WeeklyMonth, 1);
                    return $"<strong>Reporting period</strong>: Weekly for {date:MMMM yyyy}<br>";
                }
                default:
                    return "";
            }
        }

        private static string FormatDateRange(this IDateRangePicker model)
        {
            if (model.StartDate == null || model.EndDate == null)
            {
                return string.Empty;
            }

            return $"{model.StartDate.Value:yyyy-MM-dd} to {model.EndDate.Value:yyyy-MM-dd}";
        }

        public static string GetReportDateHeader()
        {
            return $"<strong>Report date</strong>: {DateTime.Now:yyyy-MM-dd h:mm tt} PST<br>";
        }

        public static string GetReportDateHeader(DateTime date)
        {
            return $"<strong>Report date</strong>: {date:yyyy-MM-dd h:mm tt} PST";
        }

        public static string GetPersistenceRefreshHeader(DateTime date)
        {
            TimeSpan timeSpan = date.AddHours(12).Subtract(DateTime.Now);
            return $"<em><small>(next update {timeSpan.Hours} hrs. {timeSpan.Minutes} min.)</small></em>";
        }

        public static string GetJobSourceHeader(this IJobSourceParams model)
        {
            switch (model.JobSourceId)
            {
                case JobSource.Federal:
                    return "<strong>Job source</strong>: Only National Job Bank Jobs<br>";
                case JobSource.Wanted:
                    return "<strong>Job source</strong>: Only External Jobs<br>";
                default:
                    return "<strong>Job source</strong>: All<br>";
            }
        }

        public static string GetCityReportTypeHeader(this ICityReportTypeParams model, SelectListService selectLists)
        {
            switch (model.ReportType)
            {
                case "region":
                    SelectListItem selectedRegion = selectLists.Regions
                        .FirstOrDefault(r => r.Value == model.RegionId.ToString());
                    return $"All cities in {selectedRegion?.Text}";
                case "top20":
                    return "Top 20 cities";
                case "top10byRegion":
                    return "Top 10 cities in each region";
                default:
                    return "All cities";
            }
        }

        public static string GetDateRegisteredHeader(this IJobSeekerDateRangePicker model)
        {
            if (model.DateTypeToggle == "any")
            {
                return "<strong>Date registered</strong>: All dates<br>";
            }

            if (model.StartDate == null || model.EndDate == null)
            {
                return string.Empty;
            }

            return $"<strong>Date registered</strong>: {model.FormatDateRange()}<br>";
        }

        public static string GetNocCategoryHeader(this NocCodeSummaryViewModel model, SelectListService selectLists)
        {
            switch (model.NocCategoryLevel)
            {
                case NocCategoryLevel.BroadOccupationalCategory:
                {
                    SelectListItem selectedCategory = selectLists.BroadOccupationalCategories
                        .FirstOrDefault(r => r.Value == model.BroadCategory);
                    return $"<strong>Broad category</strong>: {selectedCategory?.Text}<br>";
                }
                case NocCategoryLevel.MajorGroup:
                {
                    SelectListItem selectedCategory = selectLists.MajorGroups
                        .FirstOrDefault(r => r.Value == model.MajorGroup);
                    return $"<strong>Major group</strong>: {selectedCategory?.Text}<br>";
                }
                case NocCategoryLevel.SubMajorGroup:
                    {
                        SelectListItem selectedCategory = selectLists.SubMajorGroups
                            .FirstOrDefault(r => r.Value == model.SubMajorGroup);
                        return $"<strong>Sub Major group</strong>: {selectedCategory?.Text}<br>";
                    }
                case NocCategoryLevel.MinorGroup:
                {
                    SelectListItem selectedCategory = selectLists.MinorGroups
                        .FirstOrDefault(r => r.Value == model.MinorGroup);
                    return $"<strong>Minor group</strong>: {selectedCategory?.Text}<br>";
                }
                case NocCategoryLevel.UnitGroup:
                {
                    SelectListItem selectedCategory = selectLists.UnitGroups
                        .FirstOrDefault(r => r.Value == model.UnitGroup);
                    return $"<strong>Unit group</strong>: {selectedCategory?.Text}<br>";
                }
                default:
                    return "<strong>NOC category</strong>: All NOC codes<br>";
            }
        }

        public static string GetRegionHeader(this SelectListService selectLists, int? regionId)
        {
            if (regionId == null)
            {
                return "<strong>Region</strong>: All Regions<br>";
            }

            SelectListItem selectedRegion =
                selectLists.RegionsIncludingHidden.FirstOrDefault(r => r.Value == regionId.ToString());

            if (selectedRegion == null)
            {
                return "<strong>Region</strong>: All Regions<br>";
            }

            return $"<strong>Region</strong>: {selectedRegion.Text}<br>";
        }

        public static string ToExcelParams(this string p)
        {
            if (p.EndsWith("<br>"))
            {
                p = p.Substring(0, p.Length - 4);
            }

            return p.Replace("<strong>", "")
                .Replace("</strong>", "")
                .Replace("<br>", "  \u2215  ");
        }

        public static string GetJsAccountReportTypeHeader(this JobSeekerAccountViewModel model)
        {
            switch (model.ReportType)
            {
                case "STATUS":
                    return " - Account Status";
                case "ACTIVITY":
                    return " - Account Activity";
                case "GROUPS":
                    return " - Job Seeker Employment Groups";
                default:
                    return "";
            }
        }

        public static HtmlString Logo()
        {
            return new HtmlString("<img src=\"/img/workbc-logo-only.png\"><br><br>");
        }
    }
}
