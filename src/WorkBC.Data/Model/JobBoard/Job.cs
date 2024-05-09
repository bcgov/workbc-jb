using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard
{
    public class Job
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual JobId Id { get; set; }

        [StringLength(300)]
        public string Title { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        [StringLength(120)]
        public string City { get; set; }

        [StringLength(100)]
        public string EmployerName { get; set; }

        /// <summary>
        ///     2016 Noc Code
        /// </summary>
        public short? NocCodeId { get; set; }

        [ForeignKey("NocCodeId")]
        public virtual NocCode NocCode { get; set; }

        /// <summary>
        ///     2021 Noc Code
        /// </summary>
        public int? NocCodeId2021 { get; set; }

        [ForeignKey("NocCodeId2021")]
        public virtual NocCode2021 NocCode2021 { get; set; }

        /// <summary>
        ///     Industry code
        /// </summary>
        public short? IndustryId { get; set; }

        [ForeignKey("IndustryId")]
        public virtual Industry Industry { get; set; }

        /// <summary>
        ///     Salary amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Salary { get; set; }

        /// <summary>
        ///     Salary summary string to appear in saved job results
        /// </summary>
        [StringLength(60)]
        public string SalarySummary { get; set; }

        public short PositionsAvailable { get; set; }

        public DateTime DatePosted { get; set; }

        public DateTime ActualDatePosted { get; set; }

        public DateTime ExpireDate { get; set; }

        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The first time our importers found a job with this job id
        /// </summary>
        public DateTime DateFirstImported { get; set; }

        /// <summary>
        ///     The date the job was last updated by the importer
        /// </summary>
        public DateTime DateLastImported { get; set; }

        public bool IsActive { get; set; }

        public bool FullTime { get; set; }

        public bool PartTime { get; set; }

        public bool LeadingToFullTime { get; set; }

        public bool Permanent { get; set; }

        public bool Temporary { get; set; }

        public bool Casual { get; set; }

        public bool Seasonal { get; set; }

        public virtual JobSource JobSource { get; set; }

        [ForeignKey("JobSource")]
        public byte JobSourceId { get; set; }

        [StringLength(100)]
        public string OriginalSource { get; set; }

        [StringLength(800)]
        public string ExternalSourceUrl { get; set; }

        [NotMapped]
        public List<string> HoursOfWork
        {
            get
            {
                var list = new List<string>();
                if (FullTime)
                {
                    list.Add("Full-time");
                }


                if (PartTime)
                {
                    list.Add("Part-time");
                }


                if (LeadingToFullTime)
                {
                    list.Add("Part-time leading to full-time");
                }

                return list;
            }
        }

        [NotMapped]
        public List<string> PeriodOfEmployment
        {
            get
            {
                var list = new List<string>();

                if (Permanent)
                {
                    list.Add("Permanent");
                }

                if (Temporary)
                {
                    list.Add("Temporary");
                }

                if (Casual)
                {
                    list.Add("Casual");
                }

                if (Seasonal)
                {
                    list.Add("Seasonal");
                }

                return list;
            }
        }

        [NotMapped]
        public ExternalJobSource ExternalSource =>
            new ExternalJobSource
            {
                Source = new List<ExternalSource>
                {
                    new ExternalSource {Source = OriginalSource, Url = ExternalSourceUrl}
                }
            };

    }

    [NotMapped]
    public class ExternalJobSource
    {
        public List<ExternalSource> Source { get; set; }
    }

    [NotMapped]
    public class ExternalSource
    {
        public string Url { get; set; }
        public string Source { get; set; }
    }
}