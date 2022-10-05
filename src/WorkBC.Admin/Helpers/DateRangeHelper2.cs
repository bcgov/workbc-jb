using System;

namespace WorkBC.Admin.Helpers
{
    public static class DateRangeHelper2
    {
        public const string DefaultDateRangeType = "p30";

        private static readonly DateTime FirstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public static string Past30Days => $"{DateTime.Now.AddDays(-29):yyyy-MM-dd},{DateTime.Now:yyyy-MM-dd}";

        public static string ThisWeek => $"{DateTime.Now.AddDays(-1 * (int)DateTime.Now.DayOfWeek):yyyy-MM-dd},{DateTime.Now:yyyy-MM-dd}";

        public static string LastWeek => $"{DateTime.Now.AddDays(-1 * (7 + (int)DateTime.Now.DayOfWeek)):yyyy-MM-dd},{DateTime.Now.AddDays(-1 * (1 + (int)DateTime.Now.DayOfWeek)):yyyy-MM-dd}";

        public static string ThisMonth => $"{FirstDayOfThisMonth:yyyy-MM-dd},{DateTime.Now:yyyy-MM-dd}";

        public static string LastMonth => $"{FirstDayOfThisMonth.AddMonths(-1):yyyy-MM-dd},{FirstDayOfThisMonth.AddDays(-1):yyyy-MM-dd}";

        public static string ThisYear => $"{DateTime.Now.Year}-01-01,{DateTime.Now:yyyy-MM-dd}";

        public static string LastYear => $"{DateTime.Now.Year -1}-01-01,{DateTime.Now.Year - 1}-12-31";

        public static string ThisFiscalYear => DateTime.Now.Month <= 3
            ? $"{DateTime.Now.Year - 1}-04-01,{DateTime.Now:yyyy-MM-dd}"
            : $"{DateTime.Now.Year}-04-01,{DateTime.Now:yyyy-MM-dd}";

        public static string LastFiscalYear => DateTime.Now.Month <= 3
            ? $"{DateTime.Now.Year - 2}-04-01,{DateTime.Now.Year - 1}-03-31"
            : $"{DateTime.Now.Year - 1}-04-01,{DateTime.Now.Year}-03-31";
    }
}