using System;
using System.Linq;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using System.Collections.Generic;

namespace reexmonkey.xcal.domain.extensions
{
    /// <summary>
    /// Helper class that provides support functionality related to the ICalendar interface 
    /// </summary>
    public static class Translators
    {
        #region specialized enumeration translators for iCalendar

        /// <summary>
        /// Converts a given string value to an equivalent CalendarScale representation
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent CalendarScale value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to CalendarScale</exception>
        public static CALSCALE ToCALSCALE(this string value)
        {

            if (value.Equals("GREGORIAN", StringComparison.OrdinalIgnoreCase)) return CALSCALE.GREGORIAN;
            else if (value.Equals("CHINESE", StringComparison.OrdinalIgnoreCase)) return CALSCALE.CHINESE;
            else if (value.Equals("HEBREW", StringComparison.OrdinalIgnoreCase)) return CALSCALE.HEBREW;
            else if (value.Equals("INDIAN", StringComparison.OrdinalIgnoreCase)) return CALSCALE.INDIAN;
            else if (value.Equals("ISLAMIC", StringComparison.OrdinalIgnoreCase)) return CALSCALE.ISLAMIC;
            else if (value.Equals("JULIAN", StringComparison.OrdinalIgnoreCase)) return CALSCALE.JULIAN;
            else return CALSCALE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent EncodingType represention
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent CalendarScale value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to CalendarScale</exception>
        public static ENCODING ToENCODING(this string value)
        {
            if (value.Equals("BIT8", StringComparison.OrdinalIgnoreCase) || value.Equals("8BIT", StringComparison.OrdinalIgnoreCase)) return ENCODING.BIT8;
            else if (value.Equals("BASE64", StringComparison.OrdinalIgnoreCase)) return ENCODING.BASE64;
            else return ENCODING.UNKNOWN;
        }

        /// <summary>
        /// Converts an EncodingType to its equivalent string representation
        /// </summary>
        /// <param name="type">The given input EncodingType value</param>
        /// <returns>The equivalent string representation of the EncodingType value</returns>
        /// <remarks>This function is similar to the ToString() with the only exception for returning the BIT8 value as 8BIT in its string representation </remarks>
        public static string ToEncodingString(this ENCODING type)
        {
            if (type == ENCODING.BIT8) return "8BIT";
            else return ENCODING.BASE64.ToString();
        }

        /// <summary>
        /// Converts a given string value to an equivalent CalendarUserType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent CalendarUserType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="TException&lt;ICalendar&gt;"> Thrown when the string value cannot be converted to CalendarUserType</exception>
        public static CUTYPE ToCUTYPE(this string value)
        {
            if (value.Equals("GROUP", StringComparison.OrdinalIgnoreCase)) return CUTYPE.GROUP;
            else if (value.Equals("INDIVIDUAL", StringComparison.OrdinalIgnoreCase)) return CUTYPE.INDIVIDUAL;
            else if (value.Equals("RESOURCE", StringComparison.OrdinalIgnoreCase)) return CUTYPE.RESOURCE;
            else if (value.Equals("ROOM", StringComparison.OrdinalIgnoreCase)) return CUTYPE.ROOM;
            else return CUTYPE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RoleType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent ToleType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RoleType</exception>
        public static ROLE ToROLE(this string value)
        {
            if (value.Equals("CHAIR", StringComparison.OrdinalIgnoreCase)) return ROLE.CHAIR;
            else if (value.Equals("NON_PARTICIPANT", StringComparison.OrdinalIgnoreCase)) return ROLE.NON_PARTICIPANT;
            else if (value.Equals("OPT_PARTICIPANT", StringComparison.OrdinalIgnoreCase)) return ROLE.OPT_PARTICIPANT;
            else if (value.Equals("REQ_PARTICIPANT", StringComparison.OrdinalIgnoreCase)) return ROLE.REQ_PARTICIPANT;
            else return ROLE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent ParticipationStatusType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent Participation value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to ParticipationStatusType</exception>
        public static PARTSTAT ToPARTSTAT(this string value)
        {
            if (value.Equals(PARTSTAT.ACCEPTED.ToString())) return PARTSTAT.ACCEPTED;
            else if (value.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.COMPLETED;
            else if (value.Equals("DECLINED", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.DELEGATED;
            else if (value.Equals("DELEGATED", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.DELEGATED;
            else if (value.Equals("IN_PROGRESS", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.IN_PROGRESS;
            else if (value.Equals("NEEDS_ACTION", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.UNKNOWN;
            else if (value.Equals("TENTATIVE", StringComparison.OrdinalIgnoreCase)) return PARTSTAT.TENTATIVE;
            else return PARTSTAT.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RangeType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent RangeType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RangeType</exception>
        public static RANGE ToRANGE(this string value)
        {
            if (value.Equals("THISANDFUTURE", StringComparison.OrdinalIgnoreCase)) return RANGE.THISANDFUTURE;
            else if (value.Equals("THIS", StringComparison.OrdinalIgnoreCase)) return RANGE.THIS;
            else if (value.Equals("ALL", StringComparison.OrdinalIgnoreCase)) return RANGE.ALL;
            else return RANGE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent FrequencyType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent FrequencyType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to FrequencyType</exception>
        public static FREQ ToFREQ(this string value)
        {
            if (value.Equals("DAILY", StringComparison.OrdinalIgnoreCase)) return FREQ.DAILY;
            else if (value.Equals("HOURLY", StringComparison.OrdinalIgnoreCase)) return FREQ.HOURLY;
            else if (value.Equals("MINUTELY", StringComparison.OrdinalIgnoreCase)) return FREQ.MINUTELY;
            else if (value.Equals("MONTHLY", StringComparison.OrdinalIgnoreCase)) return FREQ.MONTHLY;
            else if (value.Equals("SECONDLY", StringComparison.OrdinalIgnoreCase)) return FREQ.SECONDLY;
            else if (value.Equals("WEEKLY", StringComparison.OrdinalIgnoreCase)) return FREQ.WEEKLY;
            else if (value.Equals("YEARLY", StringComparison.OrdinalIgnoreCase)) return FREQ.YEARLY;
            else return FREQ.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent WeekDayType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent WeekDayType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to WeekDayType</exception>
        public static WEEKDAY ToWEEKDAY(this string value)
        {
            if (value.Equals("SU", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.SU;
            else if (value.Equals("MO", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.MO;
            else if (value.Equals("TU", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.TU;
            else if (value.Equals("WE", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.WE;
            else if (value.Equals("TH", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.TH;
            else if (value.Equals("FR", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.FR;
            else if (value.Equals("SA", StringComparison.OrdinalIgnoreCase)) return WEEKDAY.SA;
            else return WEEKDAY.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RelationshipType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent RelationshipType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RelationshipType</exception>
        public static RELTYPE ToRELTYPE(this string value)
        {
            if (value.Equals("CHILD", StringComparison.OrdinalIgnoreCase)) return RELTYPE.CHILD;
            else if (value.Equals("PARENT", StringComparison.OrdinalIgnoreCase)) return RELTYPE.PARENT;
            else if (value.Equals("SIBLING", StringComparison.OrdinalIgnoreCase)) return RELTYPE.SIBLING;
            return RELTYPE.UNKNOWN;

        }

        public static BOOLEAN ToBOOLEAN(this string value)
        {
            if (value.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return BOOLEAN.TRUE;
            else if (value.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return BOOLEAN.FALSE;
            return BOOLEAN.UNKNOWN;
        }

        public static BOOLEAN ToBOOLEAN(this bool value)
        {
            if (value == true) return BOOLEAN.TRUE;
            else return BOOLEAN.FALSE;
        }

        #endregion

        #region  specialized date-time translators

        public static DATE_TIME ToDATE_TIME(this DateTime value, TimeZoneInfo tzinfo = null)
        {
            if(tzinfo != null)
            {
                if(value.Kind == DateTimeKind.Utc) throw new ArgumentException();
                else return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone);
            }
            else
            {
                if(value.Kind == DateTimeKind.Utc) return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc);
                else  return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Local);
            }
        }

        public static IEnumerable<DATE_TIME> ToDATE_TIMEs(this IEnumerable<DateTime> values, TimeZoneInfo tzinfo = null)
        {
            return values.Select(x => x.ToDATE_TIME(tzinfo));
        }

        public static IEnumerable<DATE_TIME> ToDATE_TIMEs(this IEnumerable<DateTime> values, TZID tzid = null)
        {
            return values.Select(x => x.ToDATE_TIME(tzid));
        }

        public static DATE_TIME ToDATE_TIME(this DateTime value, TZID tzid)
        {
            if (tzid != null)
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException("UTC Date Time not compatible with Local Time with Time Zone Id");
                else return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone, tzid);            }
            else
            {
                if (value.Kind == DateTimeKind.Utc) return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc);
                else return new DATE_TIME((uint)value.Year, (uint)value.Month, (uint)value.Day, (uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Local);
            }
        }


        public static  DATE  ToDATE(this DateTime value)
        {
            return new DATE((uint)value.Year, (uint)value.Month, (uint)value.Day);
        }

        public static TIME ToTIME(this DateTime value, TimeZoneInfo tzinfo = null)
        {
            if (tzinfo != null)
            {
                if (value.Kind == DateTimeKind.Utc) throw new ArgumentException();
                else  return new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.LocalAndTimeZone);
            }
            else
            {
                if (value.Kind == DateTimeKind.Utc) return new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Utc);
                else return new TIME((uint)value.Hour, (uint)value.Minute, (uint)value.Second, TimeType.Local);
            }
        }

        public static TZID ToTZID(this TimeZoneInfo tzinfo)
        {
            return new TZID(string.Empty, tzinfo.Id);
        }

        public static DateTime ToDateTime(this DATE value)
        {
            if (value == default(DATE)) return default(DateTime);

            if (value == null) return new DateTime();
            return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY);
        }

        public static DateTime ToDateTime(this DATE_TIME value)
        {
            if (value == default(DATE_TIME)) return default(DateTime);

            if (value.Type == TimeType.Utc)
            {
                return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                    (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Utc);
            }
            else if (value.Type == TimeType.Local || value.Type == TimeType.LocalAndTimeZone)
            {
                return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                    (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Local);
            }
            else
            {
                return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
                    (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Unspecified);
            }
        }

        public static  TimeSpan ToTimeSpan(this DATE_TIME value)
        {
            if (value == default(DATE_TIME)) return new TimeSpan();
            return new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
        }

        public static TimeSpan ToTimeSpan(this TIME value)
        {
            if (value == default(TIME)) return new TimeSpan();
            return new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
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

        public static TIME ToTIME(this TimeSpan span, TimeZoneInfo tzinfo = null, TimeType format = TimeType.Unknown)
        {
           if (tzinfo != null)
           {
               if (format == TimeType.Utc) throw new ArgumentException();
               else if (format == TimeType.LocalAndTimeZone) return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format, tzinfo.ToTZID());
               else return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
           }
           else
           {
               if (format == TimeType.Utc) return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format);
               else return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
           }
        }

        public static TIME ToTIME(this TimeSpan span, TZID tzid, TimeType format = TimeType.Unknown)
        {
            if (tzid != null)
            {
                if (format == TimeType.Utc) throw new ArgumentException();
                else if (format == TimeType.LocalAndTimeZone) return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format, tzid);
                else return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
            }
            else
            {
                if (format == TimeType.Utc) return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds, format);
                else return new TIME((uint)span.Hours, (uint)span.Minutes, (uint)span.Seconds);
            }
        }

        public static TimeSpan ToTimeSpan(this DURATION duration)
        {
            return new TimeSpan(duration.WEEKS * 7 + duration.DAYS, duration.HOURS, duration.MINUTES, duration.SECONDS);
        }

        public static DayOfWeek ToDayOfWeek(this WEEKDAY weekday)
        {
            switch(weekday)
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

        public static WEEKDAY ToWEEKDAY (this DayOfWeek dayofweek)
        {
            switch(dayofweek)
            {
                case DayOfWeek.Sunday: return WEEKDAY.SU;
                case DayOfWeek.Monday: return WEEKDAY.MO;
                case DayOfWeek.Tuesday: return WEEKDAY.TU;
                case DayOfWeek.Wednesday: return WEEKDAY.WE;
                case DayOfWeek.Thursday: return WEEKDAY.TH;
                case DayOfWeek.Friday: return WEEKDAY.FR;
                case DayOfWeek.Saturday: return WEEKDAY.SA;
                default:
                    return WEEKDAY.UNKNOWN;
            }
        }


        #endregion

    }
}
