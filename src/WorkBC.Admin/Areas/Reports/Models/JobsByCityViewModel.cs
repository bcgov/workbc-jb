using System;
using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobsByCityViewModel
    {
        public IEnumerable<JobsByCityResult> Results { get; set; }
    }

    public partial class JobsByCityViewModel : ICityReportTypeParams
    {
        public string ReportType { get; set; }
        public short? RegionId { get; set; }
    }

    public partial class JobsByCityViewModel : IDateRangePicker
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }

    public partial class JobsByCityViewModel : IJobSourceParams
    {
        public int JobSourceId { get; set; }
    }
}