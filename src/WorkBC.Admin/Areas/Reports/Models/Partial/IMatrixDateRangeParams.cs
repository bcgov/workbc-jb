namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public interface IMatrixDateRangeParams
    {
        string DateTypeToggle { get; set; }

        // yearly
        short FiscalYearFrom { get; set; }
        short FiscalYearTo { get; set; }

        // monthly
        string MonthlyRangeType { get; set; }
        byte MonthlyStartMonth { get; set; }
        short MonthlyStartYear { get; set; }
        byte MonthlyEndMonth { get; set; }
        short MonthlyEndYear { get; set; }

        // weekly
        short WeeklyYear { get; set; }
        short WeeklyMonth { get; set; }

        // toggle to determine which years to show
        bool IsJobSeekerReport { get; set; }

        public string FiscalYearFromFormatted()
        {
            var from1 = (FiscalYearFrom - 1).ToString();
            var from2 = (FiscalYearFrom - 2000).ToString();
            return $"{from1}/{from2}";
        }

        public string FiscalYearToFormatted()
        {
            var to1 = (FiscalYearTo - 1).ToString();
            var to2 = (FiscalYearTo - 2000).ToString();
            return $"{to1}/{to2}";
        }
    }
}