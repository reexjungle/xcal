using reexjungle.xcal.core.domain.concretes.extensions;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.concretes.models.values
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
    public struct TIME : ITIME, ITIME<TIME>, IEquatable<TIME>, IComparable, IComparable<TIME>, ICalendarSerializable
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        public uint HOUR { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a minute.
        /// </summary>
        public uint MINUTE { get; private set; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second.
        /// </summary>
        public uint SECOND { get; private set; }

        /// <summary>
        /// Gets the form in which the <see cref="TIME"/> instance is expressed.
        /// </summary>
        public TIME_FORM Form { get; private set; }

        /// <summary>
        /// Gets the identifier that references the time zone.
        /// </summary>
        public ITZID TimeZoneId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified hour,
        /// minute, second, time form and optionally the time zone identifier.
        /// </summary>
        /// <param name="hour">The digit representation of an hour.</param>
        /// <param name="minute">The digit representation of a minute.</param>
        /// <param name="second">The digit representation of a second.</param>
        /// <param name="form">The form in which the <see cref="TIME"/> instance is expressed.</param>
        /// <param name="tzid">The identifier that references the time zone.</param>
        public TIME(uint hour, uint minute, uint second, TIME_FORM form = TIME_FORM.LOCAL, ITZID tzid = null)
        {
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
            TimeZoneId = tzid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified <see
        /// cref="DateTime"/> and <see cref="TimeZoneInfo"/>.
        /// </summary>
        /// <param name="datetime">A point in time, typically expressed as date and time of day.</param>
        /// <param name="tzinfo">Any time zone in the world.</param>
        public TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            TimeZoneId = null;
            Form = TIME_FORM.UNSPECIFIED;
            if (tzinfo != null)
            {
                //TimeZoneId = new TZID(null, tzinfo.Id);
            }
            Form = datetime.Kind.AsTIME_FORM(tzinfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with the specified <see cref="DateTimeOffset"/>.
        /// <para/>
        /// The <see cref="DateTimeOffset.UtcDateTime"/> component initializes the components of the
        /// new instance of <see cref="TIME"/>. This implies, the <see cref="Form"/> property is set
        /// by default to <see cref="TIME_FORM.UTC"/>.
        /// </summary>
        /// <param name="datetime">
        /// A point in time, typically expressed as date and time of day, relative to Coordinate
        /// Universal Time (UTC).
        /// </param>
        public TIME(DateTimeOffset datetime)
        {
            HOUR = (uint)datetime.UtcDateTime.Hour;
            MINUTE = (uint)datetime.UtcDateTime.Minute;
            SECOND = (uint)datetime.UtcDateTime.Second;
            Form = TIME_FORM.UTC;
            TimeZoneId = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TIME"/> struct with its equivalent string representation.
        /// </summary>
        /// <param name="value">The string representation of the <see cref="TIME"/> instance.</param>
        public TIME(string value)
        {
            var time = Parse(value);
            HOUR = time.HOUR;
            MINUTE = time.MINUTE;
            SECOND = time.SECOND;
            TimeZoneId = time.TimeZoneId;
            Form = time.Form;
        }

        private static TIME Parse(string value)
        {
            var hour = 0u;
            var minute = 0u;
            var second = 0u;
            ITZID tzid = null;
            var form = TIME_FORM.UNSPECIFIED;

            var pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?T(?<hour>\d{1,2})(?<min>\d{1,2})(?<sec>\d{1,2})(?<utc>Z)?$";
            var options = RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.ExplicitCapture;

            if (Regex.IsMatch(value, pattern))
            {
                foreach (Match match in Regex.Matches(value, pattern, options))
                {
                    if (match.Groups["hour"].Success) hour = uint.Parse(match.Groups["hour"].Value);
                    if (match.Groups["min"].Success) minute = uint.Parse(match.Groups["min"].Value);
                    if (match.Groups["sec"].Success) second = uint.Parse(match.Groups["sec"].Value);
                    if (match.Groups["utc"].Success) form = TIME_FORM.UTC;
                    else if (match.Groups["tzid"].Success)
                    {
                        //TODO: Add implementation of Time Zone Reference
                        //tzid = new TZID(match.Groups["tzid"].Value);
                        form = TIME_FORM.LOCAL_TIMEZONE_REF;
                    }
                    else form = TIME_FORM.LOCAL;
                }
            }

            return new TIME(hour, minute, second, form, tzid);
        }

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of seconds
        /// to the value of this instance.
        /// </returns>
        public TIME AddSeconds(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of minutes to the value of the <see cref="TIME"/> instance.
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>
        /// A new instance of type <see cref="TIME"/> that adds the specified number of minutes to
        /// the value of this instance.
        /// </returns>
        public TIME AddMinutes(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the specified number of hours to the value of the <see cref="TIME"/> instance.
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>
        /// A new instance of type <see cref="TIME"/> that adds the specified number of hours to the
        /// value of this instance.
        /// </returns>
        public TIME AddHours(double value)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        public DateTime AsDateTime() => this == default(TIME)
            ? default(DateTime)
            : new DateTime(1, 1, 1, (int)HOUR, (int)MINUTE, (int)SECOND, Form.AsDateTimeKind(TimeZoneId));

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="System.DateTimeOffset"/> representation.
        /// </summary>
        /// <param name="func">Function to determine the offset from the time zone reference.</param>
        /// <returns>
        /// The equivalent <see cref="System.DateTimeOffset"/> respresentation of this date instance.
        /// </returns>
        public DateTimeOffset AsDateTimeOffset(Func<ITZID, IUTC_OFFSET> func = null)
        {
            if (this == default(TIME)) return default(DateTimeOffset);

            if (Form == TIME_FORM.LOCAL || Form == TIME_FORM.UTC)
                return new DateTimeOffset(AsDateTime(), TimeSpan.Zero);

            if (Form == TIME_FORM.LOCAL_TIMEZONE_REF && TimeZoneId != null && func != null)
            {
                var offset = func(TimeZoneId);
                return new DateTimeOffset(AsDateTime(), new TimeSpan(offset.HOUR, offset.MINUTE, offset.SECOND));
            }
            //Unspecified time form
            return new DateTimeOffset(AsDateTime());
        }

        public static explicit operator DateTime(TIME time) => time.AsDateTime();

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
            && Form == other.Form
            && Equals(TimeZoneId, other.TimeZoneId);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)HOUR;
                hashCode = (hashCode * 397) ^ (int)MINUTE;
                hashCode = (hashCode * 397) ^ (int)SECOND;
                hashCode = (hashCode * 397) ^ (int)Form;
                hashCode = (hashCode * 397) ^ (TimeZoneId != null ? TimeZoneId.GetHashCode() : 0);
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
            throw new ArgumentException(nameof(obj) + " is not left time");
        }

        /// <summary>
        /// Can the object be converted to its iCalendar representation?
        /// </summary>
        /// <returns>
        /// True if the object can be serialized to its iCalendar representation, otherwise false.
        /// </returns>
        public bool CanSerialize() => true;

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
            switch (Form)
            {
                case TIME_FORM.LOCAL:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                case TIME_FORM.UTC:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z");
                    break;

                case TIME_FORM.LOCAL_TIMEZONE_REF:
                    writer.WriteValue($"{TimeZoneId}:T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                default:
                    writer.WriteValue($"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
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
        public void ReadCalendar(ICalendarReader reader)
        {
            var inner = reader.ReadFragment();
            while (inner.Read())
            {
                if (inner.NodeType != NodeType.VALUE) continue;
                if (!string.IsNullOrEmpty(inner.Value) && !string.IsNullOrWhiteSpace(inner.Value))
                {
                    var time = Parse(inner.Value);
                    HOUR = time.HOUR;
                    MINUTE = time.MINUTE;
                    SECOND = time.SECOND;
                    Form = time.Form;
                    TimeZoneId = time.TimeZoneId;
                }
            }

            inner.Close();
        }

        public static bool operator <(TIME left, TIME right) => left.CompareTo(right) < 0;

        public static bool operator >(TIME left, TIME right) => left.CompareTo(right) > 0;

        public static bool operator <=(TIME left, TIME right) => left.CompareTo(right) <= 0;

        public static bool operator >=(TIME left, TIME right) => left.CompareTo(right) >= 0;

        public static bool operator ==(TIME left, TIME right) => left.Equals(right);

        public static bool operator !=(TIME left, TIME right) => !left.Equals(right);

        public override string ToString()
        {
            switch (Form)
            {
                case TIME_FORM.LOCAL: return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";

                case TIME_FORM.UTC: return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z";

                case TIME_FORM.LOCAL_TIMEZONE_REF: return $"{TimeZoneId}:T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";

                default: return $"T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";
            }
        }
    }
}
