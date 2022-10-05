using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobAlert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "tinyint")]
        public JobAlertFrequency AlertFrequency { get; set; }

        [MaxLength(1000)]
        public string UrlParameters { get; set; }

        public string JobSearchFilters { get; set; }
        public int JobSearchFiltersVersion { get; set; } = 1;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
