using System;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public interface IDateRangePicker
    {
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        string DateRangeType { get; set; }
    }
}