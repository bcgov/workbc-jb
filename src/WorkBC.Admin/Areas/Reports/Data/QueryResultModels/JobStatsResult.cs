namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobStatsResult : IMatrixResult
    {
        public string Label { get; set; }

        public int Vacancies { get; set; }
        public int Postings { get; set; }
        public int SortOrder { get; set; }
        public short CalendarYear { get; set; }
        public short? FiscalYear { get; set; }
        public byte CalendarMonth { get; set; }
        public byte? WeekOfMonth { get; set; }
    }
}