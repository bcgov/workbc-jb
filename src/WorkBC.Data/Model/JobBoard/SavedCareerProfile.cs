using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorkBC.Data.Model.JobBoard
{
    public class SavedCareerProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("NocCodeId2021")]
        public int NocCodeId2021 { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        [JsonIgnore]
        public virtual JobSeeker JobSeeker { get; set; }

        public DateTime DateSaved { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}