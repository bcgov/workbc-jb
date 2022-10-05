using System;

namespace WorkBC.Web.Models
{
    public class JobAlertModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public byte AlertFrequency { get; set; }
        public string UrlParameters { get; set; }
        public string JobSearchFilters { get; set; }
        public string AspNetUserId { get; set; }
        public bool OverwriteExisting { get; set; }
    }
}
