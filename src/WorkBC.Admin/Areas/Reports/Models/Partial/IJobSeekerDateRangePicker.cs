using System;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public interface IJobSeekerDateRangePicker : IDateRangePicker
    {
        string DateTypeToggle { get; set; }
        new DateTime? StartDate { get; set; }
        new DateTime? EndDate { get; set; }
    }
}