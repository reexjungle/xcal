using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.extensions;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using reexjungle.xcal.infrastructure.contracts;
using reexjungle.xcal.infrastructure.serialization;

namespace reexjungle.xcal.domain.models
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain a character encoding of inline binary data.
    /// The character encoding is based on the Base64 encoding
    /// </summary>
    [DataContract]
    public class BINARY : IBINARY, IEquatable<BINARY>
    {
        /// <summary>
        /// Gets or sets the value of this type
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BINARY"/> class.
        /// </summary>
        public BINARY()
        {
            Value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BINARY"/> class from a string.
        /// </summary>
        /// <param name="value">The string representation of the &quot;Value&quot; of the <see cref="BINARY"/> class.</param>
        public BINARY(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            var pattern = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";

            var options = RegexOptions.IgnoreCase
                | RegexOptions.ExplicitCapture
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.CultureInvariant;

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BINARY"/> class.
        /// </summary>
        /// <param name="binary">An instance implementing the <see cref="IBINARY"/> interface.</param>
        /// <exception cref="System.ArgumentNullException">binary</exception>
        public BINARY(IBINARY binary)
        {
            if (binary != null)
            {
                Value = binary.Value;

            }        }

        private static bool IsBase64String(string @string)
        {
            if (string.IsNullOrEmpty(@string)) return false;
            try
            {
                var bytes = Convert.FromBase64String(@string.Trim());
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        public bool Equals(BINARY other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BINARY)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Implements the equality operator ==.
        /// </summary>
        /// <param name="left">The instance to compare for equality</param>
        /// <param name="right">The other instance to compare against this instance for equality.</param>
        /// <returns>
        /// True if both instances are equal, otherwise false
        /// </returns>
        public static bool operator ==(BINARY left, BINARY right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the equality operator !=.
        /// </summary>
        /// <param name="left">The instance to compare for equality</param>
        /// <param name="right">The other instance to compare against this instance for equality.</param>
        /// <returns>
        /// True if both instances are equal, otherwise false
        /// </returns>
        public static bool operator !=(BINARY left, BINARY right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    [DataContract]
    public struct DATE : IDATE, IDATE<DATE>, IEquatable<DATE>, IComparable<DATE>
    {
        private readonly uint fullyear, month, mday;

        /// <summary>
        /// Gets or sets the 4-digit representation of a full year e.g. 2013.
        /// </summary>
        public uint FULLYEAR
        {
            get { return fullyear; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month.
        /// </summary>
        public uint MONTH
        {
            get { return month; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a day of the month.
        /// </summary>
        public uint MDAY
        {
            get { return mday; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct.
        /// </summary>
        /// <param name="fullyear">The 4-digit representation of a full year e.g. 2013.</param>
        /// <param name="month">The 2-digit representation of a month.</param>
        /// <param name="mday">The 2-digit representation of a day of the month.</param>
        public DATE(uint fullyear, uint month, uint mday)
        {
            this.fullyear = fullyear;
            this.month = month;
            this.mday = mday;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from a <see cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance.</param>
        public DATE(DATE_TIME datetime)
        {
            fullyear = datetime.FULLYEAR;
            month = datetime.MONTH;
            mday = datetime.MDAY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> instance.</param>
        public DATE(DateTime datetime)
        {
            fullyear = (uint)datetime.Year;
            month = (uint)datetime.Month;
            mday = (uint)datetime.Day;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from a string.
        /// </summary>
        /// <param name="value">The string containing serialized information about the <see cref="DATE"/> struct.</param>
        public DATE(string value)
        {
            fullyear = 1u;
            month = 1u;
            mday = 1u;
            var pattern = @"^(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["year"].Success) fullyear = uint.Parse(match.Groups["year"].Value);
                    if (match.Groups["month"].Success) month = uint.Parse(match.Groups["month"].Value);
                    if (match.Groups["day"].Success) mday = uint.Parse(match.Groups["day"].Value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct from an instance implementing the <see cref="IDATE"/> interface.
        /// </summary>
        /// <param name="date">The instance implementing the <see cref="IDATE"/> interface</param>
        /// <exception cref="System.ArgumentNullException">date</exception>
        public DATE(IDATE date)
        {
            fullyear = date.FULLYEAR;
            month = date.MONTH;
            mday = date.MDAY;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{fullyear:D4}{month:D2}{mday:D2}";  //YYYYMMDD
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DATE other)
        {
            return fullyear == other.FULLYEAR &&
                month == other.MONTH &&
                mday == other.MDAY;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
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
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)fullyear;
                hashCode = (hashCode * 397) ^ (int)month;
                hashCode = (hashCode * 397) ^ (int)mday;
                return hashCode;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(DATE other)
        {
            if (other == default(DATE)) return -2; //undefined
            if (fullyear < other.FULLYEAR) return -1;
            if (fullyear > other.FULLYEAR) return 1;
            if (month < other.MONTH) return -1;
            if (month > other.MONTH) return 1;
            if (mday < other.MDAY) return -1;
            return mday > other.MDAY ? 1 : 0;
        }

        #region overloaded operators

        /// <summary>
        /// Implements the equality operator ==.
        /// </summary>
        /// <param name="a">The instance to compare for equality</param>
        /// <param name="b">The other instance to compare against this instance for equality.</param>
        /// <returns>
        /// True if both instances are equal, otherwise false
        /// </returns>
        public static bool operator ==(DATE a, DATE b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the inequality operator !=.
        /// </summary>
        /// <param name="a">The instance to compare for inequality</param>
        /// <param name="b">The other instance to compare against this instance for inequality.</param>
        /// <returns>
        /// True if both instances are unequal, otherwise false
        /// </returns>
        public static bool operator !=(DATE a, DATE b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Implements the less-than operator &lt;.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>
        /// True if this instance is less than the other, otherwise false.
        /// </returns>
        public static bool operator <(DATE a, DATE b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Implements the less-than-or-equal-to operator &lt;=.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>
        /// True if this instance is less than or equal to the other, otherwise false.
        /// </returns>
        public static bool operator <=(DATE a, DATE b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Implements the greater-than operator &gt;.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>
        /// True if this instance is greater than the other, otherwise false.
        /// </returns>
        public static bool operator >(DATE a, DATE b)
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        /// Implements the greater-than-or-equal-to operator &gt;=.
        /// </summary>
        /// <param name="a">The instance to compare</param>
        /// <param name="b">The other instance to compare against this instance.</param>
        /// <returns>
        /// True if this instance is greater than the other, otherwise false.
        /// </returns>
        public static bool operator >=(DATE a, DATE b)
        {
            return a.CompareTo(b) >= 0;
        }

        /// <summary>
        /// Implements the substraction operator - between two <see cref="DATE"/> instances.
        /// </summary>
        /// <param name="start">The <see cref="DATE"/> instance to substract from.</param>
        /// <param name="end">The substracted <see cref="DATE"/> instance.</param>
        /// <returns>
        /// The result of the substraction.
        /// </returns>
        public static DURATION operator -(DATE start, DATE end)
        {
            return new DURATION(end.ToDateTime() - start.ToDateTime());
        }

        /// <summary>
        /// Implements the addition operator +.
        /// </summary>
        /// <param name="start">The <see cref="DATE"/> instance to add to</param>
        /// <param name="duration">The added <see cref="DURATION"/> instance.</param>
        /// <returns>
        /// A new <see cref="DATE"/> instance resulting from the addition.
        /// </returns>
        public static DATE operator +(DATE start, DURATION duration)
        {
            return (start.ToDateTime().Add(duration.ToTimeSpan())).ToDATE();
        }

        /// <summary>
        /// Implements the substraction operator - between a <see cref="DATE"/> instance and a <see cref="DURATION"/> instance.
        /// </summary>
        /// <param name="end">The <see cref="DATE"/> instance to substract from.</param>
        /// <param name="duration">The substracted <see cref="DURATION"/> instance.</param>
        /// <returns>
        /// A new <see cref="DATE"/> instance resulting from the substraction.
        /// </returns>
        public static DATE operator -(DATE end, DURATION duration)
        {
            return (end.ToDateTime().Subtract(duration.ToTimeSpan())).ToDATE();
        }

        #endregion overloaded operators

        public DATE AddDays(double value)
        {
            return new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddDays(value).ToDATE();
        }

        public DATE AddWeeks(int value)
        {
            return this.ToDateTime().AddWeeks(value).ToDATE();
        }

        public DATE AddMonths(int value)
        {
            return new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddMonths(value).ToDATE();
        }

        public DATE AddYears(int value)
        {
            return new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY).AddYears(value).ToDATE();
        }

        public WEEKDAY GetWeekday()
        {
            return this.ToDateTime().DayOfWeek.ToWEEKDAY();
        }
    }

    /// <summary>
    /// Represents a class for identifying values that specify a precise calendar date and time of the date
    /// Format: [YYYYMMSS]&quot;T&quot;[HHMMSS]&quot;Z&quot;
    /// where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    [DataContract]
    public struct DATE_TIME : IDATE, IDATE<DATE_TIME>, ITIME, ITIME<DATE_TIME>, IEquatable<DATE_TIME>, IComparable<DATE_TIME>, IiCalSerializable
    {
        private readonly uint hour, minute, second, fullyear, month, mday;
        private readonly TimeType type;
        private readonly TZID tzid;

        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013
        /// </summary>
        public uint FULLYEAR
        {
            get { return fullyear; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        public uint MONTH
        {
            get { return month; }
        }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        public uint MDAY
        {
            get { return mday; }
        }

        /// <summary>
        /// Gets the value of the hours
        /// </summary>
        public uint HOUR
        {
            get { return hour; }
        }

        /// <summary>
        /// Gets  the value of the minutes
        /// </summary>
        public uint MINUTE
        {
            get { return minute; }
        }

        /// <summary>
        /// Gets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public uint SECOND
        {
            get { return second; }
        }

        /// <summary>
        /// Gets the time format of the date-time type.
        /// </summary>
        public TimeType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the time zone identifier of the date-time type.
        /// </summary>
        public TZID TimeZoneId
        {
            get { return tzid; }
        }

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
            this.fullyear = fullyear;
            this.month = month;
            this.mday = mday;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.type = type;
            this.tzid = tzid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from a <see cref="datetime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="datetime"/> struct.</param>
        /// <param name="tzinfo">The time zone identifer (<see cref="TimeZoneInfo"/> ) of the <see cref="DATE_TIME"/> type.</param>
        public DATE_TIME(DateTime datetime, TimeZoneInfo tzinfo = null)
        {
            fullyear = (uint)datetime.Year;
            month = (uint)datetime.Month;
            mday = (uint)datetime.Day;
            hour = (uint)datetime.Hour;
            minute = (uint)datetime.Minute;
            second = (uint)datetime.Second;
            tzid = null;
            type = TimeType.NONE;
            if (tzinfo != null)
            {
                tzid = new TZID(null, tzinfo.Id);
                type = TimeType.LocalAndTimeZone;
            }
            else type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) type = TimeType.Utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> from a <see cref="datetime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="datetime"/> struct.</param>
        /// <param name="tzid">The time zone identifer (<see cref="TZID"/> ) of the <see cref="DATE_TIME"/> instance.</param>
        public DATE_TIME(DateTime datetime, TZID tzid)
        {
            fullyear = (uint)datetime.Year;
            month = (uint)datetime.Month;
            mday = (uint)datetime.Day;
            hour = (uint)datetime.Hour;
            minute = (uint)datetime.Minute;
            second = (uint)datetime.Second;
            this.tzid = null;
            type = TimeType.NONE;
            if (tzid != null)
            {
                this.tzid = tzid;
                type = TimeType.LocalAndTimeZone;
            }
            else type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) type = TimeType.Utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from a <see cref="DateTimeOffset"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTimeOffset"/> struct</param>
        public DATE_TIME(DateTimeOffset datetime)
        {
            fullyear = (uint)datetime.Year;
            month = (uint)datetime.Month;
            mday = (uint)datetime.Day;
            hour = (uint)datetime.Hour;
            minute = (uint)datetime.Minute;
            second = (uint)datetime.Second;
            type = TimeType.Utc;
            tzid = null;
        }

        /// <summary>
        /// Deserializes a new instance of the <see cref="DATE_TIME"/> struct from string.
        /// </summary>
        /// <param name="value">The string containing serialized ínformation about the <see cref="DATE_TIME"/> struct.</param>
        public DATE_TIME(string value)
        {
            fullyear = 0u;
            month = 0u;
            mday = 0u;
            hour = 0u;
            minute = 0u;
            second = 0u;
            tzid = null;
            type = TimeType.NONE;

            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})T(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})(?<utc>Z)?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from an implementation of the <see cref="IDATE_TIME"/> interface.
        /// </summary>
        /// <param name="value">The <see cref="IDATE_TIME"/> interface.</param>
        /// <param name="tzid"></param>
        public DATE_TIME(IDATE value)
        {
            fullyear = value.FULLYEAR;
            month = value.MONTH;
            mday = value.MDAY;
            hour = 0u;
            minute = 0u;
            second = 0u;
            tzid = null;
            type = TimeType.NONE;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct from an <see cref="IDATE"/> instance.
        /// </summary>
        /// <param name="value">The source <see cref="DATE_TIME"/> struct.</param>
        /// <param name="tzid">The time zone identifer (<see cref="TZID"/> ).</param>
        /// <exception cref="System.ArgumentNullException">date</exception>
        public DATE_TIME(ITIME value, TZID tzid = null)
        {
            fullyear = 0u;
            month = 0u;
            mday = 0u;
            hour = value.HOUR;
            minute = value.MINUTE;
            second = value.SECOND;
            type = tzid != null ? TimeType.LocalAndTimeZone : TimeType.NONE;
            this.tzid = tzid ?? value.TimeZoneId;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DATE_TIME other)
        {
            return hour == other.HOUR &&
                minute == other.MINUTE
                && second == other.SECOND &&
                fullyear == other.FULLYEAR &&
                month == other.MONTH &&
                mday == other.MDAY &&
                Equals(tzid, other.tzid);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
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
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)hour;
                hashCode = (hashCode * 397) ^ (int)minute;
                hashCode = (hashCode * 397) ^ (int)second;
                hashCode = (hashCode * 397) ^ (int)fullyear;
                hashCode = (hashCode * 397) ^ (int)month;
                hashCode = (hashCode * 397) ^ (int)mday;
                hashCode = (hashCode * 397) ^ (tzid != null ? tzid.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(DATE_TIME other)
        {
            if (fullyear < other.FULLYEAR) return -1;
            if (fullyear > other.FULLYEAR) return 1;
            if (month < other.MONTH) return -1;
            if (month > other.MONTH) return 1;
            if (mday < other.MDAY) return -1;
            if (mday > other.MDAY) return 1;
            if (hour < other.HOUR) return -1;
            if (hour > other.HOUR) return 1;
            if (minute < other.MINUTE) return -1;
            if (minute > other.MINUTE) return 1;
            if (second < other.SECOND) return -1;
            if (second > other.SECOND) return 1;
            return 0;
        }

        #region overloaded operators

        /// <summary>
        /// Adds a specified time duration to a specified date and time, yielding a new <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="start">The <see cref="DATE_TIME"/> value to add to.</param>
        /// <param name="duration">The time duration to add.</param>
        /// <returns>
        /// The sum of the <see cref="start"/> and <see cref="duration"/>.
        /// </returns>
        public static DATE_TIME operator +(DATE_TIME start, DURATION duration)
        {
            return (start.ToDateTime().Add(duration.ToTimeSpan())).ToDATE_TIME(start.TimeZoneId);
        }

        /// <summary>
        /// Substracts a specified time duration from a specified date and time, yielding a new <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="end">The <see cref="DATE_TIME"/> value to substract from.</param>
        /// <param name="duration">The time duration to substract.</param>
        /// <returns>
        /// The value of the <see cref="DATE_TIME"/> object minus the <see cref="duration"/>.
        /// </returns>
        public static DATE_TIME operator -(DATE_TIME end, DURATION duration)
        {
            return (end.ToDateTime().Subtract(duration.ToTimeSpan())).ToDATE_TIME(end.TimeZoneId);
        }

        public static DURATION operator -(DATE_TIME end, DATE_TIME start)
        {
            if (end > start) return new DURATION(end.ToDateTime() - start.ToDateTime());
            if (end < start) return new DURATION(start.ToDateTime() - end.ToDateTime());
            return new DURATION();
        }

        public static bool operator <(DATE_TIME a, DATE_TIME b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(DATE_TIME a, DATE_TIME b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(DATE_TIME a, DATE_TIME b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(DATE_TIME a, DATE_TIME b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(DATE_TIME left, DATE_TIME right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DATE_TIME left, DATE_TIME right)
        {
            return !left.Equals(right);
        }

        #endregion overloaded operators

        public DATE_TIME AddSeconds(double value)
        {
            return this.ToDateTime().AddSeconds(value).ToDATE_TIME(tzid);
        }

        public DATE_TIME AddMinutes(double value)
        {
            return this.ToDateTime()
                .AddMinutes(value)
                .ToDATE_TIME(tzid);
        }

        public DATE_TIME AddHours(double value)
        {
            return this.ToDateTime()
                .AddHours(value)
                .ToDATE_TIME(tzid);
        }

        public DATE_TIME AddDays(double value)
        {
            return this.ToDateTime().AddDays(value).ToDATE_TIME(TimeZoneId);
        }

        public DATE_TIME AddWeeks(int value)
        {
            return this.ToDateTime().AddWeeks(value).ToDATE_TIME(TimeZoneId);
        }

        public DATE_TIME AddMonths(int value)
        {
            return this.ToDateTime().AddMonths(value).ToDATE_TIME(TimeZoneId);
        }

        public DATE_TIME AddYears(int value)
        {
            return this.ToDateTime().AddYears(value).ToDATE_TIME(TimeZoneId);
        }

        public WEEKDAY GetWeekday()
        {
            return this.ToDateTime().DayOfWeek.ToWEEKDAY();
        }

        public void WriteCalendar(iCalWriter writer)
        {
            switch (type)
            {
                case TimeType.Local:
                    writer.WriteValue($"{fullyear:D4}{month:D2}{mday:D2}T{hour:D2}{minute:D2}{second:D2}");
                    break;
                case TimeType.Utc:
                    writer.WriteValue($"{fullyear:D4}{month:D2}{mday:D2}T{hour:D2}{minute:D2}{second:D2}Z");
                    break;
                case TimeType.LocalAndTimeZone:
                    writer.WriteValue($"{tzid}:{fullyear:D4}{month:D2}{mday:D2}T{hour:D2}{minute:D2}{second:D2}");
                    break;
                default:
                    writer.WriteValue($"{fullyear:D4}{month:D2}{mday:D2}T{hour:D2}{minute:D2}{second:D2}");
                    break;
            }
        }

        public void ReadCalendar(iCalReader reader)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public struct TIME : ITIME, ITIME<TIME>, IEquatable<TIME>, IComparable<TIME>
    {
        private readonly uint hour, minute, second;
        private readonly TimeType type;
        private readonly TZID tzid;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        public uint HOUR
        {
            get { return hour; }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;59 </exception>
        public uint MINUTE
        {
            get { return minute; }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public uint SECOND
        {
            get { return second; }
        }

        public TimeType Type
        {
            get { return type; }
        }

        public TZID TimeZoneId
        {
            get { return tzid; }
        }

        public TIME(uint hour, uint minute, uint second, TimeType type = TimeType.Local, TZID tzid = null)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.type = type;
            this.tzid = tzid;
        }

        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            hour = (uint)datetime.Hour;
            minute = (uint)datetime.Minute;
            second = (uint)datetime.Second;
            tzid = null;
            type = TimeType.NONE;
            if (tzinfo != null)
            {
                tzid = new TZID(null, tzinfo.Id);
                type = TimeType.LocalAndTimeZone;
            }
            else type = TimeType.Local;
            if (datetime.Kind == DateTimeKind.Utc) type = TimeType.Utc;
        }

        public TIME(DateTimeOffset datetime)
        {
            hour = (uint)datetime.Hour;
            minute = (uint)datetime.Minute;
            second = (uint)datetime.Second;
            type = TimeType.Utc;
            tzid = null;
        }

        public TIME(string value)
        {
            hour = 0u;
            minute = 0u;
            second = 0u;
            tzid = null;
            type = TimeType.NONE;

            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z)?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
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
        }

        public TIME(DATE_TIME datetime)
        {
            hour = datetime.HOUR;
            minute = datetime.MINUTE;
            second = datetime.SECOND;
            type = datetime.Type;
            tzid = datetime.TimeZoneId;
        }

        public TIME(ITIME time, TZID tzid = null)
        {
            if (time == null) throw new ArgumentNullException("time");
            hour = time.HOUR;
            minute = time.MINUTE;
            second = time.SECOND;
            type = time.Type;
            this.tzid = tzid == null ? time.TimeZoneId : tzid;
        }

        public override string ToString()
        {
            if (type == TimeType.Local) return $"T{hour:D2}{minute:D2}{second:D2}";
            else if (type == TimeType.Utc)
                return $"T{hour:D2}{minute:D2}{second:D2}Z";
            else if (Type == TimeType.LocalAndTimeZone)
                return string.Format("{0}:T{0:D2}{1:D2}{2:D2}", tzid, hour, minute, second);
            else
                return $"T{hour:D2}{minute:D2}{second:D2}";
        }

        public bool Equals(TIME other)
        {
            return hour == other.HOUR &&
                minute == other.MINUTE &&
                second == other.SECOND &&
                type == other.Type &&
                Equals(tzid, other.tzid);
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
                var hashCode = (int)hour;
                hashCode = (hashCode * 397) ^ (int)minute;
                hashCode = (hashCode * 397) ^ (int)second;
                hashCode = (hashCode * 397) ^ (int)type;
                hashCode = (hashCode * 397) ^ (tzid != null ? tzid.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int CompareTo(TIME other)
        {
            if (hour > other.hour) return 1;
            if (minute < other.minute) return -1;
            if (minute > other.minute) return 1;
            if (second < other.second) return -1;
            if (second > other.second) return 1;
            return 0;
        }

        #region overloaded operators

        public static TIME operator +(TIME start, DURATION duration)
        {
            return (start.ToTimeSpan().Add(duration.ToTimeSpan())).ToTIME(start.TimeZoneId);
        }

        public static TIME operator -(TIME end, DURATION duration)
        {
            return (end.ToTimeSpan().Subtract(duration.ToTimeSpan())).ToTIME(end.TimeZoneId);
        }

        public static DURATION operator -(TIME start, TIME end)
        {
            return new DURATION(end.ToTimeSpan() - start.ToTimeSpan());
        }

        public static bool operator <(TIME a, TIME b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(TIME a, TIME b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(TIME a, TIME b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(TIME a, TIME b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(TIME a, TIME b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TIME a, TIME b)
        {
            return !a.Equals(b);
        }

        #endregion overloaded operators

        public TIME AddSeconds(double value)
        {
            return this.ToDateTime().AddSeconds(value).ToTIME(tzid);
        }

        public TIME AddMinutes(double value)
        {
            return this.ToDateTime().AddMinutes(value).ToTIME(tzid);
        }

        public TIME AddHours(double value)
        {
            return this.ToDateTime().AddHours(value).ToTIME(tzid);
        }
    }

    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public struct DURATION : IDURATION, IEquatable<DURATION>, IComparable<DURATION>
    {
        private readonly int weeks, days, hours, minutes, seconds;

        /// <summary>
        /// Gets the duration in weeks
        /// </summary>
        public int WEEKS
        {
            get { return weeks; }
        }

        /// <summary>
        /// Gets the duration in hours
        /// </summary>
        public int HOURS
        {
            get { return hours; }
        }

        /// <summary>
        /// Gets the duration in minutes
        /// </summary>
        public int MINUTES
        {
            get { return minutes; }
        }

        /// <summary>
        /// Gets the duration in seconds
        /// </summary>
        public int SECONDS
        {
            get { return seconds; }
        }

        /// <summary>
        /// Gets the duration in days
        /// </summary>
        public int DAYS
        {
            get { return days; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="duration"></param>
        public DURATION(DURATION duration)
        {
            weeks = duration.WEEKS;
            days = duration.DAYS;
            hours = duration.HOURS;
            minutes = duration.MINUTES;
            seconds = duration.SECONDS;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="weeks"></param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public DURATION(int weeks, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            this.weeks = weeks;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="span"></param>
        public DURATION(TimeSpan span)
        {
            days = span.Days;
            hours = span.Hours;
            minutes = span.Minutes;
            seconds = span.Seconds;
            weeks = span.Days
                + (span.Hours / 24)
                + (span.Minutes / (24 * 60))
                + (span.Seconds / (24 * 3600))
                + (span.Milliseconds / (24 * 3600000)) / 7;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public DURATION(string value)
        {
            weeks = days = hours = minutes = seconds = 0;
            var pattern = @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
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
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="duration"></param>
        public DURATION(IDURATION duration)
        {
            if (duration == null) throw new ArgumentNullException("duration");
            weeks = duration.WEEKS;
            days = duration.DAYS;
            hours = duration.HOURS;
            minutes = duration.MINUTES;
            seconds = duration.SECONDS;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var sign = (weeks < 0 || days < 0 || hours < 0 || minutes < 0 || seconds < 0) ? "-" : string.Empty;
            sb.AppendFormat("{0}P", sign);
            if (weeks != 0) sb.AppendFormat("{0}W", weeks);
            if (days != 0) sb.AppendFormat("{0}D", days);
            if (hours != 0 || minutes != 0 || seconds != 0) sb.Append("T");
            if (hours != 0) sb.AppendFormat("{0}H", hours);
            if (minutes != 0) sb.AppendFormat("{0}M", minutes);
            if (seconds != 0) sb.AppendFormat("{0}S", seconds);
            return sb.ToString();
        }

        public bool Equals(DURATION other)
        {
            return weeks == other.WEEKS &&
                days == other.DAYS &&
                hours == other.HOURS &&
                minutes == other.MINUTES &&
                seconds == other.SECONDS;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DURATION && Equals((DURATION)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = weeks;
                hashCode = (hashCode * 397) ^ days;
                hashCode = (hashCode * 397) ^ hours;
                hashCode = (hashCode * 397) ^ minutes;
                hashCode = (hashCode * 397) ^ seconds;
                return hashCode;
            }
        }

        public int CompareTo(DURATION other)
        {
            return this.ToTimeSpan().CompareTo(other.ToTimeSpan());
        }

        #region overloaded operators

        public static DURATION operator -(DURATION duration)
        {
            return new DURATION(-duration.WEEKS, -duration.DAYS, -duration.HOURS, -duration.MINUTES, -duration.SECONDS);
        }

        public static DURATION operator +(DURATION duration)
        {
            return new DURATION(duration.WEEKS, duration.DAYS, duration.HOURS, duration.MINUTES, duration.SECONDS);
        }

        public static DURATION operator +(DURATION a, DURATION b)
        {
            return new DURATION(a.WEEKS + b.WEEKS, a.DAYS + b.DAYS, a.HOURS + b.HOURS, a.MINUTES + b.MINUTES, a.SECONDS + b.SECONDS);
        }

        public static DURATION operator -(DURATION a, DURATION b)
        {
            return new DURATION(a.WEEKS - b.WEEKS, a.DAYS - b.DAYS, a.HOURS - b.HOURS, a.MINUTES - b.MINUTES, a.SECONDS - b.SECONDS);
        }

        public static DURATION operator *(DURATION duration, int scalar)
        {
            return new DURATION(duration.WEEKS * scalar, duration.DAYS * scalar, duration.HOURS * scalar, duration.MINUTES * scalar, duration.SECONDS * scalar);
        }

        public static DURATION operator /(DURATION duration, int scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Zero dividend is forbidden!");
            return new DURATION(duration.WEEKS / scalar, duration.DAYS / scalar, duration.HOURS / scalar, duration.MINUTES / scalar, duration.SECONDS / scalar);
        }

        public static bool operator <(DURATION a, DURATION b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(DURATION a, DURATION b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(DURATION a, DURATION b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(DURATION a, DURATION b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(DURATION a, DURATION b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DURATION a, DURATION b)
        {
            return !a.Equals(b);
        }

        #endregion overloaded operators
    }

    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public struct WEEKDAYNUM : IWEEKDAYNUM, IEquatable<WEEKDAYNUM>, IComparable<WEEKDAYNUM>
    {
        private readonly int ordweek;
        private readonly WEEKDAY weekday;

        /// <summary>
        /// Gets or sets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        public int NthOccurrence
        {
            get { return ordweek; }
        }

        /// <summary>
        /// Gets or sets the weekday
        /// </summary>
        public WEEKDAY Weekday
        {
            get { return weekday; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="weekday"></param>
        public WEEKDAYNUM(WEEKDAY weekday)
        {
            ordweek = 0;
            this.weekday = weekday;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ordweek"></param>
        /// <param name="weekday"></param>
        public WEEKDAYNUM(int ordweek, WEEKDAY weekday)
        {
            this.ordweek = ordweek;
            this.weekday = weekday;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public WEEKDAYNUM(string value)
        {
            ordweek = 0;
            weekday = WEEKDAY.NONE;

            var pattern = @"^((?<minus>\-)? <?ordwk>\d{1,2})?(?<weekday>(SU|MO|TU|WE|TH|FR|SA)$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                var mulitplier = 1;
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
                {
                    if (match.Groups["minus"].Success)
                    {
                        if (match.Groups["minus"].Value == "-") mulitplier *= -1;
                    }
                    if (match.Groups["ordwk"].Success) ordweek = mulitplier * int.Parse(match.Groups["ordwk"].Value);
                    if (match.Groups["weekday"].Success) weekday = (WEEKDAY)Enum.Parse(typeof(WEEKDAY), match.Groups["weekday"].Value);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="weekdaynum"></param>
        public WEEKDAYNUM(IWEEKDAYNUM weekdaynum)
        {
            if (weekdaynum == null) throw new ArgumentNullException("weekdaynum");
            ordweek = weekdaynum.NthOccurrence;
            weekday = weekdaynum.Weekday;
        }

        public bool Equals(WEEKDAYNUM other)
        {
            return NthOccurrence == other.NthOccurrence && Weekday == other.Weekday;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals((WEEKDAYNUM)obj);
        }

        public override int GetHashCode()
        {
            return NthOccurrence.GetHashCode() ^ Weekday.GetHashCode();
        }

        public override string ToString()
        {
            if (NthOccurrence != 0)
            {
                return (NthOccurrence < 0) ?
                    $"-{(uint) NthOccurrence} {Weekday}"
                    : $"+{(uint) NthOccurrence} {Weekday}";
            }
            return $"{Weekday}";
        }

        public int CompareTo(WEEKDAYNUM other)
        {
            if (ordweek == 0 && other.NthOccurrence == 0) return weekday.CompareTo(other.Weekday);
            if (ordweek < other.NthOccurrence) return -1;
            if (ordweek > other.NthOccurrence) return 1;
            return weekday.CompareTo(other.Weekday);
        }

        public static bool operator ==(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return !a.Equals(b);
        }

        public static bool operator <(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(WEEKDAYNUM a, WEEKDAYNUM b)
        {
            return a.CompareTo(b) >= 0;
        }
    }

    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public struct UTC_OFFSET : IUTC_OFFSET, IEquatable<UTC_OFFSET>, IComparable<UTC_OFFSET>
    {
        private readonly int hour, minute, second;

        /// <summary>
        /// Gets or sets the value of the hours
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;23 </exception>
        public int HOUR
        {
            get { return hour; }
        }

        /// <summary>
        /// Gets or sets the value of the minutes
        /// </summary>
        public int MINUTE
        {
            get { return minute; }
        }

        /// <summary>
        /// Gets or sets the value of the seconds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the value &lt;0 and value &gt;60 </exception>
        public int SECOND
        {
            get { return second; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct from a serialized string.
        /// </summary>
        /// <param name="value">A serialized string representing a <see cref="UTC_OFFSET"/> struct.</param>
        public UTC_OFFSET(string value)
        {
            hour = minute = 0;
            second = 1;

            var pattern = @"^(?<minus>\-|?<plus>\+)(?<hours>\d{1,2})(?<mins>\d{1,2})(?<secs>\d{1,2})?$";
            if (Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
            {
                foreach (Match match in Regex.Matches(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture))
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct.
        /// </summary>
        /// <param name="hour">The hour to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        /// <param name="minute">The minute to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        /// <param name="second">The second to initialize the <see cref="UTC_OFFSET"/> struct.</param>
        public UTC_OFFSET(int hour, int minute, int second)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UTC_OFFSET"/> struct from a <see cref="IUTC_OFFSET"/> instance.
        /// </summary>
        /// <param name="offset"></param>
        public UTC_OFFSET(IUTC_OFFSET offset)
        {
            hour = offset.HOUR;
            minute = offset.MINUTE;
            second = offset.SECOND;
        }

        public override string ToString()
        {
            if (hour < 0 || minute < 0 || second < 0)
                return $"-{hour:D2}{minute:D2}{second:D2}";
            else
                return $"+{hour:D2}{minute:D2}{second:D2}";
        }

        public bool Equals(UTC_OFFSET other)
        {
            return hour == other.hour &&
                minute == other.minute &&
                second == other.second;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UTC_OFFSET && Equals((UTC_OFFSET)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = hour;
                hashCode = (hashCode * 397) ^ minute;
                hashCode = (hashCode * 397) ^ second;
                return hashCode;
            }
        }

        public int CompareTo(UTC_OFFSET other)
        {
            if (hour < other.HOUR) return -1;
            if (hour > other.HOUR) return 1;
            if (minute < other.MINUTE) return -1;
            if (minute > other.MINUTE) return 1;
            if (second < other.SECOND) return -1;
            if (second > other.SECOND) return 1;
            return 0;
        }

        #region overloaded operators

        public static UTC_OFFSET operator +(UTC_OFFSET a, UTC_OFFSET b)
        {
            return new UTC_OFFSET((a.HOUR + b.HOUR) % 24, (a.MINUTE + b.MINUTE) % 60, (a.SECOND + b.SECOND) % 60);
        }

        public static UTC_OFFSET operator -(UTC_OFFSET a, UTC_OFFSET b)
        {
            return new UTC_OFFSET((a.HOUR - b.HOUR).Modulo(60), (a.MINUTE - b.MINUTE).Modulo(60), (a.SECOND - b.SECOND).Modulo(60));
        }

        public static bool operator <(UTC_OFFSET a, UTC_OFFSET b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(UTC_OFFSET a, UTC_OFFSET b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(UTC_OFFSET a, UTC_OFFSET b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(UTC_OFFSET a, UTC_OFFSET b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(UTC_OFFSET left, UTC_OFFSET right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UTC_OFFSET left, UTC_OFFSET right)
        {
            return !left.Equals(right);
        }

        #endregion overloaded operators
    }

    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public struct PERIOD : IPERIOD, IEquatable<PERIOD>, IComparable<PERIOD>
    {
        private readonly DATE_TIME start, end;
        private readonly DURATION duration;
        private readonly PeriodType type;

        /// <summary>
        /// Gets or sets the start of the period
        /// </summary>
        public DATE_TIME Start
        {
            get { return start; }
        }

        /// <summary>
        /// Gets or sets the end of the period.
        /// </summary>
        public DATE_TIME End
        {
            get { return end; }
        }

        /// <summary>
        /// Gets or sets the duration of the period.
        /// </summary>
        public DURATION Duration
        {
            get { return duration; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public PERIOD(string value)
        {
            start = default(DATE_TIME);
            end = default(DATE_TIME);
            duration = default(DURATION);
            type = PeriodType.Start;

            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");

            var datetimePattern = @"((TZID=(\w+)?/(\w+)):)?(\d{4,})(\d{2})(\d{2})T(\d{2})(\d{2})(\d{2})Z?";
            var durationPattern = @"(\-)?P((\d*)W)?((\d*)D)?(T((\d*)H)?((\d*)M)?((\d*)S)?)?";
            var explicitPattern = string.Format("{0}/{0}", datetimePattern);
            var startPattern = $"{datetimePattern}/{durationPattern}";

            var pattern = $@"^(?<periodExplicit>{explicitPattern})|(?<periodStart>{startPattern})$";

            var options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.CultureInvariant;
            var regex = new Regex(pattern, options);

            if (!Regex.IsMatch(value, pattern, options)) throw new FormatException("value");
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["periodExplicit"].Success)
                {
                    var periodExplicit = match.Groups["periodExplicit"].Value;
                    var parts = periodExplicit.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    start = new DATE_TIME(parts[0]);
                    end = new DATE_TIME(parts[1]);
                    duration = end - start;
                    type = PeriodType.Explicit;
                    break;
                }
                if (match.Groups["periodStart"].Success)
                {
                    var periodStart = match.Groups["periodStart"].Value;
                    var parts = periodStart.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    start = new DATE_TIME(parts[0]);
                    duration = new DURATION(parts[1]);
                    end = start + duration;
                    type = PeriodType.Start;
                    break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public PERIOD(DATE_TIME start, DATE_TIME end)
        {
            this.start = start;
            this.end = end;
            duration = end - start;
            type = PeriodType.Explicit;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        public PERIOD(DATE_TIME start, DURATION duration)
        {
            this.start = start;
            this.duration = duration;
            end = start + duration;
            type = PeriodType.Start;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startTimeZoneInfo"></param>
        /// <param name="endTimeZoneInfo"></param>
        public PERIOD(DateTime start, DateTime end, TimeZoneInfo startTimeZoneInfo = null, TimeZoneInfo endTimeZoneInfo = null)
        {
            this.start = new DATE_TIME(start, startTimeZoneInfo);
            this.end = new DATE_TIME(end, endTimeZoneInfo);
            duration = this.end - this.start;
            type = PeriodType.Explicit;
        }

        public PERIOD(DateTime start, TimeSpan span, TimeZoneInfo timeZoneInfo = null)
        {
            this.start = new DATE_TIME(start, timeZoneInfo);
            duration = new DURATION(span);
            end = this.start + duration;
            type = PeriodType.Start;
        }

        public PERIOD(IPERIOD period)
        {
            if (period == null) throw new ArgumentNullException("period");
            start = period.Start;
            end = period.End;
            duration = period.Duration;
            type = PeriodType.Explicit;
        }

        public override string ToString()
        {
            return (type == PeriodType.Start) ?
                $"{start}/{end}"
                : $"{start}/{duration}";
        }

        public bool Equals(PERIOD other)
        {
            return start.Equals(other.Start) &&
                end.Equals(other.End) &&
                duration.Equals(other.Duration) &&
                type == other.type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PERIOD && Equals((PERIOD)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = start.GetHashCode();
                hashCode = (hashCode * 397) ^ end.GetHashCode();
                hashCode = (hashCode * 397) ^ duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)type;
                return hashCode;
            }
        }

        public int CompareTo(PERIOD other)
        {
            return (type == PeriodType.Explicit) ?
                start.CompareTo(other.start) + end.CompareTo(other.end) :
                start.CompareTo(other.start) + duration.CompareTo(other.duration);
        }

        #region overloaded operators

        public static bool operator <(PERIOD a, PERIOD b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(PERIOD a, PERIOD b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(PERIOD a, PERIOD b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(PERIOD a, PERIOD b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(PERIOD a, PERIOD b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PERIOD a, PERIOD b)
        {
            return !a.Equals(b);
        }

        #endregion overloaded operators
    }

    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public class RECUR : IRECUR, IEquatable<RECUR>, IContainsKey<Guid>
    {
        #region properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the recurrence rule.
        /// This property is REQUIRED and must not occur more than once.
        /// </summary>
        [DataMember]
        public FREQ FREQ { get; set; }

        /// <summary>
        /// Gets or sets the end date that bounds the reczrrence rule in an INCLUSIVE manner.
        /// This property is OPTIONAL but MUST be nullified if the COUNT property is non-zero.
        /// </summary>
        [DataMember]
        public DATE_TIME UNTIL { get; set; }

        /// <summary>
        /// Gets or sets the number of occurrences at which to range-bound the recurrence.
        /// This property is OPTIONAL but MUST be set to zero if the UNTIL property is non-null.
        /// </summary>
        [DataMember]
        public uint COUNT { get; set; }

        /// <summary>
        /// Represents the intervals the recurrence rule repeats.
        /// The default value is 1.
        /// For example in a SECONDLY rule, this property implies a repetition for every second.
        /// </summary>
        [DataMember]
        public uint INTERVAL { get; set; }

        /// <summary>
        /// Gets or sets the collection of seconds within a minute.
        /// Valid values are 0 to 60.
        /// </summary>
        /// <remarks>Normally, the seconds range is from 0 to 59 but the extra 60th second accounts for a leap second, which ma ybe ignored by a non-supporting system. </remarks>
        [DataMember]
        public List<uint> BYSECOND { get; set; }

        /// <summary>
        /// Gets or sets the collection of minutes within an hour.
        /// Valid values are from 0 to 59.
        /// </summary>
        [DataMember]
        public List<uint> BYMINUTE { get; set; }

        /// <summary>
        /// Gets or sets the collection of hours within a day.
        /// Valid values are from 0 to 23.
        /// </summary>
        [DataMember]
        public List<uint> BYHOUR { get; set; }

        [DataMember]
        public List<WEEKDAYNUM> BYDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the month.
        /// Valid values are 1 to 31 or -31 to -1.
        /// Negative values represent the day position from the end of the month e.g -10 is the tenth to the last day of the month
        /// </summary>
        [DataMember]
        public List<int> BYMONTHDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the year.
        /// Valid values are 1 to 366 or -366 to -1.
        /// Negative values represent the day position from the end of the year e.g -306 is the 306th to the last day of the month.
        /// This property MUST NOT be specified when the FREQ property is set to DAILY, WEEKLY, or MONTHLY
        /// </summary>
        [DataMember]
        public List<int> BYYEARDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of ordinals specifiying the weeks of the year.
        /// Valid values are 1 to 53 or -53 to -1.
        /// This property MUST NOT be specified when the FREQ property is set to anything other than YEARLY.
        /// </summary>
        [DataMember]
        public List<int> BYWEEKNO { get; set; }

        /// <summary>
        /// Gets or sets the collection of months of the year.
        /// Valid values are 1 to 12.
        /// </summary>
        [DataMember]
        public List<uint> BYMONTH { get; set; }

        /// <summary>
        /// Gets or sets the day on which the work week starts.
        /// The default day is MO.
        /// </summary>
        /// <remarks>
        /// This is used significantly when a weekly recurrence rule has an interval greater than 1 and a BYDAY rule is specified.
        /// Also, it is significantly used when a BYWEEKNO rule in a YEARLY rule is specified.
        /// </remarks>
        [DataMember]
        public WEEKDAY WKST { get; set; }

        /// <summary>
        /// Gets or sets the list of values corresponfing to the nth occurence within the set of recurrence instances in an interval of the recurrence rule.
        /// Valid values are 1 to 366 or -366 to -1
        /// This property MUST only be used together with another BYxxx rule part (e.g. BYDAY)
        /// </summary>
        /// <remarks>
        /// Example: &quot; the last work day of the month &quot;
        /// FRREQ = MONTHLY; BYDAY = MO, TU, WE, TH, FR; BYSETPOS = -1
        /// </remarks>
        [DataMember]
        public List<int> BYSETPOS { get; set; }

        #endregion properties

        /// <summary>
        ///
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
        ///
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
                const string pattern = @"^(FREQ|UNTIL|COUNT|INTERVAL|BYSECOND|BYMINUTE|BYHOUR|BYMONTHDAY|BYYEARDAY|BYWEEKNO|BYMONTH|WKST|BYSETPOS)=((\w+|\d+)(,[\s]*(\w+|\d+))*)$";
                if (!Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)) continue;

                var pair = token.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                //check FREQ
                if (pair[0].Equals("FREQ", StringComparison.OrdinalIgnoreCase)) FREQ = (FREQ)Enum.Parse(typeof(FREQ), pair[1], true);
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

                if (pair[0].Equals("WKST", StringComparison.OrdinalIgnoreCase)) WKST = (WEEKDAY)Enum.Parse(typeof(WEEKDAY), pair[1], true);

                if (pair[0].Equals("BYSETPOS", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = pair[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;
                    BYSETPOS = parts.Select(int.Parse).ToList();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="until"></param>
        public RECUR(FREQ freq, DATE_TIME until) : this()
        {
            FREQ = freq;
            UNTIL = until;
        }

        /// <summary>
        ///
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
        ///
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("FREQ={0};", FREQ);

            if (UNTIL != default(DATE_TIME)) sb.AppendFormat("UNTIL={0};", UNTIL);
            else if (COUNT != 0) sb.AppendFormat("COUNT={0};", COUNT);

            sb.AppendFormat("INTERVAL={0};", INTERVAL);
            if (!BYSECOND.NullOrEmpty())
            {
                sb.AppendFormat("BYSECOND=");
                foreach (var val in BYSECOND)
                {
                    sb.AppendFormat(val != BYSECOND.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYMINUTE.NullOrEmpty())
            {
                sb.AppendFormat("BYMINUTE=");
                foreach (var val in BYMINUTE)
                {
                    sb.AppendFormat(val != BYMINUTE.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYHOUR.NullOrEmpty())
            {
                sb.AppendFormat("BYHOUR=");
                foreach (var val in BYHOUR)
                {
                    sb.AppendFormat(val != BYHOUR.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYDAY.NullOrEmpty())
            {
                sb.AppendFormat("BYDAY=");
                foreach (var val in BYDAY)
                {
                    sb.AppendFormat(val != BYDAY.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYMONTHDAY.NullOrEmpty())
            {
                sb.AppendFormat("BYMONTHDAY=");
                foreach (var val in BYMONTHDAY)
                {
                    sb.AppendFormat(val != BYMONTHDAY.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYYEARDAY.NullOrEmpty())
            {
                sb.AppendFormat("BYYEARDAY=");
                foreach (var val in BYYEARDAY)
                {
                    sb.AppendFormat(val != BYYEARDAY.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYWEEKNO.NullOrEmpty())
            {
                sb.AppendFormat("BYWEEKNO=");
                foreach (var val in BYWEEKNO)
                {
                    sb.AppendFormat(val != BYWEEKNO.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYMONTH.NullOrEmpty())
            {
                sb.AppendFormat("BYMONTH=");
                foreach (var val in BYMONTH)
                {
                    sb.AppendFormat(val != BYMONTH.Last() ? "{0}," : "{0};", val);
                }
            }

            if (!BYSETPOS.NullOrEmpty())
            {
                sb.AppendFormat("BYSETPOS=");
                foreach (var m in BYSETPOS)
                {
                    if (m != BYSETPOS.Last())
                    {
                        if (m < 0) sb.AppendFormat("{0},", m);
                        else if (m > 0) sb.AppendFormat("+{0},", m);
                        else sb.AppendFormat("{0},", m);
                    }
                    else
                    {
                        if (m < 0) sb.AppendFormat("{0};", m);
                        else if (m > 0) sb.AppendFormat("+{0};", m);
                        else sb.AppendFormat("{0};", m);
                    }
                }
            }

            sb.AppendFormat("WKST={0}", WKST);
            return sb.ToString();
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

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RECUR left, RECUR right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RECUR left, RECUR right)
        {
            return !Equals(left, right);
        }
    }

    [DataContract]
    public class CAL_ADDRESS : Uri
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
            : base(emailable ? new Uri($"mailto:{baseUri.ToString().Replace("mailto:", string.Empty)}") : baseUri, relativeUri)
        {
        }

        public CAL_ADDRESS(Uri baseUri, Uri relativeUri, bool emailable = true) :
            base(emailable ? new Uri($"mailto:{baseUri.ToString().Replace("mailto:", string.Empty)}") : baseUri, relativeUri)
        {
        }

        public CAL_ADDRESS(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}