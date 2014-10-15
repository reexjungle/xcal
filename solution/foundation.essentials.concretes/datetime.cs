using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace reexmonkey.foundation.essentials.concretes
{
    public static class DateTimeExtensions
    {
        public static uint CountDays(this uint month, uint year)
        {
            //if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("month", "Non-valid value for month");
            //if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException("year", "Non-valid value for year");

            switch (month)
            {
                case 1: return 31u;
                case 2: return year.IsLeapYear() ? 29u : 28u;
                case 3: return 31u;
                case 4: return 30u;
                case 5: return 31u;
                case 6: return 30u;
                case 7: return 31u;
                case 8: return 31u;
                case 9: return 30u;
                case 10: return 31u;
                case 11: return 30u;
                case 12: return 31u;
                default: return 28u;
            }
        }

        public static uint CountDays(this uint startmonth, uint endmonth, uint year)
        {
            //if (startmonth > endmonth) throw new ArgumentException("The end month should be bigger or equal to the start month", "startmonth");
            //if (startmonth < 1 || startmonth > 12) throw new ArgumentOutOfRangeException("startmonth", "Non-valid value for start month");
            //if (endmonth < 1 || endmonth > 12) throw new ArgumentOutOfRangeException("endmonth", "Non-valid value for end month");
            //if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException("year", "Non-valid value for year");
            var days = 0u;
            for (uint m = startmonth; m < endmonth; m++) days += m.CountDays(year);
            return days;
        }

        public static bool IsLeapYear(this uint year)
        {
            return (year % 4 == 0 && year % 100 != 0 && year % 400 == 0);
        }

        public static uint CountYearDays(this uint year)
        {
            return (year.IsLeapYear()) ? 366u : 365u;
        }

        public static uint CountLeaps(this uint years)
        { 
            return ((years - 1) / 4 - (years - 1)/ 100 + (years - 1)/400 );
        }

        public static uint CountDays(this uint years)
        {
            return (years - 1) * 365u + years.CountLeaps();
        }

        public static int CountMonths(this DateTime start, DateTime end)
        {
            return ((start.Year - end.Year) * 12) + start.Month - end.Month;
        }

        public static uint CountYears(this uint days)
        {
            return 1 + (days - (days / 365u).CountLeaps()) / 365u; 
        }

        public static int OrdinalWeekDay(this DayOfWeek day)
        {
            switch(day)
            {
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
                case DayOfWeek.Sunday: return 7;
                default: return 0;
            }
        }

        public static DayOfWeek ToDayofWeeek(this int ordweekday)
        {
            switch(ordweekday)
            {
                case 1: return DayOfWeek.Monday;
                case 2: return DayOfWeek.Tuesday;
                case 3: return DayOfWeek.Wednesday;
                case 4: return DayOfWeek.Thursday;
                case 5: return DayOfWeek.Friday;
                case 6: return DayOfWeek.Saturday;
                case 7: return DayOfWeek.Sunday;
                default:
                    throw new ArgumentException("Ordinal day of week must range from 1 through 7");
            }


        }

        public static int ISo8601DayOfYear(this DateTime value)
        {
            Func<DateTime, int> lookup = x =>
                {
                    if (DateTime.IsLeapYear(x.Year))
                    {
                        switch(x.Month)
                        {
                            case 1: return 0;
                            case 2: return 31;
                            case 3: return 60;
                            case 4: return 91;
                            case 5: return 121;
                            case 6: return 152;
                            case 7: return 182;
                            case 8: return 213;
                            case 9: return 224;
                            case 10: return 274;
                            case 11: return 305;
                            case 12: return 335;
                            default:
                                throw new ArgumentException("Invalid Month");
                        }
                    }
                    else
                    {
                        switch (x.Month)
                        {
                            case 1: return 0;
                            case 2: return 30;
                            case 3: return 59;
                            case 4: return 90;
                            case 5: return 120;
                            case 6: return 151;
                            case 7: return 181;
                            case 8: return 212;
                            case 9: return 223;
                            case 10: return 273;
                            case 11: return 304;
                            case 12: return 334;
                            default:
                                throw new ArgumentException("Invalid Month");

                        }

                    }

                };

            return value.Day + lookup(value);

        }

        public static bool InFirstWeek(this DateTime value)
        {
            var first = new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind);
            var fdays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday };
            return fdays.Contains(first.DayOfWeek);
        }

        public static bool InLastWeek(this DateTime value)
        {
            var last = new DateTime(value.Year, 12, 31, 0, 0, 0, value.Kind);
            var ldays = new DayOfWeek[] { DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            return ldays.Contains(last.DayOfWeek);
        }

        /// <summary>
        /// Gets the week number in a year for a given date time.
        /// </summary>
        /// <param name="value">The date time value</param>
        /// <param name="rule">The rule to determine the first day of the week</param>
        /// <param name="first">The day designated as the start of the week</param>
        /// <returns>The week number in a year of a date time</returns>
        /// <see cref="http://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date"/>
        public static int WeekOfYear(this DateTime value, 
            CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek, 
            DayOfWeek first = DayOfWeek.Monday)
        {
            var day = value.DayOfWeek;
            //adjust
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) value.AddDays(3);
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(value, rule, first);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rule"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/2136487/calculate-week-of-month-in-net/2136549#2136549"/>
        public static int WeekOfMonth(this DateTime value, CalendarWeekRule rule = CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek first = DayOfWeek.Monday)
        {

            var firstday = new DateTime(value.Year, value.Month, 1);
            return value.WeekOfYear() - firstday.WeekOfYear() + 1;
        }

        public static bool IsNthWeekDayofMonth(this DateTime date, DayOfWeek weekday, int N)
        {
            if (N > 0)
            {
                var first = new DateTime(date.Year, date.Month, 1);
                return (date.Day - first.Day)/ 7 == N - 1 && date.DayOfWeek == weekday;
            }
            else
            {

                var last = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
                return (last.Day - date.Day) / 7 == (Math.Abs(N) - 1) && date.DayOfWeek == weekday;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="weekday"></param>
        /// <param name="year"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/Tips/437883/Get-the-Nth-Instance-of-a-Weekday-in-the-Specified"/>
        public static DateTime NthWeekDayOfYear(this DateTime date, DayOfWeek weekday, int year, int N)
        {
            if(DateTime.IsLeapYear(year))            
                N = (N >= 0)
                ? Math.Min(Math.Max(N, 1), 366)
                : Math.Min(367 - Math.Abs(N), 366);
            else
                N = (N >= 0)
                ? Math.Min(Math.Max(N, 1), 365)
                : Math.Min(366 - Math.Abs(N), 365);

            var instance = new DateTime(year, 1, 1); //first day of first month of given year
            var last = new DateTime(year, 12, 31); // last day of last month of given year

            var max = (last - instance).Days + 1;

            var interval = 0;
            if (instance.DayOfWeek != weekday)
            {

                // determine the number of days between the first of the year and 
                // the first instance of the specified day of week
                interval = instance.DayOfWeek - weekday;
                interval = (interval < 0) ? Math.Abs(interval) : 7 - interval;
                instance = instance.AddDays(interval);
            }

            if (N > 1)
            {
                int offset = 7 * (N - 1);
                while (offset + interval > max - 1) offset -= 7;
                instance = instance.AddDays(offset);
            }

            return instance;
        }


        public static DateTime PreviousMinute(this DateTime value)
        {
            return value.AddMinutes(-1);
        }

        public static DateTime NextMinute(this DateTime value)
        {
            return value.AddMinutes(1);
        }

        public static DateTime NextMinutes(this DateTime value, int offset)
        {
            return value.AddMinutes(Math.Abs(offset));
        }

        public static DateTime LastMinutes(this DateTime value, int offset)
        {
            return value.AddMinutes(-Math.Abs(offset));
        }

        public static DateTime LastSecond(this DateTime value)
        {
            return value.AddSeconds(-1);
        }

        public static DateTime NextSecond(this DateTime value)
        {
            return value.AddSeconds(1);
        }

        public static DateTime NextSeconds(this DateTime value, int offset)
        {
            return value.AddSeconds(offset);
        }

        public static DateTime LastSeconds(this DateTime value, int offset)
        {
            return value.AddSeconds(-Math.Abs(offset));
        }

        public static DateTime LastHour(this DateTime value)
        {
            return value.AddHours(-1);
        }

        public static DateTime NextHour(this DateTime value)
        {
            return value.AddHours(1);
        }

        public static DateTime NextHours(this DateTime value, int offset)
        {
            return value.AddHours(Math.Abs(offset));
        } 

        public static DateTime LastHours(this DateTime value, int offset)
        {
            return value.AddHours(-Math.Abs(offset));
        } 
      
        public static DateTime Yesterday(this DateTime value)
        {
            return value.Subtract(new TimeSpan(0, 1, 0, 0, 0));
        }

        public static DateTime Tomorrow(this DateTime value)
        {
            return value.AddDays(1);
        }

        public static DateTime NextDays(this DateTime value, int offset)
        {
            return value.AddDays(Math.Abs(offset));
        }

        public static DateTime LastDays(this DateTime value, int offset)
        {
            return value.AddDays(-Math.Abs(offset));
        }

        public static DateTime PreviousWeek(this  DateTime value)
        {
            return value.AddDays(-7);
        }

        public static DateTime NextWeek(this DateTime value)
        {
            return value.AddDays(7);
        }

        public static DateTime NextWeeks(this  DateTime value, int offset)
        {
            return value.AddDays(7 * offset);
        }

        public static DateTime PreviousWeeks(this  DateTime value, int offset)
        {
            return value.AddDays(-7 * Math.Abs(offset));
        }

        public static DateTime LastMonth(this DateTime value)
        {
            return value.AddMonths(-1);
        }

        public static DateTime NextMonth(this DateTime value)
        {
            return value.AddMonths(1);
        }

        public static DateTime NextMonths(this DateTime value, int offset)
        {
            return value.AddMonths(Math.Abs(offset));
        }

        public static DateTime LastMonths(this DateTime value, int offset)
        {
            return value.AddMonths(-Math.Abs(offset));
        }

        public static DateTime LastYear(this DateTime value)
        {
            return value.AddYears(-1);
        }

        public static DateTime NextYear(this DateTime value)
        {
            return value.AddYears(1);
        }

        public static bool IsDefault(this DateTime value)
        {
            return (value == new DateTime());
        }

    }

}

