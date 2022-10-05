using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public interface IMatrixReportViewModel : IMatrixDateRangeParams
    {
        public List<HtmlString> TableHeadings { get; set; }
    }
}