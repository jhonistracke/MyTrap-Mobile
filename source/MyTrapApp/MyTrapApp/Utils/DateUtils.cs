using System;
using System.Globalization;

namespace MyTrapApp.Utils
{
    public class DateUtils
    {
        public static string DATE_FORMAT_YYYY_MM_DD_HH_MM_SS = "yyyy-MM-dd HH:mm:ss";

        public static DateTime StringToDate(string date, string format = "")
        {
            if (string.IsNullOrEmpty(format))
            {
                return DateTime.ParseExact(date, DATE_FORMAT_YYYY_MM_DD_HH_MM_SS, CultureInfo.InvariantCulture);
            }
            else
            {
                return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            }
        }
    }
}