using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard.ReportData;

namespace WorkBC.Admin.Areas.Reports.Services
{
    public class MatrixReportService
    {
        private readonly JobBoardContext _jobBoardContext;

        public MatrixReportService(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        /// <summary>
        ///     Checks if weekly period exist for the specified month and create them if they are missing
        /// </summary>
        public async Task EnsureWeeklyPeriodsExist(DateTime startDate, DateTime endDate)
        {
            IList<WeeklyPeriod> weeklyPeriods =
                await _jobBoardContext.WeeklyPeriods
                    .Where(p => p.WeekStartDate >= startDate && p.WeekEndDate <= endDate)
                    .ToListAsync();

            var month = (byte) startDate.Month;

            for (var year = (short) startDate.Year; year <= endDate.Year; year++)
            {
                int monthDays = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;

                while (month <= 12 && new DateTime(year, month, monthDays) <= endDate)
                {
                    monthDays = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;

                    for (var week = 1; week <= 5; week++)
                    {
                        WeeklyPeriod period =
                            weeklyPeriods.FirstOrDefault(p =>
                                p.CalendarYear == year && p.CalendarMonth == month && p.WeekOfMonth == week);

                        if (period == null)
                        {
                            var newPeriod = new WeeklyPeriod
                            {
                                CalendarMonth = month,
                                CalendarYear = year,
                                WeekOfMonth = (byte) week,
                                FiscalYear = month <= 3 ? year : (short) (year + 1)
                            };

                            if (week < 5)
                            {
                                switch (week)
                                {
                                    case 1:
                                        newPeriod.WeekStartDate = new DateTime(year, month, 1);
                                        newPeriod.WeekEndDate = new DateTime(year, month, 7);
                                        break;
                                    case 2:
                                        newPeriod.WeekStartDate = new DateTime(year, month, 8);
                                        newPeriod.WeekEndDate = new DateTime(year, month, 14);
                                        break;
                                    case 3:
                                        newPeriod.WeekStartDate = new DateTime(year, month, 15);
                                        newPeriod.WeekEndDate = new DateTime(year, month, 21);
                                        break;
                                    case 4:
                                        newPeriod.WeekStartDate = new DateTime(year, month, 22);
                                        newPeriod.WeekEndDate = new DateTime(year, month, 28);
                                        if (monthDays == 28)
                                        {
                                            newPeriod.IsEndOfMonth = true;
                                        }
                                        break;
                                }

                                await _jobBoardContext.WeeklyPeriods.AddAsync(newPeriod);
                                await _jobBoardContext.SaveChangesAsync();
                            }
                            else if (week == 5 && monthDays > 28)
                            {
                                newPeriod.WeekStartDate = new DateTime(year, month, 29);
                                newPeriod.WeekEndDate = new DateTime(year, month, monthDays);
                                newPeriod.IsEndOfMonth = true;

                                if (newPeriod.CalendarMonth == 3)
                                {
                                    newPeriod.IsEndOfFiscalYear = true;
                                }

                                await _jobBoardContext.WeeklyPeriods.AddAsync(newPeriod);
                                await _jobBoardContext.SaveChangesAsync();
                            }
                        }
                    }

                    month++;
                    if (month != 13)
                    {
                        monthDays = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;
                    }
                }

                month = 1;
            }
        }

        /// <summary>
        ///     Gets the HTML table headings for a weekly matrix table
        /// </summary>
        public static List<HtmlString> GetWeeklyTableHeadings(IList<IMatrixResult> results)
        {
            IMatrixResult d = results.First();
            var period = new DateTime(d.CalendarYear, d.CalendarMonth, 1);

            bool isCurrentMonth = d.CalendarYear == DateTime.Now.Year && d.CalendarMonth == DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;

            int daysInMonth = period.AddMonths(1).AddDays(-1).Day;

            var headings = new List<HtmlString>();
            var month = period.ToString("MMM");

            headings.Add(new HtmlString($"{month} 1-7"));

            if (!isCurrentMonth || currentDay >= 8)
            {
                headings.Add(new HtmlString($"{month} 8-14"));
            }

            if (!isCurrentMonth || currentDay >= 15)
            {
                headings.Add(new HtmlString($"{month} 15-21"));
            }

            if (!isCurrentMonth || currentDay >= 22)
            {
                headings.Add(new HtmlString($"{month} 22-28"));
            }

            if ((!isCurrentMonth || currentDay >= 29) && daysInMonth > 28)
            {
                headings.Add(new HtmlString($"{month} 29-{daysInMonth}"));
            }

            return headings;
        }

        /// <summary>
        ///     Gets the HTML table headings for a monthly matrix table
        /// </summary>
        public static List<HtmlString> GetMonthlyTableHeadings(IList<IMatrixResult> results)
        {
            return results
                .Select(r => new DateTime(r.CalendarYear, r.CalendarMonth, 1))
                .Distinct()
                .OrderBy(r => r)
                .Select(r => new HtmlString(r.ToString("MMM <br>yyyy")))
                .ToList();
        }

        /// <summary>
        ///     Gets the HTML table headings for a yearly matrix table
        /// </summary>
        public static List<HtmlString> GetYearlyTableHeadings(IList<IMatrixResult> results)
        {
            return results
                .Select(r => r.FiscalYear)
                .Distinct()
                .OrderBy(r => r)
                .Select(r => new HtmlString($"{(r - 1)}/{r-2000}"))
                .ToList();
        }




        public static void SetDatePickerModelDefaults(IMatrixDateRangeParams model)
        {
            model.DateTypeToggle = "monthly";
            model.MonthlyRangeType = "p12";
            model.MonthlyStartMonth = (byte) DateTime.Now.AddMonths(-11).Month;
            model.MonthlyStartYear = (short) DateTime.Now.AddMonths(-11).Year;
            model.MonthlyEndMonth = (byte) DateTime.Now.Month;
            model.MonthlyEndYear = (short) DateTime.Now.Year;
            model.WeeklyMonth = (byte) DateTime.Now.Month;
            model.WeeklyYear = (short) DateTime.Now.Year;
            model.FiscalYearTo = (short) (DateTime.Now.Month <= 3 ? DateTime.Now.Year : DateTime.Now.Year + 1);
        }
    }
}