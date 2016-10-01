using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.extensions;
using reexjungle.xcal.infrastructure.serialization;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    [DataContract]
    public struct DATE : IDATE, IDATE<DATE>, IEquatable<DATE>, IComparable<DATE>, ICalendarSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct.
        /// </summary>
        /// <param name="fullyear">The 4-digit representation of a full year e.g. 2013.</param>
        /// <param name="month">The 2-digit representation of a month.</param>
        /// <param name="mday">The 2-digit representation of a day of the month.</param>
        public DATE(uint fullyear, uint month, uint mday)
        {
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> instance.</param>
        public DATE(DateTime datetime)
        {
            FULLYEAR = (uint)datetime.Year;
            MONTH = (uint)datetime.Month;
            MDAY = (uint)datetime.Day;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from a string.
        /// </summary>
        /// <param name="value">
        /// The string containing serialized information about the <see cref="DATE"/> struct.
        /// </param>
        public DATE(string value)
        {
            var date = Parse(value);
            FULLYEAR = date.FULLYEAR;
            MONTH = date.MONTH;
            MDAY = date.MDAY;
        }

        private static DATE Parse(string value)
        {
            uint fullyear = 1u;
            uint month = 1u;
            uint mday = 1u;
            var pattern = @"^(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})$";
            var options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["year"].Success) fullyear = uint.Parse(match.Groups["year"].Value);
                    if (match.Groups["month"].Success) month = uint.Parse(match.Groups["month"].Value);
                    if (match.Groups["day"].Success) mday = uint.Parse(match.Groups["day"].Value);
                }
            }

            return new DATE(fullyear, month, mday);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from an instance implementing
        /// the <see cref="IDATE"/> interface.
        /// </summary>
        /// <param name="other">The instance implementing the <see cref="IDATE"/> interface</param>
        /// <exception cref="System.ArgumentNullException">date</exception>
        public DATE(IDATE other)
        {
            FULLYEAR = other.FULLYEAR;
            MONTH = other.MONTH;
            MDAY = other.MDAY;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value
        /// has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref
        ///           name="other"/> parameter.Zero This object is equal to <paramref name="other"/>.
        ///           Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DATE other)
        {
            if (other == default(DATE)) return -2; //undefined
            if (FULLYEAR < other.FULLYEAR) return -1;
            if (FULLYEAR > other.FULLYEAR) return 1;
            if (MONTH < other.MONTH) return -1;
            if (MONTH > other.MONTH) return 1;
            if (MDAY < other.MDAY) return -1;
            return MDAY > other.MDAY ? 1 : 0;
        }

        /// <summary>
        /// Gets or sets the 4-digit representation of a full year e.g. 2013.
        /// </summary>
        public uint FULLYEAR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month.
        /// </summary>
        public uint MONTH { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a day of the month.
        /// </summary>
        public uint MDAY { get; private set; }

        public DATE AddDays(double value) => new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddDays(value).AsDATE();

        public DATE AddWeeks(int value) => this.AsDateTime().AddWeeks(value).AsDATE();

        public DATE AddMonths(int value) => new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddMonths(value).AsDATE();

        public DATE AddYears(int value) => new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddYears(value).AsDATE();

        public WEEKDAY GetWeekday() => this.AsDateTime().DayOfWeek.AsWEEKDAY();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DATE other) => FULLYEAR == other.FULLYEAR &&
       MONTH == other.MONTH &&
       MDAY == other.MDAY;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}"; //YYYYMMDD

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DATE && Equals((DATE)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)FULLYEAR;
                hashCode = (hashCode * 397) ^ (int)MONTH;
                hashCode = (hashCode * 397) ^ (int)MDAY;
                return hashCode;
            }
        }

        #region overloaded operators

        /// <summary>
        /// Implements the equality operator ==.
        /// </summary>
        /// <param name="a">The instance to compare for equality</param>
        /// <param name="b">The other instance to compare against this instance for equality.</param>
        /// <returns>True if both instances are equal, otherwise false</returns>
        public static bool operator ==(DATE a, DATE b) => a.Equals(b);

        /// <summary>
        /// Implements the inequality operator !=.
        /// </summary>
        /// <param name="a">The instance to compare for inequality</param>
        /// <param name="b">The other instance to compare against this instance for inequality.</param>
        /// <returns>True if both instances are unequal, otherwise false</returns>
        public static bool operator !=(DATE a, DATE b) => !a.Equals(b);

        /// <summary>
        /// Implements the less-than operator &lt;.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>True if this instance is less than the other, otherwise false.</returns>
        public static bool operator <(DATE a, DATE b) => a.CompareTo(b) < 0;

        /// <summary>
        /// Implements the less-than-or-equal-to operator &lt;=.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>
        /// True if this instance is less than or equal to the other, otherwise false.
        /// </returns>
        public static bool operator <=(DATE a, DATE b) => a.CompareTo(b) <= 0;

        /// <summary>
        /// Implements the greater-than operator &gt;.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>True if this instance is greater than the other, otherwise false.</returns>
        public static bool operator >(DATE a, DATE b) => a.CompareTo(b) > 0;

        /// <summary>
        /// Implements the greater-than-or-equal-to operator &gt;=.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>True if this instance is greater than the other, otherwise false.</returns>
        public static bool operator >=(DATE a, DATE b) => a.CompareTo(b) >= 0;

        /// <summary>
        /// Implements the substraction operator - between two <see cref="DATE"/> instances.
        /// </summary>
        /// <param name="start">The <see cref="DATE"/> instance to substract from.</param>
        /// <param name="end">The substracted <see cref="DATE"/> instance.</param>
        /// <returns>The result of the substraction.</returns>
        public static DURATION operator -(DATE start, DATE end) => new DURATION(end.AsDateTime() - start.AsDateTime());

        /// <summary>
        /// Implements the addition operator +.
        /// </summary>
        /// <param name="start">The <see cref="DATE"/> instance to add to</param>
        /// <param name="duration">The added <see cref="DURATION"/> instance.</param>
        /// <returns>A new <see cref="DATE"/> instance resulting from the addition.</returns>
        public static DATE operator +(DATE start, DURATION duration) => (start.AsDateTime().Add(duration.AsTimeSpan())).AsDATE();

        /// <summary>
        /// Implements the substraction operator - between a <see cref="DATE"/> instance and a <see
        /// cref="DURATION"/> instance.
        /// </summary>
        /// <param name="end">The <see cref="DATE"/> instance to substract from.</param>
        /// <param name="duration">The substracted <see cref="DURATION"/> instance.</param>
        /// <returns>A new <see cref="DATE"/> instance resulting from the substraction.</returns>
        public static DATE operator -(DATE end, DURATION duration) => (end.AsDateTime().Subtract(duration.AsTimeSpan())).AsDATE();

        #endregion overloaded operators

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var date = Parse(inner.Value);
                    FULLYEAR = date.FULLYEAR;
                    MONTH = date.MONTH;
                    MDAY = date.MDAY;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// Represents a class for identifying values that specify a precise calendar date and time of
    /// the date
    /// Format: [YYYYMMSS]"T"[HHMMSS]"Z" where YYYY is 4-digit year, MM is 2-digit month and DD is
    ///         2-digit day
    /// </summary>
    [DataContract]
    public struct DATE_TIME : IDATE, IDATE<DATE_TIME>, ITIME, ITIME<DATE_TIME>, IEquatable<DATE_TIME>, IComparable<DATE_TIME>, ICalendarSerializable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DATE_TIME"/> struct.
        /// </summary>
        /// <param name="fullyear">The 4-digit date time.</param>
        /// <param name="month">The month of the date time.</param>
        /// <param name="mday">The day of the date time.</param>
        /// <param name="hour">The hour of the date time.</param>
        /// <param name="minute">The minute of the date time.</param>
        /// <param name="second">The second of the date time.</param>
        /// <param name="type">The time format of the <see cref="DATE_TIME"/> type.</param>
        /// <param name="tzid">The time zone identifer of the <see cref="DATE_TIME"/> type.</param>
        public DATE_TIME(uint fullyear, uint month, uint mday, uint hour, uint minute, uint second,
            TimeType type = TimeType.Local, TZID tzid = null)
        {
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Type = type;
            TimeZoneId = tzid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from a <see
        /// cref="datetime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="datetime"/> struct.</param>
        /// <param name="tzinfo">
        /// The time zone identifer ( <see cref="TimeZoneInfo"/> ) of the <see cref="DATE_TIME"/> type.
        /// </param>
        public DATE_TIME(DateTime datetime, TimeZoneInfo tzinfo = null)
        {
            FULLYEAR = (uint)datetime.Year;
            MONTH = (uint)datetime.Month;
            MDAY = (uint)datetime.Day;
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            TimeZoneId = null;
            Type = TimeType.NONE;
            if (tzinfo != null)
            {
                TimeZoneId = new TZID(null, tzinfo.Id);
                Type = TimeType.LocalAndTimeZone;
            }
            else Type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) Type = TimeType.Utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> from a <see cref="datetime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="datetime"/> struct.</param>
        /// <param name="tzid">
        /// The time zone identifer ( <see cref="TZID"/> ) of the <see cref="DATE_TIME"/> instance.
        /// </param>
        public DATE_TIME(DateTime datetime, TZID tzid)
        {
            FULLYEAR = (uint)datetime.Year;
            MONTH = (uint)datetime.Month;
            MDAY = (uint)datetime.Day;
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            TimeZoneId = null;
            Type = TimeType.NONE;
            if (tzid != null)
            {
                TimeZoneId = tzid;
                Type = TimeType.LocalAndTimeZone;
            }
            else Type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) Type = TimeType.Utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from a <see
        /// cref="DateTimeOffset"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTimeOffset"/> struct</param>
        public DATE_TIME(DateTimeOffset datetime)
        {
            FULLYEAR = (uint)datetime.Year;
            MONTH = (uint)datetime.Month;
            MDAY = (uint)datetime.Day;
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            Type = TimeType.Utc;
            TimeZoneId = null;
        }

        /// <summary>
        /// Deserializes a new instance of the <see cref="DATE_TIME"/> struct from string.
        /// </summary>
        /// <param name="value">
        /// The string containing serialized ínformation about the <see cref="DATE_TIME"/> struct.
        /// </param>
        public DATE_TIME(string value)
        {
            var datetime = Parse(value);
            FULLYEAR = datetime.FULLYEAR;
            MONTH = datetime.MONTH;
            MDAY = datetime.MDAY;
            HOUR = datetime.HOUR;
            MINUTE = datetime.HOUR;
            SECOND = datetime.SECOND;
            TimeZoneId = datetime.TimeZoneId;
            Type = datetime.Type;
        }

        private static DATE_TIME Parse(string value)
        {
            var fullyear = 1u;
            var month = 1u;
            var mday = 1u;
            var hour = 0u;
            var minute = 0u;
            var second = 0u;
            TZID tzid = null;
            var type = TimeType.NONE;
            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})T(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})(?<utc>Z)?$";
            var options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["year"].Success) fullyear = uint.Parse(match.Groups["year"].Value);
                    if (match.Groups["month"].Success) month = uint.Parse(match.Groups["month"].Value);
                    if (match.Groups["day"].Success) mday = uint.Parse(match.Groups["day"].Value);
                    if (match.Groups["hour"].Success) hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success)
                    {
                        type = TimeType.Utc;
                    }
                    if (match.Groups["tzid"].Success)
                    {
                        tzid = new TZID(match.Groups["tzid"].Value);
                        type = TimeType.LocalAndTimeZone;
                    }
                }
            }

            return new DATE_TIME(fullyear, month, mday, hour, minute, second, type, tzid);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from an implementation
        /// of the <see cref="IDATE_TIME"/> interface.
        /// </summary>
        /// <param name="value">The <see cref="IDATE_TIME"/> interface.</param>
        /// <param name="tzid"></param>
        public DATE_TIME(IDATE value)
        {
            FULLYEAR = value.FULLYEAR;
            MONTH = value.MONTH;
            MDAY = value.MDAY;
            HOUR = 0u;
            MINUTE = 0u;
            SECOND = 0u;
            TimeZoneId = null;
            Type = TimeType.NONE;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from an <see
        /// cref="IDATE"/> instance.
        /// </summary>
        /// <param name="value">The source <see cref="DATE_TIME"/> struct.</param>
        /// <param name="tzid">The time zone identifer ( <see cref="TZID"/> ).</param>
        /// <exception cref="System.ArgumentNullException">date</exception>
        public DATE_TIME(ITIME value, TZID tzid = null)
        {
            FULLYEAR = 1u;
            MONTH = 1u;
            MDAY = 1u;
            HOUR = value.HOUR;
            MINUTE = value.MINUTE;
            SECOND = value.SECOND;
            Type = tzid != null ? TimeType.LocalAndTimeZone : TimeType.NONE;
            TimeZoneId = tzid ?? value.TimeZoneId;
        }

        public DATE_TIME(DATE_TIME other)
        {
            FULLYEAR = other.FULLYEAR;
            MONTH = other.MONTH;
            MDAY = other.MDAY;
            HOUR = other.HOUR;
            MINUTE = other.MINUTE;
            SECOND = other.SECOND;
            Type = other.Type;
            TimeZoneId = other.TimeZoneId;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value
        /// has the following
        /// meanings: Value Meaning Less than zero This object is less than the <paramref
        ///           name="other"/> parameter.Zero This object is equal to <paramref name="other"/>.
        ///           Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DATE_TIME other)
        {
            if (FULLYEAR < other.FULLYEAR) return -1;
            if (FULLYEAR > other.FULLYEAR) return 1;
            if (MONTH < other.MONTH) return -1;
            if (MONTH > other.MONTH) return 1;
            if (MDAY < other.MDAY) return -1;
            if (MDAY > other.MDAY) return 1;
            if (HOUR < other.HOUR) return -1;
            if (HOUR > other.HOUR) return 1;
            if (MINUTE < other.MINUTE) return -1;
            if (MINUTE > other.MINUTE) return 1;
            if (SECOND < other.SECOND) return -1;
            if (SECOND > other.SECOND) return 1;
            return 0;
        }

        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013
        /// </summary>
        public uint FULLYEAR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        public uint MONTH { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        public uint MDAY { get; private set; }

        public DATE_TIME AddDays(double value) => this.AsDateTime().AddDays(value).AsDATE_TIME(TimeZoneId);

        public DATE_TIME AddWeeks(int value) => this.AsDateTime().AddWeeks(value).AsDATE_TIME(TimeZoneId);

        public DATE_TIME AddMonths(int value) => this.AsDateTime().AddMonths(value).AsDATE_TIME(TimeZoneId);

        public DATE_TIME AddYears(int value) => this.AsDateTime().AddYears(value).AsDATE_TIME(TimeZoneId);

        public WEEKDAY GetWeekday() => this.AsDateTime().DayOfWeek.AsWEEKDAY();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DATE_TIME other) => HOUR == other.HOUR &&
       MINUTE == other.MINUTE
       && SECOND == other.SECOND &&
       FULLYEAR == other.FULLYEAR &&
       MONTH == other.MONTH &&
       MDAY == other.MDAY &&
       Equals(TimeZoneId, other.TimeZoneId);

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            switch (Type)
            {
                case TimeType.Local:
                    writer.Write($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                case TimeType.Utc:
                    writer.Write($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z");
                    break;

                case TimeType.LocalAndTimeZone:
                    writer.Write($"{TimeZoneId}:{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                default:
                    writer.Write($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;
            }
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var datetime = Parse(inner.Value);
                    FULLYEAR = datetime.FULLYEAR;
                    MONTH = datetime.MONTH;
                    MDAY = datetime.MDAY;
                    HOUR = datetime.HOUR;
                    MINUTE = datetime.MINUTE;
                    SECOND = datetime.SECOND;
                    Type = datetime.Type;
                    TimeZoneId = datetime.TimeZoneId;
                }
            }
        }

        /// <summary>
        /// Gets the value of the hours
        /// </summary>
        public uint HOUR { get; private set; }

        /// <summary>
        /// Gets the value of the minutes
        /// </summary>
        public uint MINUTE { get; private set; }

        /// <summary>
        /// Gets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;60
        /// </exception>
        public uint SECOND { get; private set; }

        /// <summary>
        /// Gets the time format of the date-time type.
        /// </summary>
        public TimeType Type { get; private set; }

        /// <summary>
        /// Gets the time zone identifier of the date-time type.
        /// </summary>
        public TZID TimeZoneId { get; private set; }

        public DATE_TIME AddSeconds(double value) => this.AsDateTime().AddSeconds(value).AsDATE_TIME(TimeZoneId);

        public DATE_TIME AddMinutes(double value) => this.AsDateTime()
    .AddMinutes(value)
    .AsDATE_TIME(TimeZoneId);

        public DATE_TIME AddHours(double value) => this.AsDateTime()
    .AddHours(value)
    .AsDATE_TIME(TimeZoneId);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DATE_TIME && Equals((DATE_TIME)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)HOUR;
                hashCode = (hashCode * 397) ^ (int)MINUTE;
                hashCode = (hashCode * 397) ^ (int)SECOND;
                hashCode = (hashCode * 397) ^ (int)FULLYEAR;
                hashCode = (hashCode * 397) ^ (int)MONTH;
                hashCode = (hashCode * 397) ^ (int)MDAY;
                hashCode = (hashCode * 397) ^ (TimeZoneId != null ? TimeZoneId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool CanSerialize() => true;

        #region overloaded operators

        /// <summary>
        /// Adds a specified time duration to a specified date and time, yielding a new <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="start">The <see cref="DATE_TIME"/> value to add to.</param>
        /// <param name="duration">The time duration to add.</param>
        /// <returns>The sum of the <see cref="start"/> and <see cref="duration"/>.</returns>
        public static DATE_TIME operator +(DATE_TIME start, DURATION duration) => (start.AsDateTime().Add(duration.AsTimeSpan())).AsDATE_TIME(start.TimeZoneId);

        /// <summary>
        /// Substracts a specified time duration from a specified date and time, yielding a new <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="end">The <see cref="DATE_TIME"/> value to substract from.</param>
        /// <param name="duration">The time duration to substract.</param>
        /// <returns>The value of the <see cref="DATE_TIME"/> object minus the <see cref="duration"/>.</returns>
        public static DATE_TIME operator -(DATE_TIME end, DURATION duration) => (end.AsDateTime().Subtract(duration.AsTimeSpan())).AsDATE_TIME(end.TimeZoneId);

        public static DURATION operator -(DATE_TIME end, DATE_TIME start)
        {
            if (end > start) return new DURATION(end.AsDateTime() - start.AsDateTime());
            if (end < start) return new DURATION(start.AsDateTime() - end.AsDateTime());
            return new DURATION();
        }

        public static bool operator <(DATE_TIME a, DATE_TIME b) => a.CompareTo(b) < 0;

        public static bool operator >(DATE_TIME a, DATE_TIME b) => a.CompareTo(b) > 0;

        public static bool operator <=(DATE_TIME a, DATE_TIME b) => a.CompareTo(b) <= 0;

        public static bool operator >=(DATE_TIME a, DATE_TIME b) => a.CompareTo(b) >= 0;

        public static bool operator ==(DATE_TIME left, DATE_TIME right) => left.Equals(right);

        public static bool operator !=(DATE_TIME left, DATE_TIME right) => !left.Equals(right);

        #endregion overloaded operators
    }

    [DataContract]
    public struct TIME : ITIME, ITIME<TIME>, IEquatable<TIME>, IComparable<TIME>, ICalendarSerializable
    {
        public TIME(uint hour, uint minute, uint second, TimeType type = TimeType.Local, TZID tzid = null)
        {
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Type = type;
            TimeZoneId = tzid;
        }

        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            TimeZoneId = null;
            Type = TimeType.NONE;
            if (tzinfo != null)
            {
                TimeZoneId = new TZID(null, tzinfo.Id);
                Type = TimeType.LocalAndTimeZone;
            }
            else Type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) Type = TimeType.Utc;
        }

        public TIME(DateTimeOffset datetime)
        {
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            Type = TimeType.Utc;
            TimeZoneId = null;
        }

        public TIME(string value)
        {
            var time = Parse(value);
            HOUR = time.HOUR;
            MINUTE = time.MINUTE;
            SECOND = time.SECOND;
            TimeZoneId = time.TimeZoneId;
            Type = time.Type;
        }

        private static TIME Parse(string value)
        {
            var hour = 0u;
            var minute = 0u;
            var second = 0u;
            TZID tzid = null;
            var type = TimeType.NONE;

            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z)?$";
            var options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (Regex.IsMatch(value, pattern))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["hour"].Success) hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success)
                    {
                        if (match.Groups["utc"].Value.Equals("Z", StringComparison.OrdinalIgnoreCase))
                            type = TimeType.Utc;
                        else if (match.Groups["utc"].Value.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
                            type = TimeType.Local;
                    }
                    if (match.Groups["tzid"].Success)
                    {
                        tzid = new TZID(match.Groups["tzid"].Value);
                        type = TimeType.LocalAndTimeZone;
                    }
                }
            }

            return new TIME(hour, minute, second, type, tzid);
        }

        public TIME(DATE_TIME datetime)
        {
            HOUR = datetime.HOUR;
            MINUTE = datetime.MINUTE;
            SECOND = datetime.SECOND;
            Type = datetime.Type;
            TimeZoneId = datetime.TimeZoneId;
        }

        public TIME(ITIME time, TZID tzid = null)
        {
            if (time == null) throw new ArgumentNullException("time");
            HOUR = time.HOUR;
            MINUTE = time.MINUTE;
            SECOND = time.SECOND;
            Type = time.Type;
            TimeZoneId = tzid == null ? time.TimeZoneId : tzid;
        }

        public int CompareTo(TIME other)
        {
            if (HOUR > other.HOUR) return 1;
            if (MINUTE < other.MINUTE) return -1;
            if (MINUTE > other.MINUTE) return 1;
            if (SECOND < other.SECOND) return -1;
            if (SECOND > other.SECOND) return 1;
            return 0;
        }

        public bool Equals(TIME other) => HOUR == other.HOUR &&
       MINUTE == other.MINUTE &&
       SECOND == other.SECOND &&
       Type == other.Type &&
       Equals(TimeZoneId, other.TimeZoneId);

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;23
        /// </exception>
        public uint HOUR { get; private set; }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;59
        /// </exception>
        public uint MINUTE { get; private set; }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;60
        /// </exception>
        public uint SECOND { get; private set; }

        public TimeType Type { get; private set; }

        public TZID TimeZoneId { get; private set; }

        public TIME AddSeconds(double value) => this.AsDateTime().AddSeconds(value).AsTIME(TimeZoneId);

        public TIME AddMinutes(double value) => this.AsDateTime().AddMinutes(value).AsTIME(TimeZoneId);

        public TIME AddHours(double value) => this.AsDateTime().AddHours(value).AsTIME(TimeZoneId);

        public override string ToString()
        {
            if (Type == TimeType.Local) return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";
            if (Type == TimeType.Utc)
                return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z";
            if (Type == TimeType.LocalAndTimeZone)
                return string.Format("{0}:T{0:D2}{1:D2}{2:D2}", TimeZoneId, HOUR, MINUTE, SECOND);
            return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TIME && Equals((TIME)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)HOUR;
                hashCode = (hashCode * 397) ^ (int)MINUTE;
                hashCode = (hashCode * 397) ^ (int)SECOND;
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (TimeZoneId != null ? TimeZoneId.GetHashCode() : 0);
                return hashCode;
            }
        }

        #region overloaded operators

        public static TIME operator +(TIME start, DURATION duration) => (start.AsTimeSpan().Add(duration.AsTimeSpan())).AsTIME(start.TimeZoneId);

        public static TIME operator -(TIME end, DURATION duration) => (end.AsTimeSpan().Subtract(duration.AsTimeSpan())).AsTIME(end.TimeZoneId);

        public static DURATION operator -(TIME start, TIME end) => new DURATION(end.AsTimeSpan() - start.AsTimeSpan());

        public static bool operator <(TIME a, TIME b) => a.CompareTo(b) < 0;

        public static bool operator >(TIME a, TIME b) => a.CompareTo(b) > 0;

        public static bool operator <=(TIME a, TIME b) => a.CompareTo(b) <= 0;

        public static bool operator >=(TIME a, TIME b) => a.CompareTo(b) >= 0;

        public static bool operator ==(TIME a, TIME b) => a.Equals(b);

        public static bool operator !=(TIME a, TIME b) => !a.Equals(b);

        #endregion overloaded operators

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        /// <summary>
        /// Converts an object into its iCalendar representation.
        /// </summary>
        /// <param name="writer">The iCalendar writer used to serialize the object.</param>
        public void WriteCalendar(CalendarWriter writer)
        {
            switch (Type)
            {
                case TimeType.Local:
                    writer.Write($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                case TimeType.Utc:
                    writer.Write($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z");
                    break;

                case TimeType.LocalAndTimeZone:
                    writer.Write($"{TimeZoneId}:T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                default:
                    writer.Write($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;
            }
        }

        /// <summary>
        /// Generates an object from its iCalendar representation.
        /// </summary>
        /// <param name="reader">
        /// The iCalendar reader used to deserialize data into the iCalendar object.
        /// </param>
        /// <returns>True if the deserialization operation was successful; otherwise false.</returns>
        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var time = Parse(inner.Value);
                    HOUR = time.HOUR;
                    MINUTE = time.MINUTE;
                    SECOND = time.SECOND;
                    Type = time.Type;
                    TimeZoneId = time.TimeZoneId;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public struct DURATION : IDURATION, IEquatable<DURATION>, IComparable<DURATION>, ICalendarSerializable
    {
        /// <summary>
        /// </summary>
        /// <param name="duration"></param>
        public DURATION(DURATION duration)
        {
            WEEKS = duration.WEEKS;
            DAYS = duration.DAYS;
            HOURS = duration.HOURS;
            MINUTES = duration.MINUTES;
            SECONDS = duration.SECONDS;
        }

        /// <summary>
        /// </summary>
        /// <param name="weeks"></param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public DURATION(int weeks, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            WEEKS = weeks;
            DAYS = days;
            HOURS = hours;
            MINUTES = minutes;
            SECONDS = seconds;
        }

        /// <summary>
        /// </summary>
        /// <param name="span"></param>
        public DURATION(TimeSpan span)
        {
            DAYS = span.Days;
            HOURS = span.Hours;
            MINUTES = span.Minutes;
            SECONDS = span.Seconds;
            WEEKS = span.Days
                    + (span.Hours / 24)
                    + (span.Minutes / (24 * 60))
                    + (span.Seconds / (24 * 3600))
                    + (span.Milliseconds / (24 * 3600000)) / 7;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public DURATION(string value)
        {
            var duration = Parse(value);
            WEEKS = duration.WEEKS;
            DAYS = duration.DAYS;
            HOURS = duration.HOURS;
            MINUTES = duration.MINUTES;
            SECONDS = duration.SECONDS;
        }

        private static DURATION Parse(string value)
        {
            int weeks = 0;
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            var pattern =
                @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
            if (Regex.IsMatch(value, pattern,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (
                    Match match in
                        Regex.Matches(value, pattern,
                            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["weeks"].Success) weeks = int.Parse(match.Groups["weeks"].Value);
                    if (match.Groups["days"].Success) days = int.Parse(match.Groups["days"].Value);
                    if (match.Groups["hours"].Success) hours = int.Parse(match.Groups["hours"].Value);
                    if (match.Groups["mins"].Success) minutes = int.Parse(match.Groups["mins"].Value);
                    if (match.Groups["secs"].Success) seconds = int.Parse(match.Groups["secs"].Value);
                    if (match.Groups["minus"].Success)
                    {
                        weeks = -weeks;
                        days = -days;
                        minutes = -minutes;
                        seconds = -seconds;
                    }
                }
            }

            return new DURATION(weeks, days, hours, minutes, seconds);
        }

        /// <summary>
        /// </summary>
        /// <param name="duration"></param>
        public DURATION(IDURATION duration)
        {
            if (duration == null) throw new ArgumentNullException("duration");
            WEEKS = duration.WEEKS;
            DAYS = duration.DAYS;
            HOURS = duration.HOURS;
            MINUTES = duration.MINUTES;
            SECONDS = duration.SECONDS;
        }

        public int CompareTo(DURATION other) => this.AsTimeSpan().CompareTo(other.AsTimeSpan());

        /// <summary>
        /// Gets the duration in weeks
        /// </summary>
        public int WEEKS { get; private set; }

        /// <summary>
        /// Gets the duration in hours
        /// </summary>
        public int HOURS { get; private set; }

        /// <summary>
        /// Gets the duration in minutes
        /// </summary>
        public int MINUTES { get; private set; }

        /// <summary>
        /// Gets the duration in seconds
        /// </summary>
        public int SECONDS { get; private set; }

        /// <summary>
        /// Gets the duration in days
        /// </summary>
        public int DAYS { get; private set; }

        public bool Equals(DURATION other) => WEEKS == other.WEEKS &&
       DAYS == other.DAYS &&
       HOURS == other.HOURS &&
       MINUTES == other.MINUTES &&
       SECONDS == other.SECONDS;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DURATION && Equals((DURATION)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = WEEKS;
                hashCode = (hashCode * 397) ^ DAYS;
                hashCode = (hashCode * 397) ^ HOURS;
                hashCode = (hashCode * 397) ^ MINUTES;
                hashCode = (hashCode * 397) ^ SECONDS;
                return hashCode;
            }
        }

        #region overloaded operators

        public static DURATION operator -(DURATION duration) => new DURATION(-duration.WEEKS, -duration.DAYS, -duration.HOURS, -duration.MINUTES, -duration.SECONDS);

        public static DURATION operator +(DURATION duration) => new DURATION(duration.WEEKS, duration.DAYS, duration.HOURS, duration.MINUTES, duration.SECONDS);

        public static DURATION operator +(DURATION a, DURATION b) => new DURATION(a.WEEKS + b.WEEKS, a.DAYS + b.DAYS, a.HOURS + b.HOURS, a.MINUTES + b.MINUTES,
    a.SECONDS + b.SECONDS);

        public static DURATION operator -(DURATION a, DURATION b) => new DURATION(a.WEEKS - b.WEEKS, a.DAYS - b.DAYS, a.HOURS - b.HOURS, a.MINUTES - b.MINUTES,
    a.SECONDS - b.SECONDS);

        public static DURATION operator *(DURATION duration, int scalar) => new DURATION(duration.WEEKS * scalar, duration.DAYS * scalar, duration.HOURS * scalar,
    duration.MINUTES * scalar, duration.SECONDS * scalar);

        public static DURATION operator /(DURATION duration, int scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Zero dividend is forbidden!");
            return new DURATION(duration.WEEKS / scalar, duration.DAYS / scalar, duration.HOURS / scalar,
                duration.MINUTES / scalar, duration.SECONDS / scalar);
        }

        public static bool operator <(DURATION a, DURATION b) => a.CompareTo(b) < 0;

        public static bool operator >(DURATION a, DURATION b) => a.CompareTo(b) > 0;

        public static bool operator <=(DURATION a, DURATION b) => a.CompareTo(b) <= 0;

        public static bool operator >=(DURATION a, DURATION b) => a.CompareTo(b) >= 0;

        public static bool operator ==(DURATION a, DURATION b) => a.Equals(b);

        public static bool operator !=(DURATION a, DURATION b) => !a.Equals(b);

        #endregion overloaded operators

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            var sb = new StringBuilder();
            var sign = (WEEKS < 0 || DAYS < 0 || HOURS < 0 || MINUTES < 0 || SECONDS < 0) ? "-" : string.Empty;
            sb.AppendFormat("{0}P", sign);
            if (WEEKS != 0) sb.AppendFormat("{0}W", WEEKS);
            if (DAYS != 0) sb.AppendFormat("{0}D", DAYS);
            if (HOURS != 0 || MINUTES != 0 || SECONDS != 0) sb.Append("T");
            if (HOURS != 0) sb.AppendFormat("{0}H", HOURS);
            if (MINUTES != 0) sb.AppendFormat("{0}M", MINUTES);
            if (SECONDS != 0) sb.AppendFormat("{0}S", SECONDS);
            writer.Write(sb.ToString());
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var duration = Parse(inner.Value);
                    WEEKS = duration.WEEKS;
                    DAYS = duration.DAYS;
                    HOURS = duration.HOURS;
                    MINUTES = duration.MINUTES;
                    SECONDS = duration.SECONDS;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public struct WEEKDAYNUM : IWEEKDAYNUM, IEquatable<WEEKDAYNUM>, IComparable<WEEKDAYNUM>, ICalendarSerializable
    {
        /// <summary>
        /// </summary>
        /// <param name="weekday"></param>
        public WEEKDAYNUM(WEEKDAY weekday)
        {
            NthOccurrence = 0;
            Weekday = weekday;
        }

        /// <summary>
        /// </summary>
        /// <param name="ordweek"></param>
        /// <param name="weekday"></param>
        public WEEKDAYNUM(int ordweek, WEEKDAY weekday)
        {
            NthOccurrence = ordweek;
            Weekday = weekday;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public WEEKDAYNUM(string value)
        {
            var weekdaynum = Parse(value);
            NthOccurrence = weekdaynum.NthOccurrence;
            Weekday = weekdaynum.Weekday;
        }

        private static WEEKDAYNUM Parse(string value)
        {
            var ordweek = 0;
            var weekday = WEEKDAY.NONE;

            var pattern = @"^((?<minus>\-)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
            var options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (Regex.IsMatch(value, pattern, options))
            {
                var mulitplier = 1;
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["minus"].Success && match.Groups["minus"].Value == "-")
                        mulitplier *= -1;
                    if (match.Groups["ordwk"].Success)
                        ordweek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
                    if (match.Groups["weekday"].Success)
                        weekday = (WEEKDAY)Enum.Parse(typeof(WEEKDAY), match.Groups["weekday"].Value);
                }
            }

            return new WEEKDAYNUM(ordweek, weekday);
        }

        /// <summary>
        /// </summary>
        /// <param name="weekdaynum"></param>
        public WEEKDAYNUM(IWEEKDAYNUM weekdaynum)
        {
            if (weekdaynum == null) throw new ArgumentNullException("weekdaynum");
            NthOccurrence = weekdaynum.NthOccurrence;
            Weekday = weekdaynum.Weekday;
        }

        public int CompareTo(WEEKDAYNUM other)
        {
            if (NthOccurrence == 0 && other.NthOccurrence == 0) return Weekday.CompareTo(other.Weekday);
            if (NthOccurrence < other.NthOccurrence) return -1;
            if (NthOccurrence > other.NthOccurrence) return 1;
            return Weekday.CompareTo(other.Weekday);
        }

        public bool Equals(WEEKDAYNUM other) => NthOccurrence == other.NthOccurrence && Weekday == other.Weekday;

        /// <summary>
        /// Gets or sets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        public int NthOccurrence { get; private set; }

        /// <summary>
        /// Gets or sets the weekday
        /// </summary>
        public WEEKDAY Weekday { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals((WEEKDAYNUM)obj);
        }

        public override int GetHashCode() => NthOccurrence.GetHashCode() ^ Weekday.GetHashCode();

        public static bool operator ==(WEEKDAYNUM a, WEEKDAYNUM b) => a.Equals(b);

        public static bool operator !=(WEEKDAYNUM a, WEEKDAYNUM b) => !a.Equals(b);

        public static bool operator <(WEEKDAYNUM a, WEEKDAYNUM b) => a.CompareTo(b) < 0;

        public static bool operator >(WEEKDAYNUM a, WEEKDAYNUM b) => a.CompareTo(b) > 0;

        public static bool operator <=(WEEKDAYNUM a, WEEKDAYNUM b) => a.CompareTo(b) <= 0;

        public static bool operator >=(WEEKDAYNUM a, WEEKDAYNUM b) => a.CompareTo(b) >= 0;

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            if (NthOccurrence != 0)
            {
                writer.Write((NthOccurrence < 0)
                    ? $"-{(uint)NthOccurrence} {Weekday}"
                    : $"+{(uint)NthOccurrence} {Weekday}");
            }
            else writer.Write($"{Weekday}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var weekdaynum = Parse(inner.Value);
                    NthOccurrence = weekdaynum.NthOccurrence;
                    Weekday = weekdaynum.Weekday;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public struct UTC_OFFSET : IUTC_OFFSET, IEquatable<UTC_OFFSET>, IComparable<UTC_OFFSET>, ICalendarSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct from a serialized string.
        /// </summary>
        /// <param name="value">A serialized string representing a <see cref="UTC_OFFSET"/> struct.</param>
        public UTC_OFFSET(string value)
        {
            var offset = Parse(value);
            HOUR = offset.HOUR;
            MINUTE = offset.MINUTE;
            SECOND = offset.SECOND;
        }

        public static UTC_OFFSET Parse(string value)
        {
            var hour = 0;
            var minute = 0;
            var second = 0;
            var pattern = @"^(?<minus>\-|?<plus>\+)(?<hours>\d{1,2})(?<mins>\d{1,2})(?<secs>\d{1,2})?$";
            var options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (Regex.IsMatch(value, pattern, options))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["hours"].Success) hour = int.Parse(match.Groups["hours"].Value);
                    if (match.Groups["mins"].Success) minute = int.Parse(match.Groups["mins"].Value);
                    if (match.Groups["secs"].Success) second = int.Parse(match.Groups["secs"].Value);
                    if (match.Groups["minus"].Success)
                    {
                        hour = -hour;
                        minute = -minute;
                        second = -second;
                    }
                }
            }

            return new UTC_OFFSET(hour, minute, second);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct.
        /// </summary>
        /// <param name="hour">The hour to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        /// <param name="minute">The minute to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        /// <param name="second">The second to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        public UTC_OFFSET(int hour, int minute, int second)
        {
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct from a <see
        /// cref="IUTC_OFFSET"/> instance.
        /// </summary>
        /// <param name="offset"></param>
        public UTC_OFFSET(IUTC_OFFSET offset)
        {
            HOUR = offset.HOUR;
            MINUTE = offset.MINUTE;
            SECOND = offset.SECOND;
        }

        public int CompareTo(UTC_OFFSET other)
        {
            if (HOUR < other.HOUR) return -1;
            if (HOUR > other.HOUR) return 1;
            if (MINUTE < other.MINUTE) return -1;
            if (MINUTE > other.MINUTE) return 1;
            if (SECOND < other.SECOND) return -1;
            if (SECOND > other.SECOND) return 1;
            return 0;
        }

        public bool Equals(UTC_OFFSET other) => HOUR == other.HOUR &&
       MINUTE == other.MINUTE &&
       SECOND == other.SECOND;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;23
        /// </exception>
        public int HOUR { get; private set; }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        public int MINUTE { get; private set; }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value &lt;0 and value &gt;60
        /// </exception>
        public int SECOND { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UTC_OFFSET && Equals((UTC_OFFSET)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HOUR;
                hashCode = (hashCode * 397) ^ MINUTE;
                hashCode = (hashCode * 397) ^ SECOND;
                return hashCode;
            }
        }

        #region overloaded operators

        public static UTC_OFFSET operator +(UTC_OFFSET a, UTC_OFFSET b) => new UTC_OFFSET((a.HOUR + b.HOUR) % 24, (a.MINUTE + b.MINUTE) % 60, (a.SECOND + b.SECOND) % 60);

        public static UTC_OFFSET operator -(UTC_OFFSET a, UTC_OFFSET b) => new UTC_OFFSET((a.HOUR - b.HOUR).Modulo(60), (a.MINUTE - b.MINUTE).Modulo(60),
    (a.SECOND - b.SECOND).Modulo(60));

        public static bool operator <(UTC_OFFSET a, UTC_OFFSET b) => a.CompareTo(b) < 0;

        public static bool operator >(UTC_OFFSET a, UTC_OFFSET b) => a.CompareTo(b) > 0;

        public static bool operator <=(UTC_OFFSET a, UTC_OFFSET b) => a.CompareTo(b) <= 0;

        public static bool operator >=(UTC_OFFSET a, UTC_OFFSET b) => a.CompareTo(b) >= 0;

        public static bool operator ==(UTC_OFFSET left, UTC_OFFSET right) => left.Equals(right);

        public static bool operator !=(UTC_OFFSET left, UTC_OFFSET right) => !left.Equals(right);

        #endregion overloaded operators

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write((HOUR < 0 || MINUTE < 0 || SECOND < 0)
                ? $"-{HOUR:D2}{MINUTE:D2}{SECOND:D2}"
                : $"+{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var offset = Parse(inner.Value);
                    HOUR = offset.HOUR;
                    MINUTE = offset.MINUTE;
                    SECOND = offset.SECOND;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public struct PERIOD : IPERIOD, IEquatable<PERIOD>, IComparable<PERIOD>, ICalendarSerializable
    {
        private PeriodType type;

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public PERIOD(string value)
        {
            var period = Parse(value);
            Start = period.Start;
            End = period.End;
            Duration = period.Duration;
            type = Duration == default(DURATION) ? PeriodType.Explicit : PeriodType.Start;
        }

        public static PERIOD Parse(string value)
        {
            var datetimePattern = @"((TZID=(\w+)?/(\w+)):)?(\d{4,})(\d{2})(\d{2})T(\d{2})(\d{2})(\d{2})Z?";
            var durationPattern = @"(\-)?P((\d*)W)?((\d*)D)?(T((\d*)H)?((\d*)M)?((\d*)S)?)?";
            var explicitPattern = string.Format("{0}/{0}", datetimePattern);
            var startPattern = $"{datetimePattern}/{durationPattern}";

            var pattern = $@"^(?<periodExplicit>{explicitPattern})|(?<periodStart>{startPattern})$";
            var options = RegexOptions.IgnoreCase 
                | RegexOptions.ExplicitCapture 
                | RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.CultureInvariant;

            var regex = new Regex(pattern, options);

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["periodExplicit"].Success)
                {
                    var periodExplicit = match.Groups["periodExplicit"].Value;
                    var parts = periodExplicit.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var start = new DATE_TIME(parts[0]);
                    var end = new DATE_TIME(parts[1]);
                    return new PERIOD(start, end);
                }
                if (match.Groups["periodStart"].Success)
                {
                    var periodStart = match.Groups["periodStart"].Value;
                    var parts = periodStart.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var start = new DATE_TIME(parts[0]);
                    var duration = new DURATION(parts[1]);
                    return new PERIOD(start, duration);
                }
            }

            return default(PERIOD);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public PERIOD(DATE_TIME start, DATE_TIME end)
        {
            Start = start;
            End = end;
            Duration = end - start;
            type = PeriodType.Explicit;
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        public PERIOD(DATE_TIME start, DURATION duration)
        {
            Start = start;
            Duration = duration;
            End = start + duration;
            type = PeriodType.Start;
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startTimeZoneInfo"></param>
        /// <param name="endTimeZoneInfo"></param>
        public PERIOD(DateTime start, DateTime end, TimeZoneInfo startTimeZoneInfo = null,
            TimeZoneInfo endTimeZoneInfo = null)
        {
            Start = new DATE_TIME(start, startTimeZoneInfo);
            End = new DATE_TIME(end, endTimeZoneInfo);
            Duration = End - Start;
            type = PeriodType.Explicit;
        }

        public PERIOD(DateTime start, TimeSpan span, TimeZoneInfo timeZoneInfo = null)
        {
            Start = new DATE_TIME(start, timeZoneInfo);
            Duration = new DURATION(span);
            End = Start + Duration;
            type = PeriodType.Start;
        }

        public PERIOD(IPERIOD period)
        {
            if (period == null) throw new ArgumentNullException("period");
            Start = period.Start;
            End = period.End;
            Duration = period.Duration;
            type = PeriodType.Explicit;
        }

        public int CompareTo(PERIOD other) => (type == PeriodType.Explicit)
    ? Start.CompareTo(other.Start) + End.CompareTo(other.End)
    : Start.CompareTo(other.Start) + Duration.CompareTo(other.Duration);

        public bool Equals(PERIOD other) => Start.Equals(other.Start) &&
       End.Equals(other.End) &&
       Duration.Equals(other.Duration) &&
       type == other.type;

        /// <summary>
        /// Gets or sets the start of the period
        /// </summary>
        public DATE_TIME Start { get; private set; }

        /// <summary>
        /// Gets or sets the end of the period.
        /// </summary>
        public DATE_TIME End { get; private set; }

        /// <summary>
        /// Gets or sets the duration of the period.
        /// </summary>
        public DURATION Duration { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PERIOD && Equals((PERIOD)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)type;
                return hashCode;
            }
        }

        #region overloaded operators

        public static bool operator <(PERIOD a, PERIOD b) => a.CompareTo(b) < 0;

        public static bool operator >(PERIOD a, PERIOD b) => a.CompareTo(b) > 0;

        public static bool operator <=(PERIOD a, PERIOD b) => a.CompareTo(b) <= 0;

        public static bool operator >=(PERIOD a, PERIOD b) => a.CompareTo(b) >= 0;

        public static bool operator ==(PERIOD a, PERIOD b) => a.Equals(b);

        public static bool operator !=(PERIOD a, PERIOD b) => !a.Equals(b);

        #endregion overloaded operators

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize() => true;

        public void WriteCalendar(CalendarWriter writer)
        {
            Start.WriteCalendar(writer);
            writer.Write("/");

            if (type == PeriodType.Start) End.WriteCalendar(writer);
            else Duration.WriteCalendar(writer);
        }

        public void ReadCalendar(CalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.CanRead())
            {
                if (inner.NodeType != CalendarNodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var period = Parse(inner.Value);
                    Start = period.Start;
                    End = period.End;
                    Duration = period.Duration;
                    type = Duration == default(DURATION) ? PeriodType.Explicit : PeriodType.Start;
                }
            }
        }

        public bool CanSerialize() => true;
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public class RECUR : IRECUR, IEquatable<RECUR>, IContainsKey<Guid>, ICalendarSerializable
    {
        /// <summary>
        /// </summary>
        public RECUR()
        {
            FREQ = FREQ.NONE;
            UNTIL = default(DATE_TIME);
            COUNT = 0u;
            INTERVAL = 1u;
            WKST = WEEKDAY.SU;

            BYMONTH = new List<uint>();
            BYWEEKNO = new List<int>();
            BYYEARDAY = new List<int>();
            BYMONTHDAY = new List<int>();
            BYDAY = new List<WEEKDAYNUM>();
            BYHOUR = new List<uint>();
            BYMINUTE = new List<uint>();
            BYSECOND = new List<uint>();
            BYSETPOS = new List<int>();
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public RECUR(string value) : this()
        {
            FREQ = FREQ.NONE;
            UNTIL = default(DATE_TIME);
            COUNT = 0u;
            INTERVAL = 1u;
            WKST = WEEKDAY.SU;

            var tokens = value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens == null || tokens.Length == 0) throw new FormatException("Invalid Recur format");

            foreach (var token in tokens)
            {
                const string pattern =
                    @"^(FREQ|UNTIL|COUNT|INTERVAL|BYSECOND|BYMINUTE|BYHOUR|BYMONTHDAY|BYYEARDAY|BYWEEKNO|BYMONTH|WKST|BYSETPOS)=((\w+|\d+)(,[\s]*(\w+|\d+))*)$";
                if (
                    !Regex.IsMatch(token, pattern,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                    continue;

                var pair = token.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                //check FREQ
                if (pair[0].Equals("FREQ", StringComparison.OrdinalIgnoreCase))
                    FREQ = (FREQ)Enum.Parse(typeof(FREQ), pair[1], true);
                if (pair[0].Equals("UNTIL", StringComparison.OrdinalIgnoreCase)) UNTIL = new DATE_TIME(pair[1]);
                if (pair[0].Equals("COUNT", StringComparison.OrdinalIgnoreCase)) COUNT = uint.Parse(pair[1]);
                if (pair[0].Equals("INTERVAL", StringComparison.OrdinalIgnoreCase)) INTERVAL = uint.Parse(pair[1]);
                if (pair[0].Equals("BYSECOND", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYSECOND = parts.Select(uint.Parse).ToList();
                }
                if (pair[0].Equals("BYMINUTE", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYMINUTE = parts.Select(uint.Parse).ToList();
                }
                if (pair[0].Equals("BYHOUR", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYMINUTE = parts.Select(uint.Parse).ToList();
                }

                if (pair[0].Equals("BYDAY", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYDAY = parts.Select(x => new WEEKDAYNUM(x)).ToList();
                }

                if (pair[0].Equals("BYMONTHDAY", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYMONTHDAY = parts.Select(int.Parse).ToList();
                }

                if (pair[0].Equals("BYYEARDAY", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYYEARDAY = parts.Select(int.Parse).ToList();
                }

                if (pair[0].Equals("BYWEEKNO", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYWEEKNO = parts.Select(int.Parse).ToList();
                }

                if (pair[0].Equals("BYMONTH", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYMONTH = parts.Select(uint.Parse).ToList();
                }

                if (pair[0].Equals("WKST", StringComparison.OrdinalIgnoreCase))
                    WKST = (WEEKDAY)Enum.Parse(typeof(WEEKDAY), pair[1], true);

                if (pair[0].Equals("BYSETPOS", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYSETPOS = parts.Select(int.Parse).ToList();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="until"></param>
        public RECUR(FREQ freq, DATE_TIME until) : this()
        {
            FREQ = freq;
            UNTIL = until;
        }

        /// <summary>
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="count"></param>
        /// <param name="interval"></param>
        public RECUR(FREQ freq, uint count, uint interval)
        {
            FREQ = freq;
            COUNT = count;
            INTERVAL = interval;
        }

        /// <summary>
        /// </summary>
        /// <param name="recur"></param>
        public RECUR(IRECUR recur)
        {
            if (recur != null)
            {
                FREQ = recur.FREQ;
                UNTIL = recur.UNTIL;
                COUNT = recur.COUNT;
                INTERVAL = recur.INTERVAL;
                WKST = recur.WKST;
                BYSECOND = new List<uint>(recur.BYSECOND);
                BYMINUTE = new List<uint>(recur.BYMINUTE);
                BYHOUR = new List<uint>(recur.BYHOUR);
                BYDAY = new List<WEEKDAYNUM>(recur.BYDAY);
                BYMONTHDAY = new List<int>(recur.BYMONTHDAY);
                BYYEARDAY = new List<int>(recur.BYYEARDAY);
                BYWEEKNO = new List<int>(recur.BYWEEKNO);
                BYMONTH = new List<uint>(recur.BYMONTH);
                BYSETPOS = new List<int>(recur.BYSETPOS);
            }
        }

        public bool Equals(RECUR other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RECUR)obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(RECUR left, RECUR right) => Equals(left, right);

        public static bool operator !=(RECUR left, RECUR right) => !Equals(left, right);

        #region properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the recurrence rule. This property is REQUIRED and must not
        /// occur more than once.
        /// </summary>
        [DataMember]
        public FREQ FREQ { get; set; }

        /// <summary>
        /// Gets or sets the end date that bounds the reczrrence rule in an INCLUSIVE manner. This
        /// property is OPTIONAL but MUST be nullified if the COUNT property is non-zero.
        /// </summary>
        [DataMember]
        public DATE_TIME UNTIL { get; set; }

        /// <summary>
        /// Gets or sets the number of occurrences at which to range-bound the recurrence. This
        /// property is OPTIONAL but MUST be set to zero if the UNTIL property is non-null.
        /// </summary>
        [DataMember]
        public uint COUNT { get; set; }

        /// <summary>
        /// Represents the intervals the recurrence rule repeats. The default value is 1. For example
        /// in a SECONDLY rule, this property implies a repetition for every second.
        /// </summary>
        [DataMember]
        public uint INTERVAL { get; set; }

        /// <summary>
        /// Gets or sets the collection of seconds within a minute. Valid values are 0 to 60.
        /// </summary>
        /// <remarks>
        /// Normally, the seconds range is from 0 to 59 but the extra 60th second accounts for a leap
        /// second, which ma ybe ignored by a non-supporting system.
        /// </remarks>
        [DataMember]
        public List<uint> BYSECOND { get; set; }

        /// <summary>
        /// Gets or sets the collection of minutes within an hour. Valid values are from 0 to 59.
        /// </summary>
        [DataMember]
        public List<uint> BYMINUTE { get; set; }

        /// <summary>
        /// Gets or sets the collection of hours within a day. Valid values are from 0 to 23.
        /// </summary>
        [DataMember]
        public List<uint> BYHOUR { get; set; }

        [DataMember]
        public List<WEEKDAYNUM> BYDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the month. Valid values are 1 to 31 or -31 to -1.
        /// Negative values represent the day position from the end of the month e.g -10 is the tenth
        /// to the last day of the month
        /// </summary>
        [DataMember]
        public List<int> BYMONTHDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the year. Valid values are 1 to 366 or -366 to -1.
        /// Negative values represent the day position from the end of the year e.g -306 is the 306th
        /// to the last day of the month. This property MUST NOT be specified when the FREQ property
        /// is set to DAILY, WEEKLY, or MONTHLY
        /// </summary>
        [DataMember]
        public List<int> BYYEARDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of ordinals specifiying the weeks of the year. Valid values
        /// are 1 to 53 or -53 to -1. This property MUST NOT be specified when the FREQ property is
        /// set to anything other than YEARLY.
        /// </summary>
        [DataMember]
        public List<int> BYWEEKNO { get; set; }

        /// <summary>
        /// Gets or sets the collection of months of the year. Valid values are 1 to 12.
        /// </summary>
        [DataMember]
        public List<uint> BYMONTH { get; set; }

        /// <summary>
        /// Gets or sets the day on which the work week starts. The default day is MO.
        /// </summary>
        /// <remarks>
        /// This is used significantly when a weekly recurrence rule has an interval greater than 1
        /// and a BYDAY rule is specified. Also, it is significantly used when a BYWEEKNO rule in a
        /// YEARLY rule is specified.
        /// </remarks>
        [DataMember]
        public WEEKDAY WKST { get; set; }

        /// <summary>
        /// Gets or sets the list of values corresponfing to the nth occurence within the set of
        /// recurrence instances in an interval of the recurrence rule. Valid values are 1 to 366 or
        /// -366 to -1 This property MUST only be used together with another BYxxx rule part (e.g. BYDAY)
        /// </summary>
        /// <remarks>
        /// Example: " the last work day of the month " FRREQ = MONTHLY; BYDAY = MO, TU, WE, TH, FR;
        ///          BYSETPOS = -1
        /// </remarks>
        [DataMember]
        public List<int> BYSETPOS { get; set; }

        #endregion properties

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize()
        {
            throw new NotImplementedException();
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.WriteParameter(nameof(FREQ), FREQ.ToString());

            if (UNTIL != default(DATE_TIME)) writer.AppendParameter(nameof(UNTIL), UNTIL);
            else if (COUNT != 0) writer.AppendParameter(nameof(COUNT), COUNT.ToString());
            writer.AppendParameter(nameof(INTERVAL), INTERVAL.ToString());
            if (BYSECOND.Any()) writer.AppendParameter(nameof(BYSECOND), BYSECOND.Select(x => x.ToString()));
            if (BYMINUTE.Any()) writer.AppendParameter(nameof(BYMINUTE), BYMINUTE.Select(x => x.ToString()));
            if (BYHOUR.Any()) writer.AppendParameter(nameof(BYHOUR), BYHOUR.Select(x => x.ToString()));
            if (BYDAY.Any()) writer.AppendParameter<WEEKDAYNUM>(nameof(BYDAY), BYDAY);
            if (BYMONTHDAY.Any()) writer.AppendParameter(nameof(BYMONTHDAY), BYMONTHDAY.Select(x => x.ToString()));
            if (BYYEARDAY.Any()) writer.AppendParameter(nameof(BYYEARDAY), BYYEARDAY.Select(x => x.ToString()));
            if (BYWEEKNO.Any()) writer.AppendParameter(nameof(BYWEEKNO), BYWEEKNO.Select(x => x.ToString()));
            if (BYMONTH.Any()) writer.AppendParameter(nameof(BYMONTH), BYMONTH.Select(x => x.ToString()));
            if (BYSETPOS.Any()) writer.AppendParameter(nameof(BYSETPOS), BYSETPOS.Select(x => x > 0 ? "+" + x.ToString() : x.ToString()));
            writer.AppendParameter(nameof(WKST), WKST.ToString());
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => true;
    }

    [DataContract]
    public class CAL_ADDRESS : Uri, IEquatable<CAL_ADDRESS>, ICalendarSerializable
    {
        public CAL_ADDRESS(CAL_ADDRESS other)
            : base(other.ToString().StartsWith("mailto")
                ? $"mailto:{other.ToString().Replace("mailto:", string.Empty)}"
                : other.ToString())
        {
        }

        public CAL_ADDRESS(string uriString, bool emailable = true)
            : base(emailable
                ? $"mailto:{uriString.Replace("mailto:", string.Empty)}"
                : uriString)
        {
        }

        public CAL_ADDRESS(string uriString, UriKind uriKind, bool emailable = true)
            : base(emailable ? $"mailto:{uriString.Replace("mailto:", string.Empty)}" : uriString, uriKind)
        {
        }

        public CAL_ADDRESS(Uri baseUri, string relativeUri, bool emailable = true)
            : base(
                emailable ? new Uri($"mailto:{baseUri.ToString().Replace("mailto:", string.Empty)}") : baseUri,
                relativeUri)
        {
        }

        public CAL_ADDRESS(Uri baseUri, Uri relativeUri, bool emailable = true) :
            base(
            emailable ? new Uri($"mailto:{baseUri.ToString().Replace("mailto:", string.Empty)}") : baseUri, relativeUri)
        {
        }

        public CAL_ADDRESS(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public Uri GetBase() => this;

        public bool Equals(CAL_ADDRESS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other.GetBase());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CAL_ADDRESS)obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(CAL_ADDRESS left, CAL_ADDRESS right) => Equals(left, right);

        public static bool operator !=(CAL_ADDRESS left, CAL_ADDRESS right) => !Equals(left, right);

        /// <summary>
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>True if the object can be deserialized from its iCalendar representation, otherwise false.</returns>
        public bool CanDeserialize()
        {
            throw new NotImplementedException();
        }

        public void WriteCalendar(CalendarWriter writer)
        {
            writer.Write(ToString());
        }

        public void ReadCalendar(CalendarReader reader)
        {
            throw new NotImplementedException();
        }

        public bool CanSerialize() => true;
    }
}
