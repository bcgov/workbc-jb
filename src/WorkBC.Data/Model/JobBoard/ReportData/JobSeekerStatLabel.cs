using System.ComponentModel.DataAnnotations;

namespace WorkBC.Data.Model.JobBoard.ReportData
{
    public class JobSeekerStatLabel
    {
        [Key]
        [StringLength(4)]
        public string Key { get; set; }

        [StringLength(100)]
        public string Label { get; set; }

        public bool IsTotal { get; set; }
    }
}