using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobVersion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        public long JobId { get; set; }

        public byte JobSourceId { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        public short? NocCodeId { get; set; }

        [ForeignKey("NocCodeId")]
        public virtual NocCode NocCode { get; set; }
        public int? NocCodeId2021 { get; set; }

        [ForeignKey("NocCodeId2021")]
        public virtual NocCode2021 NocCode2021 { get; set; }

        public short? IndustryId { get; set; }

        [ForeignKey("IndustryId")]
        public virtual Industry Industry { get; set; }

        public short PositionsAvailable { get; set; }

        public DateTime DatePosted { get; set; }

        public DateTime ActualDatePosted { get; set; }

        public DateTime DateFirstImported { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateVersionStart { get; set; }

        public DateTime? DateVersionEnd { get; set; }

        public bool IsCurrentVersion { get; set; }

        public short VersionNumber { get; set; }
    }
}