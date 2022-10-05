using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobSeekerDetailViewModel
    {
        public IList<JobSeekerDetailResult> Results { get; set; }
        public int Count { get; set; }
        public long Cookie { get; set; }
    }

    public partial class JobSeekerDetailViewModel : IJobSeekerDateRangePicker
    {
        public string DateTypeToggle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }
}