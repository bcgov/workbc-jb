using System.Collections.Generic;

namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobSeekerAccountResult : IMatrixResult
    {
        private readonly Dictionary<string, KeyValuePair<int, string>> _metadata;


        public JobSeekerAccountResult()
        {
            _metadata = new Dictionary<string, KeyValuePair<int, string>>
            {
                {"REGD", new KeyValuePair<int, string>(1, "STATUS")},
                {"NOAC", new KeyValuePair<int, string>(2, "STATUS")},
                {"DEAC", new KeyValuePair<int, string>(3, "STATUS")},
                {"DEL", new KeyValuePair<int, string>(4, "STATUS")},

                {"APPR", new KeyValuePair<int, string>(1, "GROUPS")},
                {"INDP", new KeyValuePair<int, string>(2, "GROUPS")},
                {"MAT", new KeyValuePair<int, string>(3, "GROUPS")},
                {"IMMG", new KeyValuePair<int, string>(4, "GROUPS")},
                {"PWD", new KeyValuePair<int, string>(5, "GROUPS")},
                {"STUD", new KeyValuePair<int, string>(6, "GROUPS")},
                {"VET", new KeyValuePair<int, string>(7, "GROUPS")},
                {"VMIN", new KeyValuePair<int, string>(8, "GROUPS")},
                {"YTH", new KeyValuePair<int, string>(9, "GROUPS")},

                {"LOGN", new KeyValuePair<int, string>(1, "ACTIVITY")},
                {"ALRT", new KeyValuePair<int, string>(2, "ACTIVITY")},
                {"CAPR", new KeyValuePair<int, string>(3, "ACTIVITY")},
                {"INPR", new KeyValuePair<int, string>(4, "ACTIVITY")},

                {"CFEM", new KeyValuePair<int, string>(1, "NONE")}
            };
        }

        public string LabelKey { get; set; }
        public string Label { get; set; }

        public string GroupKey =>
            !string.IsNullOrEmpty(LabelKey) && _metadata.ContainsKey(LabelKey)
                ? _metadata[LabelKey].Value
                : "NONE";

        public int SortOrder =>
            !string.IsNullOrEmpty(LabelKey) && _metadata.ContainsKey(LabelKey)
                ? _metadata[LabelKey].Key
                : 0;

        public bool IsTotal { get; set; }

        public int Value { get; set; }
        public short CalendarYear { get; set; }
        public short? FiscalYear { get; set; }
        public byte CalendarMonth { get; set; }
        public byte? WeekOfMonth { get; set; }
    }
}