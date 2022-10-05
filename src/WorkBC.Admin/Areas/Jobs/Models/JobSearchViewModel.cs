using System;

namespace WorkBC.Admin.Areas.Jobs.Models
{
    public class JobSearchViewModel
    {
        public string JobSource { get; set; }
        public long JobId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string LastUpdated { get; set; }
        public string DatePosted { get; set; }
        public string ExpireDate { get; set; }
        public string Url { get; set; }
        public string OriginalSource { get; set; }
        public byte JobSourceId { get; set; }
    }
}