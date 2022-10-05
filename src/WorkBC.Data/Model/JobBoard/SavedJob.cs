using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class SavedJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        [ForeignKey("Job")]
        public long JobId { get; set; }

        public Job Job { get; set; }

        public DateTime DateSaved { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool IsDeleted { get; set; } = false;

        [MaxLength(800)]
        public string Note { get; set; }

        public DateTime? NoteUpdatedDate { get; set; }
    }
}