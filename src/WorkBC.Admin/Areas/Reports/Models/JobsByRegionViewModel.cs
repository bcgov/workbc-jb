using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobsByRegionViewModel
    {
        public MatrixReport Postings { get; set; }
        public MatrixReport Vacancies { get; set; }
    }

    public partial class JobsByRegionViewModel : IMatrixReportViewModel
    {
        public List<HtmlString> TableHeadings { get; set; }
    }

    public partial class JobsByRegionViewModel : IMatrixDateRangeParams
    {
        public string DateTypeToggle { get; set; }
        public short FiscalYearFrom { get; set; }
        public short FiscalYearTo { get; set; }
        public string MonthlyRangeType { get; set; }
        public byte MonthlyStartMonth { get; set; }
        public short MonthlyStartYear { get; set; }
        public byte MonthlyEndMonth { get; set; }
        public short MonthlyEndYear { get; set; }
        public short WeeklyYear { get; set; }
        public short WeeklyMonth { get; set; }
        public bool IsJobSeekerReport { get; set; } = false;
    }

    public partial class JobsByRegionViewModel : IJobSourceParams
    {
        public int JobSourceId { get; set; }
    }
}