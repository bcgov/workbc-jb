namespace WorkBC.Admin.Areas.JobSeekers.Models
{
    public class JobSeekerSearchViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string LastModified { get; set; }
        public string DateRegistered { get; set; }
        public string AccountStatus { get; set; }
        public string Actions { get; set; }
        public int? LockedByAdminUserId { get; set; }
        public string AdminDisplayName { get; set; }
        public string DateLocked { get; set; }
        public string TimeLocked { get; set; }
        public bool LockedByCurrentAdmin { get; set; }
    }
}
