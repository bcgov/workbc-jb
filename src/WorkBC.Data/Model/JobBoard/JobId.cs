using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        public DateTime DateFirstImported { get; set; }

        public virtual JobSource JobSource { get; set; }

        [ForeignKey("JobSource")]
        public byte JobSourceId { get; set; }
    }
}