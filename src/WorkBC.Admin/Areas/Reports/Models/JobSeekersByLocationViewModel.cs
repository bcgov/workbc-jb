﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Models
{
    public partial class JobSeekersByLocationViewModel
    {
        public MatrixReport Users { get; set; }
        public int TotalJobSeekerCount { get; set; }
        public bool IsPeriodToDate { get; set; }
        public DateTime? PersistentDataTimestamp { get; set; }
    }

    public partial class JobSeekersByLocationViewModel : IMatrixReportViewModel
    {
        public List<HtmlString> TableHeadings { get; set; }
    }

    public partial class JobSeekersByLocationViewModel : IMatrixDateRangeParams
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
        public bool IsJobSeekerReport { get; set; } = true;
    }
}