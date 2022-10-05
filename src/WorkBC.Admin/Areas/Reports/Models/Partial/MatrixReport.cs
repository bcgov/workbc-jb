using System.Collections.Generic;
using System.Linq;

namespace WorkBC.Admin.Areas.Reports.Models.Partial
{
    public class MatrixReport
    {
        public IList<MatrixRow> TableData { get; set; }
        public string FooterTotalLabel { get; set; }

        public int Total
        {
            get { return TableData.Sum(row => row.Values.Sum()); }
        }

        public int SumColumn(int index)
        {
            if (index >= TableData.First().Values.Count)
            {
                return 0;
            }

            return TableData.Sum(row => row.Values[index]);
        }

        public MatrixReport StartsWith(string filter)
        {
            return new MatrixReport
            {
                TableData = TableData.Where(td => td.Label.ToLower().StartsWith(filter.ToLower())).ToList(),
                FooterTotalLabel = FooterTotalLabel
            };
        }

        public MatrixReport DoesNotStartWith(string filter)
        {
            return new MatrixReport
            {
                TableData = TableData.Where(td => !td.Label.ToLower().StartsWith(filter.ToLower())).ToList(),
                FooterTotalLabel = FooterTotalLabel
            };
        }

        public MatrixReport HasGroupKey(string filter)
        {
            return new MatrixReport
            {
                TableData = TableData.Where(td => td.GroupKey.ToLower().StartsWith(filter.ToLower())).OrderBy(td => td.SortOrder).ToList(),
                FooterTotalLabel = FooterTotalLabel
            };
        }
    }
}