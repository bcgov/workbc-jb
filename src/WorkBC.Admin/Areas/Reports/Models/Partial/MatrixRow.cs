using System.Collections.Generic;
using System.Linq;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public class MatrixRow
    {
        public string Label { get; set; }
        public string GroupKey { get; set; }
        public int SortOrder { get; set; }
        public bool IsTotal { get; set; }
        public List<int> Values { get; set; }
        public int Total => IsTotal ? Values.Last() : Values.Sum();
    }
}