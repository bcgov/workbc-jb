using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorkBC.Data.Model.JobBoard
{
    public class JobSeekerFlags
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("JobSeeker")]
        public string AspNetUserId { get; set; }

        [JsonIgnore]
        public virtual JobSeeker JobSeeker { get; set; }

        public bool IsApprentice { get; set; }
        public bool IsIndigenousPerson { get; set; }
        public bool IsMatureWorker { get; set; }
        public bool IsNewImmigrant { get; set; }
        public bool IsPersonWithDisability { get; set; }
        public bool IsStudent { get; set; }
        public bool IsVeteran { get; set; }
        public bool IsVisibleMinority { get; set; }
        public bool IsYouth { get; set; }
    }
}