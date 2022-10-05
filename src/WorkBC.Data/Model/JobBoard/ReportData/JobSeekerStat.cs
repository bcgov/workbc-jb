using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard.ReportData
{
    public class JobSeekerStat
    {
        [ForeignKey("WeeklyPeriod")]
        public int WeeklyPeriodId { get; set; }

        public WeeklyPeriod WeeklyPeriod { get; set; }

        [ForeignKey("JobSeekerStatLabel")]
        public string LabelKey { get; set; }

        public JobSeekerStatLabel JobSeekerStatLabel { get; set; }

        public Region Region { get; set; }

        [ForeignKey("Region")]
        public int RegionId { get; set; }

        public int Value { get; set; }
    }
}