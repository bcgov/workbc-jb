using System;

namespace WorkBC.Admin.Areas.Jobs.Models
{
    public class JobPostingHistoryItem
    {
        public DateTime TimeStamp { get; set; }
        public string Activity { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Editor { get; set; }
    }
}