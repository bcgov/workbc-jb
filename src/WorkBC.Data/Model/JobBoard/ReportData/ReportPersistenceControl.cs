using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard.ReportData
{
    public class ReportPersistenceControl
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WeeklyPeriodId { get; set; }

        [StringLength(25)]
        public string TableName { get; set; }

        [ForeignKey("WeeklyPeriodId")]
        public virtual WeeklyPeriod WeeklyPeriod { get; set; }

        public DateTime DateCalculated { get; set; }

        public bool IsTotalToDate { get; set; }
    }
}