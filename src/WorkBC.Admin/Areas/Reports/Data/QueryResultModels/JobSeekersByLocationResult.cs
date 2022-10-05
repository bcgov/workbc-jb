namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobSeekersByLocationResult : IMatrixResult
    {
        public string Label { get; set; }
        public int SortOrder { get; set; }

        public int Users { get; set; }
        public short CalendarYear { get; set; }
        public short? FiscalYear { get; set; }
        public byte CalendarMonth { get; set; }
        public byte? WeekOfMonth { get; set; }
    }
}