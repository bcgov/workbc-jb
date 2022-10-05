using System;
using Microsoft.AspNetCore.Html;
using WorkBC.Admin.Areas.Reports.Models.Partial;
using WorkBC.Admin.Areas.Reports.Services;

namespace WorkBC.Admin.Areas.Reports.Extensions
{
    public static class DateRangeExtensions
    {
        public static HtmlString GetHeaderTotalLabel(this IMatrixDateRangeParams model)
        {
            DateTime startDate = model.GetStartDate();
            DateTime endDate = model.GetEndDate();

            switch (model.DateTypeToggle)
            {
                case "yearly":

                    if (endDate > DateTime.Now)
                    {
                        return new HtmlString("TOTAL TO DATE");
                    }
                    else
                    {
                        return new HtmlString("ALL YEARS");
                    }

                case "monthly":

                    int totalMonths = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1;

                    if (endDate.AddDays(1) > DateTime.Now)
                    {
                        return new HtmlString(totalMonths == 12 ? "YTD <br>TOTAL" : "TOTAL <br>TO DATE");
                    }
                    else
                    {
                        return new HtmlString(totalMonths == 12 ? "YEARLY <br>Total" : "PERIOD <br>TOTAL");
                    }

                case "weekly":

                    DateTime nextMonth = new DateTime(model.WeeklyYear, model.WeeklyMonth, 1).AddMonths(1);

                    return new HtmlString(DateTime.Now > nextMonth ? "MONTHLY <br>TOTAL" : "MTD <br>TOTAL");
            }

            return new HtmlString("TOTAL");
        }

        public static DateTime GetStartDate(this IMatrixDateRangeParams model)
        {
            switch (model.DateTypeToggle)
            {
                case "yearly":
                    return new DateTime(model.FiscalYearFrom - 1, 4, 1);
                case "monthly":
                    return new DateTime(model.MonthlyStartYear, model.MonthlyStartMonth, 1);
                case "weekly":
                    return new DateTime(model.WeeklyYear, model.WeeklyMonth, 1);
                default:
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
        }

        public static DateTime GetEndDate(this IMatrixDateRangeParams model)
        {
            switch (model.DateTypeToggle)
            {
                case "yearly":
                    return new DateTime(model.FiscalYearTo, 3, 31);
                case "monthly":
                    return LastDayOfMonth(model.MonthlyEndYear, model.MonthlyEndMonth);
                case "weekly":
                    return LastDayOfMonth(model.WeeklyYear, model.WeeklyMonth);
                default:
                    return LastDayOfMonth(DateTime.Now.Year, DateTime.Now.Month);
            }
        }

        private static DateTime LastDayOfMonth(int year, int month)
        {
            return new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        }
    }
}