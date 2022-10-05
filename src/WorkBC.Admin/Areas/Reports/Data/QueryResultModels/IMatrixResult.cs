namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public interface IMatrixResult
    {
        short CalendarYear { get; set; }
        short? FiscalYear { get; set; }
        byte CalendarMonth { get; set; }
        byte? WeekOfMonth { get; set; }
    }
}