using System;
using System.Collections.Generic;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Web.Models
{
    public class SavedJobsModel
    {
        // Job properties
        public long JobId { get; set; }
        public string Title { get; set; }
        public int LocationId { get; set; }
        public string City { get; set; }
        public string EmployerName { get; set; }
        public decimal? Salary { get; set; }
        public string SalarySummary { get; set; }
        public short PositionsAvailable { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
        public byte Source { get; set; }
        public string ExternalSourceName { get; set; }
        public string ExternalSourceUrl { get; set; }
        public List<string> HoursOfWork { get; set; }
        public List<string> PeriodOfEmployment { get; set; }
        public ExternalJobSource ExternalSource { get; set; }

        // JobView properties
        public int Views { get; set; }
        public bool IsNew { get; set; }

        // SavedJob properties
        public int Id { get; set; }
        public string Note { get; set; }
    }
}