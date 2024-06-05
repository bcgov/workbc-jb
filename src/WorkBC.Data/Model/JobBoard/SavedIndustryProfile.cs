using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class SavedIndustryProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("IndustryId")]
        public short IndustryId { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        public DateTime DateSaved { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}