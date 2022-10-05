using System;
using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobsByIndustryViewModel
    {
        public int RegionId { get; set; }
        public IList<JobsByIndustryResult> Results { get; set; }
    }

    public partial class JobsByIndustryViewModel : IDateRangePicker
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }
}