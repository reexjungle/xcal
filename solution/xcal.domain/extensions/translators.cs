using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xmisc.foundation.concretes;
// ReSharper disable InconsistentNaming

namespace reexjungle.xcal.domain.extensions
{
    /// <summary>
    /// Helper class that provides support functionality related to the ICalendar interface
    /// </summary>
    public static class Translators
    {
        #region specialized enumeration translators for iCalendar

        /// <summary>
        /// Converts an EncodingType to its equivalent string representation
        /// </summary>
        /// <param name="type">The given input EncodingType value</param>
        /// <returns>The equivalent string representation of the EncodingType value</returns>
        /// <remarks>This function is similar to the ToString() with the only exception for returning the BIT8 value as 8BIT in its string representation </remarks>
        public static string ToStringRfc5545(this ENCODING type)
        {
            return type == ENCODING.BIT8 ? "8BIT" : ENCODING.BASE64.ToString();
        }

        #endregion specialized enumeration translators for iCalendar

        #region specialized date-time translators

        public static DATE_TIME ToDATE_TIME(this DateTime value, TimeZoneInfo tzinfo)
        {
            if (tzinfo != null)
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException();
                return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone);
            }
            return value.Kind == DateTimeKind.Utc 
                ? new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc) 
                : new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second);
        }

        public static IEnumerable<DATE_TIME> ToDATE_TIMEs(this IEnumerable<DateTime> values, TimeZoneInfo tzinfo)
        {
            if (values == null) throw new ArgumentNullException("values");
            return values.Select(x => x.ToDATE_TIME(tzinfo));
        }

        public static IEnumerable<DATE_TIME> ToDATE_TIMEs(this IEnumerable<DateTime> values, TZID tzid = null)
        {
            if (values == null) throw new ArgumentNullException("values");
            return values.Select(x => x.ToDATE_TIME(tzid));
        }

        public static DATE_TIME ToDATE_TIME(this DateTime value, TZID tzid = null)
        {
            if (value == default(DateTime)) return new DATE_TIME(default(DATE_TIME), tzid);
            if (tzid != null)
            {
                return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone, tzid);
            }
            return value.Kind == DateTimeKind.Utc 
                ? new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc) 
                : new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second);
        }

        public static DATE ToDATE(this DateTime value)
        {
            return value == default(DateTime) 
                ? default(DATE) 
                : new DATE((uint)value.Year, (uint)value.Month, (uint)value.Day);
        }

        public static IEnumerable<DATE> ToDATEs(this IEnumerable<DateTime> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            return values.Select(x => x.ToDATE());
        }


        public static TIME ToTIME(this DateTime value, TimeZoneInfo tzinfo)
        {
            if (value == default(DateTime)) return new TIME(default(TIME), tzinfo != null ? tzinfo.ToTZID() : null);
            if (tzinfo != null)
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException();
                return new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone);
            }
            return value.Kind == DateTimeKind.Utc 
                ? new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc) 
                : new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second);
        }

        public static TIME ToTIME(this DateTime value, TZID tzid = null)
        {
            if (value == default(DateTime)) return new TIME(default(TIME), tzid);
            if (tzid != null)
            {
                return new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone, tzid);
            }
            return value.Kind == DateTimeKind.Utc
                ? new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc)
                :  new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second);
        }


        public static IEnumerable<TIME> ToTIMEs(this IEnumerable<DateTime> values, TimeZoneInfo tzinfo)
        {
            if (values == null) throw new ArgumentNullException("values");
            return values.Select(x => x.ToTIME(tzinfo));
        }

        public static IEnumerable<TIME> ToTIMEs(this IEnumerable<DateTime> values, TZID tzid = null)
        {
            if (values == null) throw new ArgumentNullException("values");
            return values.Select(x => x.ToTIME(tzid));
        }

        public static TZID ToTZID(this TimeZoneInfo tzinfo)
        {
            return new TZID(string.Empty, tzinfo.Id);
        }

        public static DateTime ToDateTime(this DATE value)
        {
            return value == default(DATE) 
                ? default(DateTime) 
                : new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY);
        }

        public static DateTime ToDateTime(this DATE_TIME value)
        {
            if (value == default(DATE_TIME)) return default(DateTime);

            switch (value.Type)
            {
                case TimeType.Utc:
                    return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                        (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Utc);
                case TimeType.Local:
                case TimeType.LocalAndTimeZone:
                    return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                        (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Local);
            }
            return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Unspecified);
        }

        public static DateTime ToDateTime(this TIME value)
        {
            if (value == default(TIME)) return default(DateTime);
            switch (value.Type)
            {
               case TimeType.Utc:
                    return new DateTime(0001, 1, 1, (int)value.HOUR, (int)value.MINUTE, (int) value.SECOND, DateTimeKind.Utc);
                case TimeType.Local:
                case TimeType.LocalAndTimeZone:
                    return new DateTime(0001, 1, 1, (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Local);
            }

            return new DateTime(0001, 1, 1, (int) value.HOUR, (int) value.MINUTE, (int) value.SECOND,
                DateTimeKind.Unspecified);
        }


        public static TimeSpan ToTimeSpan(this DATE_TIME value)
        {
            return value == default(DATE_TIME) 
                ? new TimeSpan() 
                : new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
        }

        public static TimeSpan ToTimeSpan(this TIME value)
        {
            return value == default(TIME) 
                ? new TimeSpan() 
                : new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
        }

        public static DURATION ToDURATION(this TimeSpan span)
        {
            var days = span.Days;
            var hours = span.Hours;
            var minutes = span.Minutes;
            var seconds = span.Seconds;
            var weeks = span.Days
                + (span.Hours / 24)
                + (span.Minutes / (24 * 60))
                + (span.Seconds / (24 * 3600))
                + (span.Milliseconds / (24 * 3600000)) / 7;
            return new DURATION(weeks, days, hours, minutes, seconds);
        }

        public static TIME ToTIME(this TimeSpan span, TimeZoneInfo tzinfo = null, TimeType format = TimeType.NONE)
        {
            if (tzinfo != null)
            {
                if (format == TimeType.Utc) throw new ArgumentException();
                return format == TimeType.LocalAndTimeZone 
                    ? new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format, tzinfo.ToTZID()) 
                    : new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
            }
            return format == TimeType.Utc 
                ? new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format) 
                : new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
        }

        public static TIME ToTIME(this TimeSpan span, TZID tzid, TimeType format = TimeType.NONE)
        {
            if (tzid != null)
            {
                if (format == TimeType.Utc) throw new ArgumentException();
                return format == TimeType.LocalAndTimeZone 
                    ? new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format, tzid) 
                    : new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
            }
            return format == TimeType.Utc 
                ? new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format) 
                : new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
        }

        public static TimeSpan ToTimeSpan(this DURATION duration)
        {
            return new TimeSpan(duration.WEEKS * 7 + duration.DAYS, duration.HOURS, duration.MINUTES, duration.SECONDS);
        }

        public static DayOfWeek ToDayOfWeek(this WEEKDAY weekday)
        {
            switch (weekday)
            {
                case WEEKDAY.SU: return DayOfWeek.Sunday;
                case WEEKDAY.MO: return DayOfWeek.Monday;
                case WEEKDAY.TU: return DayOfWeek.Tuesday;
                case WEEKDAY.WE: return DayOfWeek.Wednesday;
                case WEEKDAY.TH: return DayOfWeek.Thursday;
                case WEEKDAY.FR: return DayOfWeek.Friday;
                case WEEKDAY.SA: return DayOfWeek.Saturday;
                default:
                    return DayOfWeek.Sunday;
            }
        }

        public static WEEKDAY ToWEEKDAY(this DayOfWeek dayofweek)
        {
            switch (dayofweek)
            {
                case DayOfWeek.Sunday: return WEEKDAY.SU;
                case DayOfWeek.Monday: return WEEKDAY.MO;
                case DayOfWeek.Tuesday: return WEEKDAY.TU;
                case DayOfWeek.Wednesday: return WEEKDAY.WE;
                case DayOfWeek.Thursday: return WEEKDAY.TH;
                case DayOfWeek.Friday: return WEEKDAY.FR;
                case DayOfWeek.Saturday: return WEEKDAY.SA;
                default: return WEEKDAY.NONE;
            }
        }

        #endregion specialized date-time translators
    }
}