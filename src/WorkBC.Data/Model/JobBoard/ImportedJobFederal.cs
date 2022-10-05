using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class ImportedJobFederal : IImportedJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual JobId Id { get; set; }

        public string JobPostEnglish { get; set; }
        public string JobPostFrench { get; set; }

        public DateTime DateLastImported { get; set; }

        public DateTime ApiDate { get; set; }

        public DateTime DateFirstImported { get; set; }

        public bool ReIndexNeeded { get; set; }

        // wanted jobs don't have this field because we just add 30 days to DateRefreshed
        public DateTime? DisplayUntil { get; set; }
    }
}