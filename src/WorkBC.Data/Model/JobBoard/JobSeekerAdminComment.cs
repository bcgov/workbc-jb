using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobSeekerAdminComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        public string Comment { get; set; }

        public bool IsPinned { get; set; }

        [ForeignKey("AdminUser")]
        public int EnteredByAdminUserId { get; set; }

        public virtual AdminUser EnteredByAdminUser { get; set; }

        public virtual DateTime DateEntered { get; set; }
    }
}