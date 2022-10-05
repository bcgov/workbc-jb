namespace WorkBC.Web.Models
{
    public class CareerProfileModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NocCode { get; set; }    
    }

    public class CareerProfileTopJobsModel
    {
        public long JobId { get; set; }
        public string DatePosted { get; set; }
        public string JobTitle { get; set; }
        public string Employer { get; set; }
        public string Location { get; set; }
        public string ExternalUrl { get; set; }
        public string ExteralSource { get; set; }
    }
}
