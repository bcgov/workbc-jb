using System;
using System.Collections.Generic;

namespace WorkBC.Admin.Areas.JobSeekers.Models
{
    public class JobSeekerViewProfileHistoryViewModel
    {
        public string JobSeekerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<JobSeekerProfileHistory> History { get; set; }
    }

    public class JobSeekerProfileHistory
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Activity { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public string Editor { get; set; }
    }
}
