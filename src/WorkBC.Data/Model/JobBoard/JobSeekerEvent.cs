using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobSeekerEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        public virtual JobSeeker JobSeeker { get; set; }

        public EventType EventTypeId { get; set; }

        public DateTime DateLogged { get; set; }
    }
}