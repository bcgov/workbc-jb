using System;
using Microsoft.AspNetCore.Html;
using WorkBC.Data.Enums;

namespace WorkBC.Admin.Helpers
{
    public static class FormattingExtensions
    {
        public static string YesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        public static string MyDateFormat(this DateTime? value)
        {
            return !value.HasValue
                ? ""
                : value.Value.MyDateFormat();
        }

        public static string MyDateFormat(this DateTime value)
        {
            return $"{value:yyyy-MM-dd HH:mm} PST";
        }

        public static HtmlString HtmlDateFormat(this DateTime? value)
        {
            return !value.HasValue
                ? new HtmlString("")
                : value.Value.HtmlDateFormat();
        }

        public static HtmlString HtmlDateFormat(this DateTime value)
        {
            return new HtmlString($"{value:yyyy&#8209;MM&#8209;dd<br>HH:mm}&nbsp;PST");
        }

        public static string GetLabel(this AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Pending:
                    return "Pending activation";
                default:
                    return status.ToString();
            }
        }
    }
}