using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class ExpiredJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual JobId Id { get; set; }

        //Indicates if the job was removed from the Elastic Search Index
        public bool RemovedFromElasticsearch { get; set; }

        public DateTime DateRemoved { get; set; }
    }
}