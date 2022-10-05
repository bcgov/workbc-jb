using System;
using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobSeekersByCityViewModel
    {
        public IList<JobSeekersByCityResult> Results { get; set; }
    }

    public partial class JobSeekersByCityViewModel : ICityReportTypeParams
    {
        public string ReportType { get; set; }
        public short? RegionId { get; set; }
    }

    public partial class JobSeekersByCityViewModel : IJobSeekerDateRangePicker
    {
        public string DateTypeToggle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }
}