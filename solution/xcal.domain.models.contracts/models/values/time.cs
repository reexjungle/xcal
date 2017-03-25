using reexjungle.xcal.core.domain.contracts.extensions;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a point in time, typically expressed as time of day.
    /// <para/>
    /// Format 1 (Local Time): [HHMMSS]
    /// <para/>
    /// Format 2 (UTC Time): [HHMMSS]Z
    /// <para/>
    /// where HH is 2-digit hour, MM is 2-digit minute, SS is 2-digit second and Z is UTC zone indicator.
    /// </summary>
    [DataContract]
    public struct TIME : IEquatable<TIME>, IComparable, IComparable<TIME>
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        [DataMember]
        public int HOUR { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a minute.
        /// </summary>
        [DataMember]
        public int MINUTE { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second.
        /// </summary>
        [DataMember]
        public int SECOND { get; private set; }

        /// <summary>
        /// Gets the form in which the <see cref="TIME"/> instance is expressed.
        /// </summary>
        [DataMember]
        public TIME_FORM Form { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified hour,
        /// minute, second, time form and optionally the time zone identifier.
        /// </summary>
        /// <param name="hour">The digit representation of an hour.</param>
        /// <param name="minute">The digit representation of a minute.</param>
        /// <param name="second">The digit representation of a second.</param>
        /// <param name="form">The form in which the <see cref="TIME"/> instance is expressed.</param>
        /// <param name="tzid">The identifier that references the time zone.</param>
        public TIME(int hour, int minute, int second, TIME_FORM form = TIME_FORM.LOCAL)
        {
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified <see
        /// cref="DateTime"/> and <see cref="TimeZoneInfo"/>.
        /// </summary>
        /// <param name="datetime">A point in time, typically expressed as date and time of day.</param>
        public TIME(DateTime datetime)
        {
            HOUR = datetime.Hour;
            MINUTE = datetime.Minute;
            SECOND = datetime.Second;
            Form = datetime.Kind.AsTIME_FORM();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified <see cref="DateTimeOffset"/>.
        /// <para/>
        /// Note: By using this constructor, the new instance of <see cref="TIME"/> shall always be
        ///       initialized as UTC time.
        /// </summary>
        /// <param name="datetime">
        /// A point in time, typically expressed as date and time of day, relative to Coordinate
        /// Universal Time (UTC).
        /// </param>
        public TIME(DateTimeOffset datetime)
        {
            HOUR = datetime.UtcDateTime.Hour;
            MINUTE = datetime.UtcDateTime.Minute;
            SECOND = datetime.UtcDateTime.Second;
            Form = TIME_FORM.UTC;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with its equivalent string representation.
        /// </summary>
        /// <param name="value">The string representation of the <see cref="TIME"/> instance.</param>
        public TIME(string value)
        {
            var hour = 0;
            var minute = 0;
            var second = 0;
            var form = TIME_FORM.LOCAL;

            const string pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z)?$";
            const RegexOptions options = RegexOptions.IgnoreCase
                                         | RegexOptions.CultureInvariant
                                         | RegexOptions.ExplicitCapture
                                         | RegexOptions.Compiled;

            var regex = new Regex(pattern, options);

            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["hour"].Success) hour = int.Parse(match.Groups["hour"].Value);
                if (match.Groups["min"].Success) minute = int.Parse(match.Groups["min"].Value);
                if (match.Groups["sec"].Success) second = int.Parse(match.Groups["sec"].Value);
                if (match.Groups["utc"].Success) form = TIME_FORM.UTC;
            }
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
        }

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this time instance.</returns>
        public DateTime AsDateTime() => this == default(TIME)
            ? default(DateTime)
            : new DateTime(1, 1, 1, HOUR, MINUTE, SECOND, Form.AsDateTimeKind());

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="System.DateTimeOffset"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="System.DateTimeOffset"/> respresentation of this date instance.
        /// </returns>
        public DateTimeOffset AsDateTimeOffset()
        {
            return this == default(TIME)
                ? default(DateTimeOffset)
                : new DateTimeOffset(new DateTime(1, 1, 1, HOUR, MINUTE, SECOND, Form.AsDateTimeKind()), TimeSpan.Zero);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same
        /// value; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TIME && Equals((TIME)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TIME other) =>
            HOUR == other.HOUR
            && MINUTE == other.MINUTE
            && SECOND == other.SECOND
            && Form == other.Form;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HOUR;
                hashCode = (hashCode * 397) ^ MINUTE;
                hashCode = (hashCode * 397) ^ SECOND;
                hashCode = (hashCode * 397) ^ (int)Form;
                return hashCode;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value
        /// has the following meanings: Value Meaning Less than zero This object is less than the
        /// <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>.
        /// Greater than zero This object is greater than <paramref name="other"/>.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(TIME other)
        {
            if (HOUR > other.HOUR) return 1;
            if (MINUTE < other.MINUTE) return -1;
            if (MINUTE > other.MINUTE) return 1;
            if (SECOND < other.SECOND) return -1;
            if (SECOND > other.SECOND) return 1;
            return 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same
        /// position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value
        /// has these meanings: Value Meaning Less than zero This instance precedes <paramref
        /// name="obj"/> in the sort order. Zero This instance occurs in the same position in the
        /// sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref
        /// name="obj"/> in the sort order.
        /// </returns>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is TIME) return CompareTo((TIME)obj);
            throw new ArgumentException(nameof(obj) + " is not a valid time");
        }

        public TIME AddSeconds(double value) => (TIME)AsDateTime().AddSeconds(value);

        public TIME AddMinutes(double value) => (TIME)AsDateTime().AddMinutes(value);

        public TIME AddHours(double value) => (TIME)AsDateTime().AddHours(value);

        public TIME Add(DURATION duration) => (TIME)AsDateTime().Add(duration.AsTimeSpan());

        public TIME Subtract(DURATION duration) => (TIME)AsDateTime().Subtract(duration.AsTimeSpan());

        public DURATION Subtract(TIME other) => AsDateTime().Subtract(other.AsDateTime());

        public static explicit operator TIME(DateTime datetime) => new TIME(datetime);

        public static implicit operator DateTime(TIME time) => time.AsDateTime();

        public static explicit operator TIME(DateTimeOffset datetime) => new TIME(datetime);

        public static implicit operator DateTimeOffset(TIME time) => time.AsDateTimeOffset();

        public static TIME operator +(TIME time, DURATION duration) => time.Add(duration);

        public static TIME operator -(TIME time, DURATION duration) => time.Subtract(duration);

        public static DURATION operator -(TIME time, TIME other) => time.Subtract(other);

        /// <summary>
        /// Determines whether one specified <see cref="TIME"/> is earlier than another specified
        /// <see cref="TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator <(TIME left, TIME right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="TIME"/> is later than another specified <see cref="TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator >(TIME left, TIME right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="TIME"/> is earlier than or the same as
        /// another specified <see cref="TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator <=(TIME left, TIME right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="TIME"/> is later than or the same as another
        /// specified <see cref="TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator >=(TIME left, TIME right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Determines whether two specified instances of <see cref="TIME"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> represent the same time
        /// instance; otherwise false.
        /// </returns>
        public static bool operator ==(TIME left, TIME right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="TIME"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> do not represent the same
        /// time instance; otherwise false.
        /// </returns>
        public static bool operator !=(TIME left, TIME right) => !left.Equals(right);

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => Form == TIME_FORM.LOCAL
            ? $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}"
            : $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z";
    }
}