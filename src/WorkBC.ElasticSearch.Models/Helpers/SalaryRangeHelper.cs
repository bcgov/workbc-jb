using System;
using System.Collections.Generic;

namespace WorkBC.ElasticSearch.Models.Helpers
{
    public static class SalaryRangeHelper
    {
        public enum SalaryType
        {
            HOURLY = 0,
            WEEKLY = 1,
            BI_WEEKLY = 2,
            MONTHLY = 3,
            ANNUALLY = 4,
            NONE = 5
        }

        private static SalaryRange[] Hourly
        {
            get
            {
                return new[]
                {
                    new SalaryRange {Minimum = 0m, Maximum = 19.999999m},
                    new SalaryRange {Minimum = 20m, Maximum = 29.999999m},
                    new SalaryRange {Minimum = 30m, Maximum = 39.999999m},
                    new SalaryRange {Minimum = 40m, Maximum = 49.999999m},
                    new SalaryRange {Minimum = 50m, Maximum = 50000m}
                };
            }
        }

        private static SalaryRange[] Weekly
        {
            get
            {
                return new[]
                {
                    new SalaryRange {Minimum = 0m, Maximum = 799.999999m},
                    new SalaryRange {Minimum = 800m, Maximum = 1199.999999m},
                    new SalaryRange {Minimum = 1200m, Maximum = 1599.999999m},
                    new SalaryRange {Minimum = 1600m, Maximum = 1999.999999m},
                    new SalaryRange {Minimum = 2000m, Maximum = 200000m}
                };
            }
        }

        private static SalaryRange[] Biweekly
        {
            get
            {
                return new[]
                {
                    new SalaryRange {Minimum = 0m, Maximum = 1599.999999m},
                    new SalaryRange {Minimum = 1600m, Maximum = 2399.999999m},
                    new SalaryRange {Minimum = 2400m, Maximum = 3199.999999m},
                    new SalaryRange {Minimum = 3200m, Maximum = 3999.999999m},
                    new SalaryRange {Minimum = 4000m, Maximum = 400000m}
                };
            }
        }

        private static SalaryRange[] Monthly
        {
            get
            {
                return new[]
                {
                    new SalaryRange {Minimum = 0m, Maximum = 3999.999999m},
                    new SalaryRange {Minimum = 4000m, Maximum = 5999.999999m},
                    new SalaryRange {Minimum = 6000m, Maximum = 7999.999999m},
                    new SalaryRange {Minimum = 8000m, Maximum = 9999.999999m},
                    new SalaryRange {Minimum = 10000m, Maximum = 10000000m}
                };
            }
        }

        private static SalaryRange[] Annually
        {
            get
            {
                return new[]
                {
                    new SalaryRange {Minimum = 0m, Maximum = 39999.999999m},
                    new SalaryRange {Minimum = 40000m, Maximum = 59999.999999m},
                    new SalaryRange {Minimum = 60000m, Maximum = 79999.999999m},
                    new SalaryRange {Minimum = 80000m, Maximum = 99999.999999m},
                    new SalaryRange {Minimum = 100000m, Maximum = 100000000m}
                };
            }
        }

        public static KeyValuePair<string, string> GetAnnualRange(SalaryType salaryType, int bracket)
        {
            SalaryRange range;
            int multiplier;

            if (bracket > 5 || bracket < 1)
            {
                throw new IndexOutOfRangeException("Salary bracket must be between 1 and 5");
            }

            switch (salaryType)
            {
                case SalaryType.HOURLY:
                    multiplier = 2080;
                    range = Hourly[bracket - 1];
                    break;

                case SalaryType.WEEKLY:
                    multiplier = 52;
                    range = Weekly[bracket - 1];
                    break;

                case SalaryType.BI_WEEKLY:
                    multiplier = 26;
                    range = Biweekly[bracket - 1];
                    break;

                case SalaryType.MONTHLY:
                    multiplier = 12;
                    range = Monthly[bracket - 1];
                    break;

                default:
                    multiplier = 1;
                    range = Annually[bracket - 1];
                    break;
            }

            return new KeyValuePair<string, string>
            (
                (multiplier * range.Minimum).ToString("0.00"),
                (multiplier * range.Maximum -0.01m).ToString("0.00")
            );
        }

        private class SalaryRange
        {
            public decimal Minimum { get; set; }
            public decimal Maximum { get; set; }
        }
    }
}