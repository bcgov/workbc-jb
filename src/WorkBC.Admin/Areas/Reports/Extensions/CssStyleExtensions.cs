using System.Collections.Generic;
using WorkBC.Admin.Areas.Reports.Models.Partial;

namespace WorkBC.Admin.Areas.Reports.Extensions
{
    public static class CssStyleExtensions
    {
        public const string Header =
            "font-size: 0.95em; font-weight: bold; border-top: none; border-bottom: 1px solid #dee2e6;";

        public const string Title =
            "font-size: 1.3em; font-weight: bold; border-top: none; border-bottom: 1px solid #dee2e6;";

        public const string Space =
            "border: none;";

        public const string Footer =
            "font-weight: bold; border-top: 2px solid #dee2e6; border-bottom: 1px solid #dee2e6;";

        public static string GetGroupCss(this List<int> groupRows)
        {
            var groupTitleCssSelectors = new List<string>();
            var groupHeaderCssSelectors = new List<string>();
            var groupSpaceCssSelectors = new List<string>();
            var groupFooterCssSelectors = new List<string>();
            for (var i = 0; i < groupRows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    groupTitleCssSelectors.Add("tr:nth-of-type(" + (groupRows[i] + 1) + ") td");
                    groupHeaderCssSelectors.Add("tr:nth-of-type(" + (groupRows[i] + 2) + ") td");

                    if (i > 0)
                    {
                        groupSpaceCssSelectors.Add("tr:nth-of-type(" + (groupRows[i] - 1) + ") td");
                    }
                }
                else
                {
                    groupFooterCssSelectors.Add("tr:nth-of-type(" + groupRows[i] + ") td");
                }
            }

            var groupCss = "thead { display: none; }";

            groupCss += string.Join(',', groupTitleCssSelectors) + " { " + Title + " }"
                        + string.Join(',', groupHeaderCssSelectors) + " { " + Header + " }"
                        + string.Join(',', groupSpaceCssSelectors) + " { " + Space + " }"
                        + string.Join(',', groupFooterCssSelectors) + " { " + Footer + " }";

            return groupCss;
        }

        public static string GetGroupCss(this MatrixReport model)
        {
            int groupRowCount = model.TableData.Count;

            // Add css to each of the group header/footer rows to make them bold in the print version

            var groupTitleCssSelectors = new List<string>();
            var groupHeaderCssSelectors = new List<string>();
            var groupSpaceCssSelectors = new List<string>();
            var groupFooterCssSelectors = new List<string>();

            groupTitleCssSelectors.Add("tr:nth-of-type(1) td");
            groupHeaderCssSelectors.Add("tr:nth-of-type(2) td");

            groupFooterCssSelectors.Add($"tr:nth-of-type({groupRowCount + 3}) td");
            groupFooterCssSelectors.Add($"tr:nth-of-type({groupRowCount * 2 + 7}) td");

            groupTitleCssSelectors.Add($"tr:nth-of-type({groupRowCount + 5}) td");
            groupHeaderCssSelectors.Add($"tr:nth-of-type({groupRowCount + 6}) td");

            groupSpaceCssSelectors.Add($"tr:nth-of-type({groupRowCount + 4}) td");
            groupSpaceCssSelectors.Add($"tr:nth-of-type({groupRowCount * 2 + 8}) td");

            var groupCss = "thead { display: none; }";

            groupCss += string.Join(',', groupTitleCssSelectors) + " { " + Title + " }"
                        + string.Join(',', groupHeaderCssSelectors) + " { " + Header + " }"
                        + string.Join(',', groupSpaceCssSelectors) + " { " + Space + " }"
                        + string.Join(',', groupFooterCssSelectors) + " { " + Footer + " }";

            return groupCss;
        }
    }
}