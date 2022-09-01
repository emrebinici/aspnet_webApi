using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlowDMApi.Core.Extentions
{
    public class TimespanInfo
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
    }

    public static class DateTimeExtensions
    {
        public static string ToDateString(this DateTime? date)
        {
            return date.HasValue ? ToDateString(date.Value) : "";
        }

        public static string ToDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToTimeString(this TimeSpan? time)
        {
            return time.HasValue ? ToTimeString(time.Value) : "";
        }
        public static string ToTimeString(this TimeSpan time)
        {
            return time.ToString();
        }

        public static int GetTotalYears(this DateTime pastDate, DateTime currentDate)
        {
            //
            const int p = 60*24*365;
            return (int) Math.Floor(currentDate.Subtract(pastDate).TotalMinutes/p);
        }

        public static TimespanInfo GetRealDiff(this DateTime? pastDate, DateTime? currentDate = null)
        {
            if (pastDate.HasValue)
            {
                return GetRealDiff(pastDate.Value, currentDate);
            }
            return new TimespanInfo();
        }

        public static TimespanInfo GetRealDiff(this DateTime pastDate, DateTime? currentDate = null)
        {
            if (!currentDate.HasValue)
            {
                currentDate = DateTime.Now;
            }
            var result = new TimespanInfo();
            result.Years = currentDate.Value.Year - pastDate.Year;

            if (currentDate.Value < pastDate.AddYears(result.Years))
            {
                result.Years --;
            }

            Func<DateTime, int, int, DateTime> addRealMonthsAndDays = (current, elapsedMonths, elapsedDays) =>
            {
                var day = current.Day + elapsedDays;
                if (day >= 31) day = 31;
                var month = current.Month + elapsedMonths;
                var year = current.Year;

                if (month > 12)
                {
                    month = month-12;
                    year++;
                }

                try
                {
                    var newDate = new DateTime(year, month, day);
                    return newDate;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.WriteLine(ex.Message);
                    day = 1;
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                    var newDate = new DateTime(year, month, day);
                    return newDate;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            };

            var loop = 0;
            var currentYearDate = pastDate.AddYears(result.Years);
            while (addRealMonthsAndDays(currentYearDate, result.Months + 1, 0) <= currentDate.Value && loop < 100)
            {
                result.Months++;
                loop++;
            }

            var currentMonthDate = currentYearDate.AddMonths(result.Months);
            while (addRealMonthsAndDays(currentMonthDate, 0, result.Days + 1) <= currentDate.Value && loop < 100)
            {
                result.Days++;
                loop++;
            }

            return result;
        }

        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday
                && date.DayOfWeek != DayOfWeek.Sunday;
        }

        public static List<DateTime> GetWorkingDaysBetweenDates(this DateTime firstDate, DateTime lastDate, params DateTime[] holidays)
        {
            var days = new List<DateTime>();
            var difference = lastDate.Subtract(firstDate).TotalDays;
            for (int i = 0; i < difference; i++)
            {
                var tmp = firstDate.AddDays(i);
                if ((holidays == null || !holidays.Contains(tmp)) && tmp.IsWorkingDay()) days.Add(tmp);
            }
            return days;
        }
        public static int GetTotalWorkingDaysBetweenDates(this DateTime firstDate, DateTime lastDate, params DateTime[] holidays)
        {
            var days = 0;
            var difference = lastDate.Subtract(firstDate).TotalDays;
            for (int i = 0; i < difference; i++)
            {
                var tmp = firstDate.AddDays(i);
                if ((holidays == null || !holidays.Contains(tmp)) && tmp.IsWorkingDay()) days++;
            }
            return days;
        }
    }
}
