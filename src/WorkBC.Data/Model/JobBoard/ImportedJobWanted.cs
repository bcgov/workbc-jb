using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class ImportedJobWanted : IImportedJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual JobId Id { get; set; }

        public string JobPostEnglish { get; set; }

        [NotMapped]
        public string JobPostFrench
        {
            get => string.Empty;
            set => throw new NotImplementedException();
        } 

        public DateTime DateFirstImported { get; set; }

        public DateTime DateLastImported { get; set; }

        public DateTime ApiDate { get; set; }

        public bool ReIndexNeeded { get; set; }

        public bool IsFederalOrWorkBc { get; set; }

        public long HashId { get; set; }

        public DateTime DateLastSeen { get; set; }
    }
}