using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Helpers
{
    public static class ExtensionsMethodHelper
    {
        public static decimal TruncateDecimal(this decimal value, int precision = 2)
        {
            var step = (decimal) Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt, DayOfWeek? firstDayOfWeek = null)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - (firstDayOfWeek ?? culture.DateTimeFormat.FirstDayOfWeek);

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt, DayOfWeek? firstDayOfWeek = null) =>
            dt.FirstDayOfWeek(firstDayOfWeek).AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);
    }
}
