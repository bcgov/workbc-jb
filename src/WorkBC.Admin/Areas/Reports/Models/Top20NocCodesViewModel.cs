using System;
using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class Top20NocCodesViewModel : IJobsByNocCodeViewModel
    {
        public IList<JobsByNocCodeResult> Results { get; set; }
    }

    public partial class Top20NocCodesViewModel : IDateRangePicker
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DateRangeType { get; set; }
    }

    public partial class Top20NocCodesViewModel : IJobSourceParams
    {
        public int JobSourceId { get; set; }
    }
}