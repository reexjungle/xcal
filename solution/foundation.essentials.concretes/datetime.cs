using System;

namespace reexmonkey.foundation.essentials.concretes
{
    public static class DateTimeExtensions
    {
        public static uint CountDays(this uint month, uint year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("month", "Non-valid value for month");
            if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException("year", "Non-valid value for year");

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
            if (startmonth > endmonth) throw new ArgumentException("The end month should be bigger or equal to the start month", "startmonth");
            if (startmonth < 1 || startmonth > 12) throw new ArgumentOutOfRangeException("startmonth", "Non-valid value for start month");
            if (endmonth < 1 || endmonth > 12) throw new ArgumentOutOfRangeException("endmonth", "Non-valid value for end month");
            if (year < 1 || year > 9999) throw new ArgumentOutOfRangeException("year", "Non-valid value for year");
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

        public static uint CountYears(this uint days)
        {
            return 1 + (days - (days / 365u).CountLeaps()) / 365u; 
        }

        public static DateTime LastMinute(this DateTime value)
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

        public static DateTime LastWeek(this  DateTime value)
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

        public static DateTime LastWeeks(this  DateTime value, int offset)
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

