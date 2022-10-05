using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobSeekerChangeEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        [StringLength(100)]
        public string Field { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [ForeignKey("AdminUser")]
        public int? ModifiedByAdminUserId { get; set; }

        public virtual AdminUser ModifiedByAdminUser { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}