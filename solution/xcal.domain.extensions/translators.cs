using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.domain.extensions
{
    /// <summary>
    /// Helper class that provides support functionality related to the ICalendar interface 
    /// </summary>
    public static class TranslateExtensions
    {
        #region specialized enumeration translators for iCalendar

        /// <summary>
        /// Converts a given string value to an equivalent CalendarScale representation
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent CalendarScale value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to CalendarScale</exception>
        public static CALSCALE TranslateToCALSCALE(this string value)
        {
            
            if (value.ToUpper().Equals("GREGORIAN")) return CALSCALE.GREGORIAN;
            else if (value.ToUpper().Equals("CHINESE")) return CALSCALE.CHINESE;
            else if (value.ToUpper().Equals("HEBREW")) return CALSCALE.HEBREW;
            else if (value.ToUpper().Equals("INDIAN")) return CALSCALE.INDIAN;
            else if (value.ToUpper().Equals("ISLAMIC")) return CALSCALE.ISLAMIC;
            else if (value.ToUpper().Equals("JULIAN")) return CALSCALE.JULIAN;
            else return CALSCALE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent EncodingType represention
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent CalendarScale value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to CalendarScale</exception>
        public static ENCODING TranslateToENCODING(this string value)
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
        public static string TranslateToString(this ENCODING type)
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
        public static CUTYPE TranslateToCUTYPE(this string value)
        {
            if (value.ToUpper().Equals("GROUP")) return CUTYPE.GROUP;
            else if (value.ToUpper().Equals("INDIVIDUAL")) return CUTYPE.INDIVIDUAL;
            else if (value.ToUpper().Equals("RESOURCE")) return CUTYPE.RESOURCE;
            else if (value.ToUpper().Equals("ROOM")) return CUTYPE.ROOM;
            else return CUTYPE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RoleType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent ToleType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RoleType</exception>
        public static ROLE TranslateToROLE(this string value)
        {
            if (value.ToUpper().Equals("CHAIR")) return ROLE.CHAIR;
            else if (value.ToUpper().Equals("NON_PARTICIPANT")) return ROLE.NON_PARTICIPANT;
            else if (value.ToUpper().Equals("OPT_PARTICIPANT")) return ROLE.OPT_PARTICIPANT;
            else if (value.ToUpper().Equals("REQ_PARTICIPANT")) return ROLE.REQ_PARTICIPANT;
            else return ROLE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent ParticipationStatusType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent Participation value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to ParticipationStatusType</exception>
        public static PARTSTAT TranslateToPARTSTAT(this string value)
        {
            if (value.ToUpper().Equals(PARTSTAT.ACCEPTED.ToString())) return PARTSTAT.ACCEPTED;
            else if (value.ToUpper().Equals("COMPLETED.ToString")) return PARTSTAT.COMPLETED;
            else if (value.ToUpper().Equals("DECLINED")) return PARTSTAT.DELEGATED;
            else if (value.ToUpper().Equals("DELEGATED")) return PARTSTAT.DELEGATED;
            else if (value.ToUpper().Equals("IN_PROGRESS")) return PARTSTAT.IN_PROGRESS;
            else if (value.ToUpper().Equals("NEEDS_ACTION")) return PARTSTAT.UNKNOWN;
            else if (value.ToUpper().Equals("TENTATIVE")) return PARTSTAT.TENTATIVE;
            else return PARTSTAT.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RangeType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent RangeType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RangeType</exception>
        public static RANGE TranslateToRANGE(this string value)
        {
            if (value.ToUpper().Equals("THISANDFUTURE")) return RANGE.THISANDFUTURE;
            else if(value.ToUpper().Equals("THIS")) return RANGE.THIS;
            else if(value.ToUpper().Equals("ALL")) return RANGE.ALL;
            else return RANGE.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent FrequencyType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent FrequencyType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to FrequencyType</exception>
        public static FREQ TranslateToFREQ(this string value)
        {
            if (value.ToUpper().Equals("DAILY")) return FREQ.DAILY;
            else if (value.ToUpper().Equals("HOURLY")) return FREQ.HOURLY;
            else if (value.ToUpper().Equals("MINUTELY")) return FREQ.MINUTELY;
            else if (value.ToUpper().Equals("MONTHLY")) return FREQ.MONTHLY;
            else if (value.ToUpper().Equals("SECONDLY")) return FREQ.SECONDLY;
            else if (value.ToUpper().Equals("WEEKLY")) return FREQ.WEEKLY;
            else if (value.ToUpper().Equals("YEARLY")) return FREQ.YEARLY;
            else return FREQ.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent WeekDayType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent WeekDayType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to WeekDayType</exception>
        public static WEEKDAY TranslateToWEEKDAY(this string value)
        {
            if (value.ToUpper().Equals("SU")) return WEEKDAY.SU;
            else if (value.ToUpper().Equals("MO")) return WEEKDAY.MO;
            else if (value.ToUpper().Equals("TU")) return WEEKDAY.TU;
            else if (value.ToUpper().Equals("WE")) return WEEKDAY.WE;
            else if (value.ToUpper().Equals("TH")) return WEEKDAY.TH;
            else if (value.ToUpper().Equals("FR")) return WEEKDAY.FR;
            else if (value.ToUpper().Equals("SA")) return WEEKDAY.SA;
            else return WEEKDAY.UNKNOWN;
        }

        /// <summary>
        /// Converts a given string value to an equivalent RelationshipType representation.
        /// </summary>
        /// <param name="value">The given input string value</param>
        /// <returns>An equivalent RelationshipType value if the conversion succeeds, otherwise an exception is thrown</returns>
        /// <exception cref="ArgumentException"> Thrown when the string value cannot be converted to RelationshipType</exception>
        public static RELTYPE TranslateToRELTYPE(this string value)
        {
            if (value.ToUpper().Equals("CHILD")) return RELTYPE.CHILD;
            else if (value.ToUpper().Equals("PARENT")) return RELTYPE.PARENT;
            else if (value.ToUpper().Equals("SIBLING")) return RELTYPE.SIBLING;
            else return RELTYPE.UNKNOWN;

        }

        #endregion

        #region specialized type converters for iCalendar Value types

    //    #region  date-time translators

    //    /// <summary>
    //    /// Converts an IDATE_TIME entity to its IDATE equivalent
    //    /// </summary>
    //    /// <typeparam name="T">The type of IDATE entity</typeparam>
    //    /// <param name="value">The IDATE_TIME entity to be converted</param>
    //    /// <returns>The equuivalent IDATE entity</returns>
    //    public static T To_IDATE<T>(this IDATE_TIME value)
    //        where T : IDATE, new()
    //    {
    //        if (value == null) return default(T);
    //        var date = Activator.CreateInstance<T>();
    //        date.FULLYEAR = value.FULLYEAR;
    //        date.MONTH = value.MONTH;
    //        date.MDAY = value.MDAY;
    //        return date;
    //    }

    //    /// <summary>
    //    /// Converts an IDATE entity to its IDATE_TIME equivalent
    //    /// </summary>
    //    /// <typeparam name="T">The type of IDATE_TIME entity</typeparam>
    //    /// <param name="value">The IDATE entity to be converted</param>
    //    /// <param name="tzid">An optional Time Zone ID for the conversion</param>
    //    /// <returns>The equivalent IDATE_TIME entity</returns>
    //    public static T To_IDATE_TIME<T>(this IDATE value, ITZID tzid = null)
    //        where T : IDATE_TIME, new()
    //    {
    //        if (value == null) return default(T);
    //        var datetime = Activator.CreateInstance<T>();
    //        datetime.FULLYEAR = value.FULLYEAR;
    //        datetime.MONTH = value.MONTH;
    //        datetime.MDAY = value.MDAY;
    //        datetime.TimeZoneId = tzid;
    //        return datetime;
    //    }

    //    /// <summary>
    //    /// Converts an IDATE_TIME entity to its ITIME equivakebt
    //    /// </summary>
    //    /// <typeparam name="T">The type of ITIME entity</typeparam>
    //    /// <param name="value">The IDATE_TIME entity to be converted</param>
    //    /// <returns>The equivalent ITIME entity</returns>
    //    public static T To_ITIME<T>(this IDATE_TIME value)
    //        where T : ITIME, new()
    //    {
    //        if (value == null) return default(T);
    //        var time = Activator.CreateInstance<T>();
    //        time.HOUR = value.HOUR;
    //        time.MINUTE = value.MINUTE;
    //        time.SECOND = value.SECOND;
    //        time.TimeZoneId = value.TimeZoneId;
    //        return time;
    //    }

    //    /// <summary>
    //    /// Converts an ITIME entity to its IDATE_TIME equivalent
    //    /// </summary>
    //    /// <typeparam name="T">The type of IDATE_TIME entity</typeparam>
    //    /// <param name="value">The ITIME entity to be converted</param>
    //    /// <returns>The equivalent IDATE_TIME entity</returns>
    //    public static T To_IDATE_TIME<T>(this ITIME value)
    //        where T : IDATE_TIME, new()
    //    {
    //        if (value == null) return default(T);
    //        var datetime = Activator.CreateInstance<T>();
    //        datetime.HOUR = value.HOUR;
    //        datetime.MINUTE = value.MINUTE;
    //        datetime.SECOND = value.SECOND;
    //        datetime.TimeZoneId = value.TimeZoneId;
    //        return datetime;
    //    }

    //    public static T To_IDATE<T>(this DateTime value)
    //where T : IDATE, new()
    //    {
    //        var date = Activator.CreateInstance<T>();
    //        date.FULLYEAR = (uint)value.Year;
    //        date.MONTH = (uint)value.Month;
    //        date.MDAY = (uint)value.Day;
    //        return date;
    //    }

    //    public static T To_IDATE_TIME<T>(this DateTime value)
    //        where T : IDATE_TIME, new()
    //    {
    //        if (value == null) return default(T);
    //        var datetime = Activator.CreateInstance<T>();
    //        datetime.FULLYEAR = (uint)value.Year;
    //        datetime.MONTH = (uint)value.Month;
    //        datetime.MDAY = (uint)value.Day;
    //        datetime.HOUR = (uint)value.Hour;
    //        datetime.MINUTE = (uint)value.Minute;
    //        datetime.SECOND = (uint)value.Second;
    //        if (value.Kind == DateTimeKind.Utc) datetime.TimeFormat = TimeFormat.Utc;
    //        else if (value.Kind == DateTimeKind.Local) datetime.TimeFormat = TimeFormat.Local;
    //        else datetime.TimeFormat = TimeFormat.Unknown; 
    //        return datetime;
    //    }

    //    public static T To_IDATE_TIME<T, U>(this DateTime value, TimeZoneInfo tzinfo)
    //        where T : IDATE_TIME, new()
    //        where U : ITZID, new()
    //    {
    //        if (value == null) return default(T);
    //        var datetime = Activator.CreateInstance<T>();
    //        datetime.FULLYEAR = (uint)value.Year;
    //        datetime.MONTH = (uint)value.Month;
    //        datetime.MDAY = (uint)value.Day;
    //        datetime.HOUR = (uint)value.Hour;
    //        datetime.MINUTE = (uint)value.Minute;
    //        datetime.SECOND = (uint)value.Second;
    //        if (tzinfo != null)
    //        {
    //            datetime.TimeZoneId = tzinfo.To_ITZID<U>();
    //            datetime.TimeFormat = TimeFormat.LocalAndTimeZone;
    //        }
    //        else
    //        {
    //            if (value.Kind == DateTimeKind.Utc) datetime.TimeFormat = TimeFormat.Utc;
    //            else if (value.Kind == DateTimeKind.Local) datetime.TimeFormat = TimeFormat.Local;
    //            else datetime.TimeFormat = TimeFormat.Unknown;
    //        }
    //        return datetime;
    //    }

    //    public static T To_ITIME<T>(this DateTime value)
    //        where T : ITIME, new()
    //    {
    //        if (value == null) return default(T);
    //        var time = Activator.CreateInstance<T>();
    //        time.HOUR = (uint)value.Hour;
    //        time.MINUTE = (uint)value.Minute;
    //        time.SECOND = (uint)value.Second;
    //        if (value.Kind == DateTimeKind.Utc) time.TimeFormat = TimeFormat.Utc;
    //        else if (value.Kind == DateTimeKind.Local) time.TimeFormat = TimeFormat.Local;
    //        else time.TimeFormat = TimeFormat.Unknown; 
    //        return time;
    //    }

    //    public static T To_ITIME<T, U>(this DateTime value, TimeZoneInfo tzinfo)
    //        where T : ITIME, new()
    //        where U : ITZID, new()
    //    {
    //        if (value == null) return default(T);
    //        var time = Activator.CreateInstance<T>();
    //        time.HOUR = (uint)value.Hour;
    //        time.MINUTE = (uint)value.Minute;
    //        time.SECOND = (uint)value.Second;
    //        if (tzinfo != null)
    //        {
    //            time.TimeZoneId = tzinfo.To_ITZID<U>();
    //            time.TimeFormat = TimeFormat.LocalAndTimeZone;
    //        }
    //        else
    //        {
    //            if (value.Kind == DateTimeKind.Utc) time.TimeFormat = TimeFormat.Utc;
    //            else if (value.Kind == DateTimeKind.Local) time.TimeFormat = TimeFormat.Local;
    //            else time.TimeFormat = TimeFormat.Unknown;
    //        } 
    //        return time;
    //    }

    //    public static T To_ITZID<T>(this TimeZoneInfo tzinfo)
    //        where T : ITZID, new()
    //    {
    //        var tzid = Activator.CreateInstance<T>();
    //        tzid.Suffix = tzinfo.Id;
    //        return tzid;
    //    }

    //    public static DateTime ToDateTime(this IDATE value)
    //    {
    //        if (value == null) return new DateTime();
    //        return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY);
    //    }

    //    public static DateTime ToDateTime(this IDATE_TIME value)
    //    {
    //        if (value == null) return new DateTime();

    //        if(value.TimeFormat == TimeFormat.Utc)
    //        {
    //            return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
    //                (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Utc);
    //        }
    //        else if(value.TimeFormat == TimeFormat.Local || value.TimeFormat == TimeFormat.LocalAndTimeZone)
    //        {
    //            return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
    //                (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Local);
    //        }
    //        else
    //        {
    //            return new DateTime((int)value.FULLYEAR, (int)value.MONTH, (int)value.MDAY,
    //                (int)value.HOUR, (int)value.MINUTE, (int)value.SECOND, DateTimeKind.Unspecified);
    //        }
    //    }

    //    public static TimeSpan ToTimeSpan(this ITIME value)
    //    {
    //        if (value == null) return new TimeSpan();
    //        return new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
    //    }

    //    public static TimeSpan ToTimeSpan(this IDATE_TIME value)
    //    {
    //        if (value == null) return new TimeSpan();
    //        return new TimeSpan((int)value.HOUR, (int)value.MINUTE, (int)value.SECOND);
    //    }

    //    public static T To_IDURATION<T>(this TimeSpan span)
    //        where T : IDURATION, new()
    //    {
    //        var duration = Activator.CreateInstance<T>();
    //        duration.DAYS = (uint)span.Days;
    //        duration.HOURS = (uint)span.Hours;
    //        duration.MINUTES = (uint)span.Minutes;
    //        duration.SECONDS = (uint)span.Seconds;
    //        duration.WEEKS = (uint)(span.TotalDays - (span.Days + (span.Hours / 24) + (span.Minutes / (24 * 60)) + (span.Seconds / (24 * 3600)) + (span.Milliseconds / (24 * 3600000)))) / 7u;
    //        var sum = span.Days + (span.Hours / 24) + (span.Minutes / (24 * 60)) + (span.Seconds / (24 * 3600)) + (span.Milliseconds / (24 * 3600000));
    //        duration.Sign = (sum >= 0) ? SignType.Positive : SignType.Negative;
    //        return duration;
    //    }

    //    public static T ToTime<T>(this TimeSpan span)
    //        where T : ITIME, new()
    //    {
    //        var time = Activator.CreateInstance<T>();
    //        time.HOUR = (uint)span.Hours;
    //        time.MINUTE = (uint)span.Minutes;
    //        time.SECOND = (uint)span.Seconds;
    //        return time;
    //    }

    //    public static TimeSpan ToTimeSpan(this IDURATION duration)
    //    {
    //        if (duration == null) throw new ArgumentNullException("Duration must not be null");
    //        var ts = new TimeSpan((int)duration.DAYS, (int)duration.HOURS, (int)duration.MINUTES, (int)duration.SECONDS);
    //        return ts;
    //    } 
    //    #endregion

    //    #region string parsers

    //    public static T Parse_IBINARY<T>(this string value)
    //where T : IBINARY, new()
    //    {
    //        var binary = Activator.CreateInstance<T>();
    //        binary.Value = value;
    //        return binary;
    //    }

    //    public static T Parse_IDATE<T>(this string value)
    //        where T : IDATE, new()
    //    {
    //        var date = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["year"].Success) date.FULLYEAR = uint.Parse(match.Groups["year"].Value);
    //                if (match.Groups["month"].Success) date.MONTH = uint.Parse(match.Groups["month"].Value);
    //                if (match.Groups["day"].Success) date.MDAY = uint.Parse(match.Groups["day"].Value);
    //            }
    //        }
    //        else throw new FormatException("Invalid date format. Please consult the definition of DATE in ICalendar Protocol RFC5545");
    //        return date;
    //    }

    //    public static T TryParse_IDATE<T>(this string value)
    //where T : IDATE, new()
    //    {
    //        var date = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["year"].Success) date.FULLYEAR = uint.Parse(match.Groups["year"].Value);
    //                if (match.Groups["month"].Success) date.MONTH = uint.Parse(match.Groups["month"].Value);
    //                if (match.Groups["day"].Success) date.MDAY = uint.Parse(match.Groups["day"].Value);
    //            }
    //        }
    //        else return default(T);
    //        return date;
    //    }

    //    public static T Parse_IDATETIME<T>(this string value, ITZID tzid = null)
    //where T : IDATE_TIME, new()
    //    {
    //        var date = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})(T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?))?$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["year"].Success) date.FULLYEAR = uint.Parse(match.Groups["year"].Value);
    //                if (match.Groups["month"].Success) date.MONTH = uint.Parse(match.Groups["month"].Value);
    //                if (match.Groups["day"].Success) date.MDAY = uint.Parse(match.Groups["day"].Value);
    //                if (match.Groups["hour"].Success) date.HOUR = uint.Parse(match.Groups["hour"].Value);
    //                if (match.Groups["min"].Success) date.MINUTE = uint.Parse(match.Groups["min"].Value);
    //                if (match.Groups["sec"].Success) date.SECOND = uint.Parse(match.Groups["sec"].Value);
    //                if (match.Groups["utc"].Success)
    //                {
    //                    if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
    //                        date.TimeFormat = TimeFormat.Utc;
    //                }
    //            }
    //        }
    //        else throw new FormatException("Invalid date time format. Please consult the definition of DATE-TIME in ICalendar Protocol RFC5545");
    //        date.TimeZoneId = tzid;
    //        return date;
    //    }

    //    public static T TryParse_IDATETIME<T>(this string value, ITZID tzid = null)
    //        where T : IDATE_TIME, new()
    //    {
    //        var date = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<year>\d{2,4})(?<month>\d{1,2})(?<day>\d{1,2})(T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?))?$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["year"].Success) date.FULLYEAR = uint.Parse(match.Groups["year"].Value);
    //                if (match.Groups["month"].Success) date.MONTH = uint.Parse(match.Groups["month"].Value);
    //                if (match.Groups["day"].Success) date.MDAY = uint.Parse(match.Groups["day"].Value);
    //                if (match.Groups["hour"].Success) date.HOUR = uint.Parse(match.Groups["hour"].Value);
    //                if (match.Groups["min"].Success) date.MINUTE = uint.Parse(match.Groups["min"].Value);
    //                if (match.Groups["sec"].Success) date.SECOND = uint.Parse(match.Groups["sec"].Value);
    //                if (match.Groups["utc"].Success)
    //                {
    //                    if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
    //                        date.TimeFormat = TimeFormat.Utc;
    //                }
    //            }
    //        }
    //        else return default(T);
    //        date.TimeZoneId = tzid;
    //        return date;
    //    }

    //    public static T Parse_ITIME<T>(this string value, ITZID tzid)
    //        where T : ITIME, new()
    //    {
    //        var time = Activator.CreateInstance<T>();
    //        var pattern = @"^T(?<hour>\d{2,4})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?)$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["hour"].Success) time.HOUR = uint.Parse(match.Groups["hour"].Value);
    //                if (match.Groups["min"].Success) time.MINUTE = uint.Parse(match.Groups["min"].Value);
    //                if (match.Groups["sec"].Success) time.SECOND = uint.Parse(match.Groups["sec"].Value);
    //                if (match.Groups["utc"].Success)
    //                {
    //                    if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
    //                        time.TimeFormat = TimeFormat.Utc;
    //                }
    //            }
    //        }
    //        else throw new FormatException("Invalid date time format. Please consult the definition of TIME in ICalendar Protocol RFC5545");
    //        time.TimeZoneId = tzid;
    //        return time;
    //    }

    //    public static T TryParse_ITME<T>(this string value, ITZID tzid)
    //        where T : ITIME, new()
    //    {
    //        var time = Activator.CreateInstance<T>();
    //        var pattern = @"^T(?<hour>\d{2,4})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z?)$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["hour"].Success) time.HOUR = uint.Parse(match.Groups["hour"].Value);
    //                if (match.Groups["min"].Success) time.MINUTE = uint.Parse(match.Groups["min"].Value);
    //                if (match.Groups["sec"].Success) time.SECOND = uint.Parse(match.Groups["sec"].Value);
    //                if (match.Groups["utc"].Success)
    //                {
    //                    if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
    //                        time.TimeFormat = TimeFormat.Utc;
    //                }
    //            }
    //        }
    //        else return default(T);
    //        time.TimeZoneId = tzid;
    //        return time;
    //    }

    //    public static T Parse_IDURATION<T>(this string value)
    //        where T : IDURATION, new()
    //    {
    //        var duration = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["weeks"].Success) duration.WEEKS = uint.Parse(match.Groups["weeks"].Value);
    //                if (match.Groups["days"].Success) duration.DAYS = uint.Parse(match.Groups["days"].Value);
    //                if (match.Groups["hours"].Success) duration.HOURS = uint.Parse(match.Groups["hours"].Value);
    //                if (match.Groups["mins"].Success) duration.MINUTES = uint.Parse(match.Groups["mins"].Value);
    //                if (match.Groups["secs"].Success) duration.SECONDS = uint.Parse(match.Groups["secs"].Value);
    //                if (match.Groups["minus"].Success) duration.Sign = SignType.Negative;
    //                else duration.Sign = SignType.Positive;
    //            }
    //        }
    //        else throw new FormatException("Invalid date time format. Please consult the definition of TIME in ICalendar Protocol RFC5545");
    //        return duration;
    //    }

    //    public static T TryParse_IDURATION<T>(this string value)
    //        where T : IDURATION
    //    {
    //        var duration = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["weeks"].Success) duration.WEEKS = uint.Parse(match.Groups["weeks"].Value);
    //                if (match.Groups["days"].Success) duration.DAYS = uint.Parse(match.Groups["days"].Value);
    //                if (match.Groups["hours"].Success) duration.HOURS = uint.Parse(match.Groups["hours"].Value);
    //                if (match.Groups["mins"].Success) duration.MINUTES = uint.Parse(match.Groups["mins"].Value);
    //                if (match.Groups["secs"].Success) duration.SECONDS = uint.Parse(match.Groups["secs"].Value);
    //                if (match.Groups["minus"].Success) duration.Sign = SignType.Negative;
    //                else duration.Sign = SignType.Positive;
    //            }
    //        }
    //        else return default(T);
    //        return duration;
    //    }

    //    public static T Parse_IPERIOD<T, U, V>(this string value)
    //        where T : IPERIOD, new()
    //        where U : IDATE_TIME, new()
    //        where V : IDURATION, new()
    //    {
    //        var period = Activator.CreateInstance<T>();
    //        var parts = value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
    //        if (parts == null || parts.Length != 2) throw new FormatException("Invalid period format");

    //        try
    //        {
    //            period.Start = parts[0].Parse_IDATETIME<U>();
    //            period.End = parts[1].Parse_IDATETIME<U>();
    //        }
    //        catch (FormatException) 
    //        {            
    //            try
    //            {
    //                period.Duration = parts[1].Parse_IDURATION<V>();
    //            }
    //            catch (FormatException) { throw; }
    //            catch (Exception) { throw; }            
    //        }
    //        catch (Exception) { throw; }
    //        return period;
    //    }

    //    public static T TryParse_IPERIOD<T, U, V>(this string value)
    //        where T : IPERIOD, new()
    //        where U : IDATE_TIME, new()
    //        where V : IDURATION, new()
    //    {
    //        var period = Activator.CreateInstance<T>();
    //        var parts = value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
    //        if (parts == null || parts.Length != 2) throw new FormatException("Invalid period format");

    //        var start = parts[0].Parse_IDATETIME<U>();
    //        if (!start.Equals(default(U))) period.Start = start;
    //        else return default(T);

    //        var end = parts[1].Parse_IDATETIME<U>();
    //        if (!end.Equals(default(U))) period.End = end;
    //        else
    //        {
    //            var dur = parts[1].Parse_IDURATION<V>();
    //            if (!dur.Equals(default(V))) period.Duration = dur;
    //            else return default(T);
    //        }
    //        return period;
    //    }

    //    public static T Parse_IWEEKDAYNUM<T>(this string value)
    //        where T : IWEEKDAYNUM, new()
    //    {
    //        var weekdaynum = Activator.CreateInstance<T>();
    //        var pattern = @"^((?<sign>\w)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            var mulitplier = 1;
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["sign"].Success)
    //                {
    //                    if (match.Groups["sign"].Value == "-") mulitplier *= -1;
    //                }
    //                if (match.Groups["ordwk"].Success) weekdaynum.OrdinalWeek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
    //                if (match.Groups["weekday"].Success) weekdaynum.Weekday = match.Groups["weekday"].Value.TranslateToWEEKDAY();
    //            }
    //        }
    //        else throw new FormatException("Invalid WeekDayNum format. Please consult the definition of WeekDayNum in ICalendar Protocol RFC5545");
    //        return weekdaynum;
    //    }

    //    public static T TryParse_IWEEKDAYNUM<T>(this string value)
    //where T : IWEEKDAYNUM, new()
    //    {
    //        var weekdaynum = Activator.CreateInstance<T>();
    //        var pattern = @"^((?<sign>\w)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            var mulitplier = 1;
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["sign"].Success)
    //                {
    //                    if (match.Groups["sign"].Value == "-") mulitplier *= -1;
    //                }
    //                if (match.Groups["ordwk"].Success) weekdaynum.OrdinalWeek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
    //                if (match.Groups["weekday"].Success) weekdaynum.Weekday = match.Groups["weekday"].Value.TranslateToWEEKDAY();
    //            }
    //        }
    //        else return default(T);
    //        return weekdaynum;
    //    }


    //    public static T Parse_IRECUR<T, U, V>(this string value)
    //        where T : IRECUR, new()
    //        where U : IDATE_TIME, new()
    //        where V : IWEEKDAYNUM, new()
    //    {
    //        var recur = Activator.CreateInstance<T>();
    //        var tokens = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
    //        if (tokens == null || tokens.Length == 0) throw new FormatException("Invalid Recur format");

    //        try
    //        {
    //            foreach (var token in tokens)
    //            {
    //                var pattern = @"^(FREQ|UNTIL|COUNT|INTERVAL|BYSECOND|BYMINUTE|BYHOUR|BYMONTHDAY|BYYEARDAY|BYWEEKNO|BYMONTH|WKST|BYSETPOS)=((\w+|\d+)(,[\s]*(\w+|\d+))*)$";
    //                if (!Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)) continue;

    //                var pair = token.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

    //                //check FREQ
    //                if (pair[0].Equals("FREQ", StringComparison.OrdinalIgnoreCase)) recur.FREQ = pair[1].TranslateToFREQ();
    //                if (pair[0].Equals("UNTIL", StringComparison.OrdinalIgnoreCase)) recur.UNTIL = pair[1].Parse_IDATETIME<U>();
    //                if (pair[0].Equals("COUNT", StringComparison.OrdinalIgnoreCase)) recur.COUNT = uint.Parse(pair[1]);
    //                if (pair[0].Equals("INTERVAL", StringComparison.OrdinalIgnoreCase)) recur.COUNT = uint.Parse(pair[1]);
    //                if (pair[0].Equals("BYSECOND", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYSECOND = parts.Select(x => uint.Parse(x));
    //                }
    //                if (pair[0].Equals("BYMINUTE", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMINUTE = parts.Select(x => uint.Parse(x));
    //                }
    //                if (pair[0].Equals("BYHOUR", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMINUTE = parts.Select(x => uint.Parse(x));
    //                }

    //                if (pair[0].Equals("BYDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYDAY = parts.Select(x => x.Parse_IWEEKDAYNUM<V>() as IWEEKDAYNUM);
    //                }

    //                if (pair[0].Equals("BYMONTHDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMONTHDAY = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYYEARDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYYEARDAY = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYWEEKNO", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYWEEKNO = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYMONTH", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMONTH = parts.Select(x => uint.Parse(x));
    //                }

    //                if (pair[0].Equals("WKST", StringComparison.OrdinalIgnoreCase)) recur.WKST = pair[1].TranslateToWEEKDAY();

    //                if (pair[0].Equals("BYSETPOS", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYSETPOS = parts.Select(x => int.Parse(x));
    //                }
    //            }
    //        }
    //        catch (ArgumentNullException) { throw; }
    //        catch (FormatException) { throw; }
    //        catch (OverflowException) { throw; }
    //        catch (Exception) { throw; }

    //        return recur;
    //    }


    //    public static T TryParse_IRECUR<T, U, V>(this string value)
    //        where T : IRECUR, new()
    //        where U : IDATE_TIME, new()
    //        where V : IWEEKDAYNUM, new()
    //    {
    //        var recur = Activator.CreateInstance<T>();
    //        var tokens = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
    //        if (tokens == null || tokens.Length == 0) return default(T);

    //        try
    //        {
    //            foreach (var token in tokens)
    //            {
    //                var pattern = @"^(FREQ|UNTIL|COUNT|INTERVAL|BYSECOND|BYMINUTE|BYHOUR|BYMONTHDAY|BYYEARDAY|BYWEEKNO|BYMONTH|WKST|BYSETPOS)=((\w+|\d+)(,[\s]*(\w+|\d+))*)$";
    //                if (!Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)) continue;

    //                var pair = token.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

    //                //check FREQ
    //                if (pair[0].Equals("FREQ", StringComparison.OrdinalIgnoreCase)) recur.FREQ = pair[1].TranslateToFREQ();
    //                if (pair[0].Equals("UNTIL", StringComparison.OrdinalIgnoreCase)) recur.UNTIL = pair[1].Parse_IDATETIME<U>();
    //                if (pair[0].Equals("COUNT", StringComparison.OrdinalIgnoreCase)) recur.COUNT = uint.Parse(pair[1]);
    //                if (pair[0].Equals("INTERVAL", StringComparison.OrdinalIgnoreCase)) recur.COUNT = uint.Parse(pair[1]);
    //                if (pair[0].Equals("BYSECOND", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYSECOND = parts.Select(x => uint.Parse(x));
    //                }
    //                if (pair[0].Equals("BYMINUTE", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMINUTE = parts.Select(x => uint.Parse(x));
    //                }
    //                if (pair[0].Equals("BYHOUR", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMINUTE = parts.Select(x => uint.Parse(x));
    //                }

    //                if (pair[0].Equals("BYDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYDAY = parts.Select(x => x.Parse_IWEEKDAYNUM<V>() as IWEEKDAYNUM);
    //                }

    //                if (pair[0].Equals("BYMONTHDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMONTHDAY = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYYEARDAY", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYYEARDAY = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYWEEKNO", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYWEEKNO = parts.Select(x => int.Parse(x));
    //                }

    //                if (pair[0].Equals("BYMONTH", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYMONTH = parts.Select(x => uint.Parse(x));
    //                }

    //                if (pair[0].Equals("WKST", StringComparison.OrdinalIgnoreCase)) recur.WKST = pair[1].TranslateToWEEKDAY();

    //                if (pair[0].Equals("BYSETPOS", StringComparison.OrdinalIgnoreCase))
    //                {
    //                    var parts = pair[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (parts == null || parts.Length == 0) continue;
    //                    recur.BYSETPOS = parts.Select(x => int.Parse(x));
    //                }
    //            }
    //        }
    //        catch (ArgumentNullException) { return default(T); }
    //        catch (FormatException) { return default(T); }
    //        catch (OverflowException) { return default(T); }
    //        catch (Exception) { return default(T); }

    //        return recur;
    //    }


    //    public static T Parse_IURI<T>(this string value)
    //        where T : IURI, new()
    //    {
    //        var uri = Activator.CreateInstance<T>();
    //        try
    //        {
    //            uri.Path = value;
    //        }
    //        catch (FormatException) { throw; }
    //        catch (Exception) { throw; }
    //        return uri;
    //    }

    //    public static T TryParse_IURI<T>(this string value)
    //        where T : IURI, new()
    //    {
    //        var uri = Activator.CreateInstance<T>();
    //        try
    //        {
    //            uri.Path = value;
    //        }
    //        catch (FormatException) { return default(T); }
    //        catch (Exception) { return default(T); }
    //        return uri;
    //    }

    //    public static T Parse_ITUCOFFSET<T>(this string value)
    //        where T : IUTC_OFFSET, new()
    //    {
    //        var offset = Activator.CreateInstance<T>();
    //        var pattern = @"^(?<minus>\-|?<plus>\+)(?<hours>\d{1,2})(?<mins>\d{1,2})(?<secs>\d{1,2})?$";
    //        if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //        {
    //            foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
    //            {
    //                if (match.Groups["hours"].Success) offset.HOUR = uint.Parse(match.Groups["hours"].Value);
    //                if (match.Groups["mins"].Success) offset.MINUTE = uint.Parse(match.Groups["mins"].Value);
    //                if (match.Groups["secs"].Success) offset.MINUTE = uint.Parse(match.Groups["secs"].Value);
    //                if (match.Groups["minus"].Success) offset.Sign = SignType.Negative;
    //                else if (match.Groups["plus"].Success) offset.Sign = SignType.Positive;
    //            }
    //        }
    //        else throw new FormatException("Invalid UTC OFFSET format");

    //        return offset;
    //    } 
    //    #endregion

        #endregion

        #region specialized converters for iCalendar Parameter types

        #region string parsers

        public static T Parse_IALTREP<T>(this string value)
    where T : IALTREP, new()
        {
            var altrep = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^ALTREP=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Alternaitve Text Representation Format");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                        altrep.Path = match.Groups["value"].Value.Replace("\"", string.Empty);
                }

            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return altrep;
        }

        public static T TryParse_IALTREP<T>(this string value)
            where T : IALTREP, new()
        {
            var altrep = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^ALTREP=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                    {
                        altrep.Path = match.Groups["value"].Value.Replace("\"", string.Empty);
                        break;
                    }
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return altrep;
        }


        public static string Parse_CommonName(this string value)
        {
            var cn = string.Empty;
            try
            {
                if (!Regex.IsMatch(value, @"^(\p{Lt})+(,*\s\p{Lt}+)*$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Common Name Format");
                var parts = value.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts != null && parts.Length == 2) cn = parts[1];

            }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            return cn;
        }

        public static string TryParse_CommonName(this string value)
        {
            var cn = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(value)) return null;
                if (!Regex.IsMatch(value, @"^(\p{Lt})+(,*\s\p{Lt}+)*$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return null;
                var parts = value.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts != null && parts.Length == 2) cn = parts[1];
            }
            catch (ArgumentNullException) { return null; }
            catch (ArgumentOutOfRangeException) { return null; }
            catch (ArgumentException) { return null; }
            return cn;
        }

        public static T Parse_IDELEGATE<T, U>(this string value)
            where T : IDELEGATE, new()
            where U : IURI, new()
        {
            var del = Activator.CreateInstance<T>();
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,[\s]*{0})*$", uricheck);
            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Delegate format");
                var val = string.Empty;

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                del.Addresses = new List<IURI>(parts.Select(p => p.Parse_IURI<U>() as IURI));
            }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            return del;
        }

        public static T TryParse_IDELEGATE<T, U>(this string value)
            where T : IDELEGATE, new()
            where U : IURI, new()
        {
            var del = Activator.CreateInstance<T>();
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,[\s]*{0})*$", uricheck);
            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);
                var val = string.Empty;

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                del.Addresses = new List<IURI>(parts.Select(p => p.Parse_IURI<U>() as IURI));
            }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            return del;
        }

        public static T Parse_IDIR<T, U>(this string value)
            where T : IDIR, new()
            where U : IURI, new()
        {
            var dir = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^DIR=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Diretory Format");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                        dir.Uri = match.Groups["value"].Value.Replace("\"", string.Empty).Parse_IURI<U>();
                }

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }

            return dir;
        }


        public static T TryParse_IDIR<T, U>(this string value)
            where T : IDIR, new()
            where U : IURI, new()
        {
            var dir = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^DIR=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                        dir.Uri = match.Groups["value"].Value.Parse_IURI<U>();
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return dir;
        }

        public static T Parse_IFMTTYPE<T>(this string value)
            where T : IFMTTYPE, new()
        {
            var fmt = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^FMTTYPE=(?<type>\w+)(\.?<subtype>\w+)?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["type"].Success) fmt.TypeName = match.Groups["type"].Value;
                    else if (match.Groups["subtype"].Success) fmt.SubTypeName = match.Groups["subtype"].Value;
                }

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }

            return fmt;
        }

        public static T TryParse_IFMTTYPE<T>(this string value)
            where T : IFMTTYPE, new()
        {
            var fmt = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) return default(T);
                var pattern = @"^FMTTYPE=(?<type>\w+)(\.?<subtype>\w+)?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["type"].Success) fmt.TypeName = match.Groups["type"].Value;
                    else if (match.Groups["subtype"].Success) fmt.SubTypeName = match.Groups["subtype"].Value;
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { throw; }
            catch (Exception) { return default(T); }

            return fmt;
        }

        public static T Parse_ILANGUAGE<T>(this string value)
            where T : ILANGUAGE, new()
        {
            var language = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^LANGUAGE=(?<tag>\w+)(\-(?<subtag>\w+))?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["tag"].Success) language.Tag = match.Groups["tag"].Value;
                    else if (match.Groups["subtag"].Success) language.SubTag = match.Groups["subtag"].Value;
                }

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
            return language;
        }

        public static T TryParse_ILANGUAGE<T>(this string value)
            where T : ILANGUAGE, new()
        {
            var language = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) return default(T);
                var pattern = @"^LANGUAGE=(?<tag>\w+)(\-(?<subtag>\w+))?$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Format type");
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["tag"].Success) language.Tag = match.Groups["tag"].Value;
                    else if (match.Groups["subtag"].Success) language.SubTag = match.Groups["subtag"].Value;
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return language;
        }

        public static T Parse_IMEMBER<T, U>(this string value)
            where T : IMEMBER, new()
            where U : IURI, new()
        {
            var member = Activator.CreateInstance<T>();
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,\s*{0})*$", uricheck);

            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new FormatException("Invalid Member format");
                var val = string.Empty;

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                member.Addresses = new List<IURI>(parts.Select(p => p.Parse_IURI<U>() as IURI));

            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
            return member;
        }

        public static T TryParse_IMEMBER<T, U>(this string value)
            where T : IMEMBER, new()
            where U : IURI, new()
        {
            var member = Activator.CreateInstance<T>();
            var uricheck = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";
            var pattern = string.Format(@"^(DELEGATED\-{0}(,\s*{0})*$", uricheck);

            try
            {
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)) return default(T);
                var val = string.Empty;
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success) val = match.Groups["value"].Value;
                }
                var parts = val.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                member.Addresses = new List<IURI>(parts.Select(p => p.Parse_IURI<U>() as IURI));
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return member;
        }


        public static T Parse_ISENTBY<T, U>(this string value)
            where T : ISENT_BY, new()
            where U : IURI, new()
        {
            var sentby = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^SENT\-BY=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                        sentby.Address = match.Groups["value"].Value.Parse_IURI<U>();
                }
            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
            return sentby;
        }

        public static T TryParse_ISENTBY<T, U>(this string value)
            where T : ISENT_BY, new()
            where U : IURI, new()
        {
            var sentby = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) return default(T);
                var pattern = @"^SENT\-BY=(\"")(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))(\"")$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["value"].Success)
                        sentby.Address = match.Groups["value"].Value.Parse_IURI<U>();
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return sentby;
        }


        public static T Parse_ITZID<T>(this string value)
            where T : ITZID, new()
        {
            var tzid = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                var pattern = @"^(?<prefix>(\p{L})+)*(?<solidus>\/)*(?<suffix>(\p{L}+\p{P}*\s*)+)$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    throw new ArgumentNullException("value");

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["prefix"].Success)
                    {
                        tzid.Prefix = match.Groups["prefix"].Value;
                        tzid.GloballyUnique = false;
                    }
                    if (match.Groups["suffix"].Success) tzid.Suffix = match.Groups["suffix"].Value;
                    if (match.Groups["solidus"].Success && !match.Groups["prefix"].Success) tzid.GloballyUnique = true;
                }
            }
            catch (FormatException) { throw; }
            catch (ArgumentNullException) { throw; }
            catch (ArgumentOutOfRangeException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception) { throw; }
            return tzid;
        }

        public static T TryParse_ITZID<T>(this string value)
     where T : ITZID, new()
        {
            var tzid = Activator.CreateInstance<T>();
            try
            {
                if (string.IsNullOrEmpty(value)) return default(T);
                var pattern = @"^(?<prefix>(\p{L})+)*(?<solidus>\/)*(?<suffix>(\p{L}+\p{P}*\s*)+)$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                    return default(T);

                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase))
                {
                    if (match.Groups["prefix"].Success)
                    {
                        tzid.Prefix = match.Groups["prefix"].Value;
                        tzid.GloballyUnique = false;
                    }
                    if (match.Groups["suffix"].Success) tzid.Suffix = match.Groups["suffix"].Value;
                    if (match.Groups["solidus"].Success && !match.Groups["prefix"].Success) tzid.GloballyUnique = true;
                }
            }
            catch (FormatException) { return default(T); }
            catch (ArgumentNullException) { return default(T); }
            catch (ArgumentOutOfRangeException) { return default(T); }
            catch (ArgumentException) { return default(T); }
            catch (Exception) { return default(T); }
            return tzid;
        } 
        #endregion

        #endregion

    }


}
