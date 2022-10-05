using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard.ReportData
{
    public class JobStat
    {
        [ForeignKey("WeeklyPeriod")]
        public int WeeklyPeriodId { get; set; }

        public WeeklyPeriod WeeklyPeriod { get; set; }

        public virtual JobSource JobSource { get; set; }

        [ForeignKey("JobSource")]
        public byte JobSourceId { get; set; }

        public Region Region { get; set; }

        [ForeignKey("Region")]
        public int RegionId { get; set; }

        public int JobPostings { get; set; }

        public int PositionsAvailable { get; set; }
    }
}