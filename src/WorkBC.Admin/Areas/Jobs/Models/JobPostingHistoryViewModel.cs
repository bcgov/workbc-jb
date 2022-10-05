using System.Collections.Generic;

namespace WorkBC.Admin.Areas.Jobs.Models
{
    public class JobPostingHistoryViewModel
    {
        public long JobId { get; set; }
        public string City { get; set; }
        public string JobTitle { get; set; }
        public List<JobPostingHistoryItem> History { get; set; }
    }
}