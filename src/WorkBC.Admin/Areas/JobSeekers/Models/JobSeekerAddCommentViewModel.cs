namespace WorkBC.Admin.Areas.JobSeekers.Models
{
    public class JobSeekerAddCommentViewModel
    {
        public string JobSeekerId { get; set; }
        public string FirstName {get;set;}
        public string LastName { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public bool IsSticky { get; set; }
    }
}
