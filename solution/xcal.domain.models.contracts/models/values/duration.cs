using reexjungle.xcal.core.domain.contracts.extensions;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a duration of time.
    /// </summary>
    [DataContract]
    public struct DURATION : IEquatable<DURATION>, IComparable, IComparable<DURATION>, IConvertible, ICalendarSerializable
    {
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
        /// <param name="span">The interval for the duration of time.</param>
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
        /// Initializes a new instance of the <see cref="DURATION"/> struct with a string value.
        /// </summary>
        /// <param name="value">
        /// The string representation of a <see cref="DURATION"/> to initialize the new <see
        /// cref="DURATION"/> instance with.
        /// </param>
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

            return new DURATION(weeks, days, hours, minutes, seconds);
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
        /// Gets the number of weeks in the duration of time.
        /// </summary>
        public int WEEKS { get; private set; }

        /// <summary>
        /// Gets the numbers of hours in the duration of time.
        /// </summary>
        public int HOURS { get; private set; }

        /// <summary>
        /// Gets the number of minutes in the duration of time.
        /// </summary>
        public int MINUTES { get; private set; }

        /// <summary>
        /// Gets the number of seconds in the duration of time.
        /// </summary>
        public int SECONDS { get; private set; }

        /// <summary>
        /// Gets the number of days in the duration of time.
        /// </summary>
        public int DAYS { get; private set; }

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
        /// Can the object be generated from its iCalendar representation?
        /// </summary>
        /// <returns>
        /// True if the object can be deserialized from its iCalendar representation, otherwise false.
        /// </returns>
        public bool CanDeserialize() => true;

        /// <summary>
        /// Converts an object into its iCalendar representation.
        /// </summary>
        /// <param name="writer">The iCalendar writer used to serialize the object.</param>
        public void WriteCalendar(ICalendarWriter writer)
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
            writer.WriteValue(sb.ToString());
        }

        /// <summary>
        /// Generates an object from its iCalendar representation.
        /// </summary>
        /// <param name="reader">
        /// The iCalendar reader used to deserialize data into the iCalendar object.
        /// </param>
        /// <returns>True if the deserialization operation was successful; otherwise false.</returns>
        public void ReadCalendar(ICalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.Read())
            {
                if (inner.NodeType != NodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var duration = Parse(inner.Value);
                    WEEKS = WEEKS;
                    DAYS = DAYS;
                    HOURS = HOURS;
                    MINUTES = MINUTES;
                    SECONDS = SECONDS;
                }
            }

            inner.Close();
        }

        /// <summary>
        /// Can the object be converted to its iCalendar representation?
        /// </summary>
        /// <returns>
        /// True if the object can be serialized to its iCalendar representation, otherwise false.
        /// </returns>
        public bool CanSerialize() => true;

        /// <summary>
        /// Converts the specified <see cref="DURATION"/> instance into a <see cref="TimeSpan"/> value.
        /// </summary>
        /// <returns>The equivalent <see cref="TimeSpan"/> resulting from the conversion.</returns>
        public TimeSpan AsTimeSpan() => new TimeSpan(7 * WEEKS + DAYS, HOURS, MINUTES, SECONDS);

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


        public static implicit operator TimeSpan(DURATION duration) => duration.AsTimeSpan();

        public static implicit operator DURATION(TimeSpan span) => span.AsDURATION();


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

        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value
        /// type that implements this interface.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public TypeCode GetTypeCode() => TypeCode.Object;

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified
        /// culture-specific formatting information.
        /// </summary>
        /// <returns>A Boolean value equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Boolean));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>A Unicode character equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Char));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(SByte));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Byte));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Int16));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(UInt16));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Int32));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Int32));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Int64));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(UInt64));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Single));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Double));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/>
        /// number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(Decimal));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/>
        /// using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + nameof(DateTime));
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using
        /// the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider provider) => ToString();

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified
        /// <see cref="T:System.Type"/> that has an equivalent value, using the specified
        /// culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose
        /// value is equivalent to the value of this instance.
        /// </returns>
        /// <param name="conversionType">
        /// The <see cref="T:System.Type"/> to which the value of this instance is converted.
        /// </param>
        /// <param name="provider">
        /// An <see cref="T:System.IFormatProvider"/> interface implementation that supplies
        /// culture-specific formatting information.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(TimeSpan)) return AsTimeSpan();
            if (conversionType == typeof(string)) return ToString();
            throw new InvalidCastException("Invalid Cast from type" + nameof(DURATION) + "to type " + conversionType.FullName);
        }
    }
}
