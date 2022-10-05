using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBC.Data.Model.JobBoard.ReportData
{
    public class WeeklyPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public short CalendarYear { get; set; }

        public byte CalendarMonth { get; set; }

        public short FiscalYear { get; set; }

        public byte WeekOfMonth { get; set; }

        public DateTime WeekStartDate { get; set; }

        public DateTime WeekEndDate { get; set; }

        public bool IsEndOfMonth { get; set; }

        public bool IsEndOfFiscalYear { get; set; }
    }
}