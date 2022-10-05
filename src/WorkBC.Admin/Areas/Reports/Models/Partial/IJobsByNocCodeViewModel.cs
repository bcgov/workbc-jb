using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Data.QueryResultModels;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public interface IJobsByNocCodeViewModel
    {
        public IList<JobsByNocCodeResult> Results { get; set; }
    }
}