using System;
using System.Collections.Generic;

namespace WorkBC.Admin.Areas.JobSeekers.Models
{
    public class JobSeekerViewCommentsViewModel
    {
        public string JobSeekerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public List<JobSeekerJobComment> Comments { get; set; }
    }

    public class JobSeekerJobComment
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Comment { get; set; }
        public string CommentMadeBy { get; set; }
        public bool IsSticky { get; set; }
    }
}
