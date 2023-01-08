using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OCEL.CSharp;

namespace Serilog.Sinks.OCEL
{
    internal static class Helpers
    {
        /// <summary>
        /// Determine a file path for a log file, based on its rolling period.
        /// </summary>
        /// <param name="directory">The base directory</param>
        /// <param name="fileName">The base file name</param>
        /// <param name="rollingPeriod">The rolling period</param>
        /// <returns></returns>
        internal static string DetermineFilePath(string directory, string fileName, RollingPeriod rollingPeriod)
        {
            var now = DateTime.Now;
            switch (rollingPeriod)
            {
                case RollingPeriod.Never:
                    return Path.Combine(directory, fileName);
                case RollingPeriod.Year:
                    return Path.Combine(directory, $"{now:yyyy}_{fileName}");
                case RollingPeriod.Month:
                    return Path.Combine(directory, $"{now:yyyy-MM}_{fileName}");
                case RollingPeriod.Day:
                    return Path.Combine(directory, $"{now:yyyy-MM-dd}_{fileName}");
                case RollingPeriod.Week:
                    return Path.Combine(directory, $"{now.WeekNumber()}_{fileName}");
                case RollingPeriod.Hour:
                    return Path.Combine(directory, $"{now:yyyy-MM-dd_HH}_{fileName}");
                case RollingPeriod.HalfHour:
                    return Path.Combine(directory, $"{now:yyyy-MM-dd_HH}-{now.HalfHourInterval()}_{fileName}");
                case RollingPeriod.QuarterHour:
                    return Path.Combine(directory, $"{now:yyyy-MM-dd_HH}-{now.QuarterHourInterval()}_{fileName}");
                case RollingPeriod.Minute:
                    return Path.Combine(directory, $"{now:yyyy-MM-dd_HH-mm}_{fileName}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(rollingPeriod), rollingPeriod, "Unknown rolling period.");
            }
        }

        private static int WeekNumber(this DateTime dateTime)
        {
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var calendarWeekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, calendarWeekRule, firstDayOfWeek);
        }

        private static string HalfHourInterval(this DateTime dateTime)
        {
            return dateTime.Minute < 30 ? "00" : "30";
        }

        private static string QuarterHourInterval(this DateTime dateTime)
        {
            if (dateTime.Minute < 15)
            {
                return "00";
            }

            if (dateTime.Minute < 30)
            {
                return "15";
            }

            if (dateTime.Minute < 45)
            {
                return "30";
            }

            return "45";
        }
    }
}
