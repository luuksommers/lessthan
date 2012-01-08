using System;
using System.Globalization;
using System.Web.Mvc;

namespace LessThan.Helpers
{
    public static class DateHelpers
    {
        public static MvcHtmlString ToFriendlyDueDate(this HtmlHelper html, DateTime? date)
        {
            return new MvcHtmlString(ToFriendlyDueDate(date));
        }

        public static string ToFriendlyDueDate(DateTime? date)
        {
            if (date.HasValue)
            {
                var workingDate = date.Value;
                var diff = (DateTime.Now - workingDate);
                var datestr = ToFormattedDueDate(date);
                if(diff.TotalHours > 0 && diff.TotalHours < 24)
                {
                    return "vandaag";
                }
                if (diff.TotalHours < 0 && diff.TotalHours > -24)
                {
                    return "morgen";
                }
                if (diff.TotalHours > 24 && diff.TotalHours < 48)
                {
                    return "gisteren";
                }
                if (diff.TotalDays > -7 && diff.TotalDays < 0)
                {
                    return workingDate.ToString("dddd", new CultureInfo(1043).DateTimeFormat);
                }
                return datestr;
            }
            return string.Empty;
        }

        public static string ToFormattedDueDate(DateTime? date)
        {
            if (date.HasValue)
            {
                var workingDate = date.Value;
                var diff = (DateTime.Now - workingDate);
                var datestr = workingDate.ToString("d MMMM");
                if (diff.TotalDays < -100)
                {
                    datestr = workingDate.ToString("d MMMM yyyy");
                }
                return datestr;
            }
            return string.Empty;
        }
    }
}