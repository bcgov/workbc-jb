using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobSeekerVersion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [ForeignKey("Country")]
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        [ForeignKey("Province")]
        public int? ProvinceId { get; set; }

        public virtual Province Province { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        public DateTime DateRegistered { get; set; }

        [Column(TypeName = "smallint")]
        public AccountStatus AccountStatus { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool IsApprentice { get; set; }
        public bool IsIndigenousPerson { get; set; }
        public bool IsMatureWorker { get; set; }
        public bool IsNewImmigrant { get; set; }
        public bool IsPersonWithDisability { get; set; }
        public bool IsStudent { get; set; }
        public bool IsVeteran { get; set; }
        public bool IsVisibleMinority { get; set; }
        public bool IsYouth { get; set; }

        public DateTime DateVersionStart { get; set; }

        public DateTime? DateVersionEnd { get; set; }

        public bool IsCurrentVersion { get; set; }

        public short VersionNumber { get; set; }
    }
}