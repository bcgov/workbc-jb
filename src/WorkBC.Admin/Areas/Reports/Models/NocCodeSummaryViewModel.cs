using System;
using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;
using WorkBC.Data.Enums;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class NocCodeSummaryViewModel
    {
        public NocCategoryLevel NocCategoryLevel { get; set; }
        public string BroadCategory { get; set; }
        public string MajorGroup { get; set; }
        public string MinorGroup { get; set; }
        public string UnitGroup { get; set; }
        public bool ShowZeroes { get; set; }
    }

    public partial class NocCodeSummaryViewModel : IJobsByNocCodeViewModel
    {
        public IList<JobsByNocCodeResult> Results { get; set; }
    }

    public partial class NocCodeSummaryViewModel : IDateRangePicker
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }

    public partial class NocCodeSummaryViewModel : IJobSourceParams
    {
        public int JobSourceId { get; set; }
    }
}