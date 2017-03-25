using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NodaTime;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a duration of time.
    /// </summary>
    [DataContract]
    public struct DURATION : IEquatable<DURATION>, IComparable, IComparable<DURATION>
    {
        /// <summary>
        /// Represents the zero <see cref="DURATION"/> value. This field is readonly.
        /// </summary>
        public static readonly DURATION Zero = new DURATION();

        /// <summary>
        /// Gets the number of weeks in the duration of time.
        /// </summary>
        public int WEEKS { get; }

        /// <summary>
        /// Gets the numbers of hours in the duration of time.
        /// </summary>
        public int HOURS { get; }

        /// <summary>
        /// Gets the number of minutes in the duration of time.
        /// </summary>
        public int MINUTES { get; }

        /// <summary>
        /// Gets the number of seconds in the duration of time.
        /// </summary>
        public int SECONDS { get; }

        /// <summary>
        /// Gets the number of days in the duration of time.
        /// </summary>
        public int DAYS { get; }

        /// <summary>
        /// Iniializes a new instance of the <see cref="DURATION"/> struct with the given weeks,
        /// days, hours, minutes and seconds.
        /// </summary>
        /// <param name="weeks">The number of weeks in the duration of time.</param>
        /// <param name="days">Optional: The number of days in the duration of time.</param>
        /// <param name="hours">Optional: The number of hours in the duration of time.</param>
        /// <param name="minutes">Optional: The number of minutes in the duration of time.</param>
        /// <param name="seconds">Optional: The number of seconds in the duration of time.</param>
        public DURATION(int weeks, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            WEEKS = weeks;
            DAYS = days;
            HOURS = hours;
            MINUTES = minutes;
            SECONDS = seconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DURATION"/> struct with the given time interval.
        /// </summary>
        /// <param name="timespan">The interval for the duration of time.</param>
        public DURATION(TimeSpan timespan)
        {
            var period = Period.FromDays(timespan.Days)
                         + Period.FromHours(timespan.Hours)
                         + Period.FromMinutes(timespan.Minutes)
                         + Period.FromSeconds(timespan.Seconds);

            WEEKS = (int)period.Weeks;
            DAYS = (int)period.Days;
            HOURS = (int)period.Hours;
            MINUTES = (int)period.Minutes;
            SECONDS = (int)period.Seconds;
        }

        public DURATION(Duration duration) : this(duration.ToTimeSpan())
        {
        }

        public DURATION(Period period)
        {
            DAYS = (int)period.Days;
            HOURS = (int)period.Hours;
            MINUTES = (int)period.Minutes;
            SECONDS = (int)period.Seconds;
            WEEKS = (int)period.Weeks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DURATION"/> struct with a string value.
        /// </summary>
        /// <param name="value">
        /// The string representation of a <see cref="DURATION"/> to initialize the new <see
        /// cref="DURATION"/> instance with.
        /// </param>
        public DURATION(string value)
        {
            var weeks = 0;
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            const string pattern = @"^(?<minus>\-)?P((?<weeks>\d*)W)?((?<days>\d*)D)?(T((?<hours>\d*)H)?((?<mins>\d*)M)?((?<secs>\d*)S)?)?$";
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Compiled;
            var regex = new Regex(pattern, options);

            foreach (Match match in regex.Matches(value))
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
            WEEKS = weeks;
            DAYS = days;
            HOURS = hours;
            MINUTES = minutes;
            SECONDS = seconds;
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
        public int CompareTo(DURATION other)
        {
            if (WEEKS < other.WEEKS) return 1;
            if (WEEKS > other.WEEKS) return -1;
            if (DAYS > other.DAYS) return 1;
            if (DAYS < other.DAYS) return -1;
            if (HOURS > other.HOURS) return 1;
            if (HOURS < other.HOURS) return -1;
            if (MINUTES > other.MINUTES) return 1;
            if (MINUTES < other.MINUTES) return -1;
            if (SECONDS > other.SECONDS) return 1;
            if (SECONDS < other.SECONDS) return -1;
            return 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DURATION other)
            => WEEKS == other.WEEKS
            && DAYS == other.DAYS
            && HOURS == other.HOURS
            && MINUTES == other.MINUTES
            && SECONDS == other.SECONDS;

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
            return obj is DURATION && Equals((DURATION)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the specified <see cref="DURATION"/> instance into a <see cref="TimeSpan"/> value.
        /// </summary>
        /// <returns>The equivalent <see cref="TimeSpan"/> resulting from the conversion.</returns>
        public TimeSpan AsTimeSpan() => new TimeSpan(7 * WEEKS + DAYS, HOURS, MINUTES, SECONDS);

        public Duration AsDuration() => Duration.FromTimeSpan(AsTimeSpan());

        public Period AsPeriod()
        {
            return Period.FromWeeks(WEEKS)
                   + Period.FromDays(DAYS)
                   + Period.FromHours(HOURS)
                   + Period.FromMinutes(MINUTES)
                   + Period.FromSeconds(SECONDS);
        }

        /// <summary>
        /// Adds two specified <see cref="DURATION"/> instances.
        /// </summary>
        /// <param name="other">The other duration to add.</param>
        /// <returns>
        /// The duration instance, whose value is the scalar sum of the of the values of this
        /// instance and the <paramref name="other"/> instance.
        /// </returns>
        public DURATION Add(DURATION other)
            => new DURATION(WEEKS + other.WEEKS, DAYS + other.DAYS, HOURS + other.HOURS, MINUTES + other.MINUTES, SECONDS + other.SECONDS);

        /// <summary>
        /// Subtract a specified <see cref="DURATION"/> instance from another.
        /// </summary>
        /// <param name="other">The subtrahend.</param>
        /// <returns>
        /// The duration instance, whose value is the scalar difference of this instance and the
        /// <paramref name="other"/> instance.
        /// </returns>
        public DURATION Subtract(DURATION other)
            => new DURATION(WEEKS - other.WEEKS, DAYS - other.DAYS, HOURS - other.HOURS, MINUTES - other.MINUTES, SECONDS - other.SECONDS);

        /// <summary>
        /// Multiplies a specified <see cref="DURATION"/> instance by a specified scalar value.
        /// </summary>
        /// <param name="scalar">The multiplier.</param>
        /// <returns>
        /// The duration instance, whose value is the scalar multiple of this instance and the
        /// specified <paramref name="scalar"/> value.
        /// </returns>
        public DURATION MultiplyBy(int scalar)
            => new DURATION(WEEKS * scalar, DAYS * scalar, HOURS * scalar, MINUTES * scalar, SECONDS * scalar);

        /// <summary>
        /// Divides a specified <see cref="DURATION"/> instance by a specified scalar value.
        /// </summary>
        /// <param name="scalar">The denominator.</param>
        /// <returns>
        /// The duration instance, whose value is the scalar dividend of the values of this instance
        /// and the specified <paramref name="scalar"/> value.
        /// </returns>
        public DURATION DivideBy(int scalar)
            => new DURATION(WEEKS / scalar, DAYS / scalar, HOURS / scalar, MINUTES / scalar, SECONDS / scalar);

        /// <summary>
        /// Returns a <see cref="DURATION"/> instance, whose value is the negation of this instance.
        /// </summary>
        /// <returns>A duration of time that has the same numeric value as this instance, but opposite sign.</returns>
        public DURATION Negate() => new DURATION(-Math.Abs(WEEKS), -Math.Abs(DAYS), -Math.Abs(HOURS), -Math.Abs(MINUTES), -Math.Abs(SECONDS));

        /// <summary>
        /// Checks if this duration instance is negative.
        /// </summary>
        /// <returns>True if this </returns>
        public bool IsNegative() => this < Zero;

        public static implicit operator TimeSpan(DURATION duration) => duration.AsTimeSpan();

        public static implicit operator DURATION(TimeSpan timespan) => new DURATION(timespan);

        public static implicit operator Duration(DURATION duration) => duration.AsDuration();

        public static implicit operator DURATION(Duration duration) => new DURATION(duration);

        public static implicit operator Period(DURATION duration) => duration.AsPeriod();

        public static implicit operator DURATION(Period period) => new DURATION(period);

        /// <summary>
        /// Returns a <see cref="DURATION"/> instance, whose value is the negation of the specified duration of time.
        /// </summary>
        /// <param name="duration">The duration of time to be negated.</param>
        /// <returns>A duration of time that has the same numeric value as the specified duration of time, but opposite sign.</returns>
        public static DURATION operator -(DURATION duration) => duration.Negate();

        /// <summary>
        /// Returns the specified instance of <see cref="DURATION"/>.
        /// Each numeric property of the duration of time remains unchanged.
        /// </summary>
        /// <param name="duration">The duration of time to return.</param>
        /// <returns>The duration of time specified by <paramref name="duration"/>.</returns>
        public static DURATION operator +(DURATION duration) => duration;

        /// <summary>
        /// Adds two specified <see cref="DURATION"/> instances.
        /// </summary>
        /// <param name="left">The first duration of time to add.</param>
        /// <param name="right">The second duration of time to add.</param>
        /// <returns>A duration of time, whose value is the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static DURATION operator +(DURATION left, DURATION right) => left.Add(right);

        /// <summary>
        /// Subtracts a specified <see cref="DURATION"/> instance from another.
        /// </summary>
        /// <param name="left">The first duration of time to subtract from (the minuend).</param>
        /// <param name="right">The second duration of time that is subtracted (the subtrahend).</param>
        /// <returns>A duration of time, whose value is the value of <paramref name="left"/> minus <paramref name="right"/>.</returns>
        public static DURATION operator -(DURATION left, DURATION right) => left.Subtract(right);

        /// <summary>
        /// Multiplies a specified duration of time by a scalar value.
        /// </summary>
        /// <param name="duration">The duration of time to be multiplied.</param>
        /// <param name="scalar">The multiplier.</param>
        /// <returns>The duration instance, whose value is the scalar multiple of this instance and the
        /// specified <paramref name="scalar"/> value.</returns>
        public static DURATION operator *(DURATION duration, int scalar) => duration.MultiplyBy(scalar);

        /// <summary>
        /// Divides a specified duration of time by a scalar value.
        /// </summary>
        /// <param name="duration">The duration of time to be divided.</param>
        /// <param name="scalar">The denominator.</param>
        /// <returns>The duration instance, whose value is the scalar dividend of this instance and the
        /// specified <paramref name="scalar"/> value.</returns>
        public static DURATION operator /(DURATION duration, int scalar) => duration.DivideBy(scalar);

        /// <summary>
        /// Determines if the specified <see cref="DURATION"/> is less than another specified <see cref="DURATION"/> instance.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the value of <paramref name="left"/> is less than the value of <paramref name="right"/>; otherwise false.</returns>
        public static bool operator <(DURATION left, DURATION right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines if the specified <see cref="DURATION"/> is greater than another specified <see cref="DURATION"/> instance.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the value of <paramref name="left"/> is greater than the value of <paramref name="right"/>; otherwise false.</returns>
        public static bool operator >(DURATION left, DURATION right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines if the specified <see cref="DURATION"/> is less than or equal to another specified <see cref="DURATION"/> instance.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the value of <paramref name="left"/> is less than or equal to the value of <paramref name="right"/>; otherwise false.</returns>
        public static bool operator <=(DURATION left, DURATION right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines if the specified <see cref="DURATION"/> is greater than or equal to another specified <see cref="DURATION"/> instance.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the value of <paramref name="left"/> is greater than or equal to the value of <paramref name="right"/>; otherwise false.</returns>
        public static bool operator >=(DURATION left, DURATION right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Determines if two instances of <see cref="DURATION"/> are equal.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(DURATION left, DURATION right) => left.Equals(right);

        /// <summary>
        /// Determines if two instances of <see cref="DURATION"/> are not equal.
        /// </summary>
        /// <param name="left">The first duration of time to compare.</param>
        /// <param name="right">The second duration of time to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(DURATION left, DURATION right) => !left.Equals(right);

    }
}
