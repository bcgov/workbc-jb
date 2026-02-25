using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobView
    {
        [Key] [ForeignKey("Job")] public string JobId { get; set; }

        public virtual Job Job { get; set; }

        public int? Views { get; set; }

        public DateTime DateLastViewed { get; set; }
    }
}