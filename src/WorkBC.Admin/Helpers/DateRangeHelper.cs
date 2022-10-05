using System;

namespace WorkBC.Admin.Helpers
{
    public static class DateRangeHelper
    {
        public static string Past12Months => $"{DateTime.Now.AddMonths(-11):MMM yyyy} to {DateTime.Now:MMM yyyy}";

        public static string ThisYear => $"Jan {DateTime.Now.Year} to Dec {DateTime.Now.Year}";

        public static string ThisFiscalYear => DateTime.Now.Month <= 3
            ? $"Apr {DateTime.Now.Year - 1} to Mar {DateTime.Now.Year}"
            : $"Apr {DateTime.Now.Year} to Mar {DateTime.Now.Year + 1}";

        public static string LastYear => $"Jan {DateTime.Now.Year - 1} to Dec {DateTime.Now.Year - 1}";

        public static string LastFiscalYear => DateTime.Now.Month <= 3
            ? $"Apr {DateTime.Now.Year - 2} to Mar {DateTime.Now.Year - 1}"
            : $"Apr {DateTime.Now.Year - 1} to Mar {DateTime.Now.Year}";

        public static string Past12MonthsData =>
            $"{DateTime.Now.AddMonths(-11).Month},{DateTime.Now.AddMonths(-11).Year},{DateTime.Now.Month},{DateTime.Now.Year}";

        public static string ThisYearData => $"1,{DateTime.Now.Year},12,{DateTime.Now.Year}";

        public static string ThisFiscalYearData => DateTime.Now.Month <= 3
            ? $"4,{DateTime.Now.Year - 1},3,{DateTime.Now.Year}"
            : $"4,{DateTime.Now.Year},3,{DateTime.Now.Year + 1}";

        public static string LastYearData => $"1,{DateTime.Now.Year - 1},12,{DateTime.Now.Year - 1}";

        public static string LastFiscalYearData => DateTime.Now.Month <= 3
            ? $"4,{DateTime.Now.Year - 2},3,{DateTime.Now.Year - 1}"
            : $"4,{DateTime.Now.Year - 1},3,{DateTime.Now.Year}";

        
    }
}