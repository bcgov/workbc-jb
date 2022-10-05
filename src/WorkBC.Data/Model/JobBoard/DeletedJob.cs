using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class DeletedJob
    {
        [Key]
        [ForeignKey("Job")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JobId { get; set; }

        public Job Job { get; set; }

        [ForeignKey("AdminUser")]
        public int DeletedByAdminUserId { get; set; }

        public virtual AdminUser DeletedByAdminUser { get; set; }

        public DateTime DateDeleted { get; set; }
    }
}