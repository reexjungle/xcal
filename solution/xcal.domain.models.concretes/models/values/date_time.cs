using reexjungle.xcal.core.domain.concretes.extensions;
using reexjungle.xcal.core.domain.concretes.models.properties;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xcal.core.domain.contracts.serialization;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.concretes.models.values
{
    [DataContract]
    public struct DATE_TIME : IDATE_TIME, IEquatable<DATE_TIME>, IComparable, IComparable<DATE_TIME>, ICalendarSerializable
    {
        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013
        /// </summary>
        [DataMember]
        public uint FULLYEAR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        [DataMember]
        public uint MONTH { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        [DataMember]
        public uint MDAY { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        [DataMember]
        public uint HOUR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representatio of a minute.
        /// </summary>
        [DataMember]
        public uint MINUTE { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a second.
        /// </summary>
        [DataMember]
        public uint SECOND { get; private set; }

        /// <summary>
        /// Gets the form in which the <see cref="ITIME"/> instance is expressed.
        /// </summary>
        [DataMember]
        public TIME_FORM Form { get; private set; }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        [DataMember]
        public ITZID TimeZoneId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with the year, month and day.
        /// </summary>
        /// <param name="fullyear">The 4-digit representation of a full year e.g. 2013.</param>
        /// <param name="month">The 2-digit representation of a month.</param>
        /// <param name="mday">The 2-digit representation of a day of the month.</param>
        /// <param name="hour">The digit representation of an hour.</param>
        /// <param name="minute">The digit representation of a minute.</param>
        /// <param name="second">The digit representation of a second.</param>
        /// <param name="form">The form in which the <see cref="DATE_TIME"/> instance is expressed.</param>
        /// <param name="tzid">The identifier that references the time zone.</param>
        public DATE_TIME(uint fullyear, uint month, uint mday, uint hour, uint minute, uint second, TIME_FORM form = TIME_FORM.LOCAL, ITZID tzid = null)
        {
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
            TimeZoneId = tzid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with an <see
        /// cref="IDATE_TIME"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="IDATE_TIME"/> instance that initializes this instance.</param>
        public DATE_TIME(IDATE_TIME other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            FULLYEAR = other.FULLYEAR;
            MONTH = other.MONTH;
            MDAY = other.MDAY;
            HOUR = other.HOUR;
            MINUTE = other.MINUTE;
            SECOND = other.SECOND;
            Form = other.Form;
            TimeZoneId = other.TimeZoneId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with an <see
        /// cref="IDATE"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="IDATE"/> instance that initializes this instance.</param>
        public DATE_TIME(IDATE other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            FULLYEAR = other.FULLYEAR;
            MONTH = other.MONTH;
            MDAY = other.MDAY;
            HOUR = 0u;
            MINUTE = 0u;
            SECOND = 0u;
            Form = TIME_FORM.UNSPECIFIED;
            TimeZoneId = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with an <see
        /// cref="ITIME"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="ITIME"/> instance that initializes this instance.</param>
        public DATE_TIME(ITIME other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            FULLYEAR = 1u;
            MONTH = 1u;
            MDAY = 1u;
            HOUR = other.HOUR;
            MINUTE = other.MINUTE;
            SECOND = other.SECOND;
            Form = other.Form;
            TimeZoneId = other.TimeZoneId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with a <see
        /// cref="DateTime"/> instance
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> that initializes this instance.</param>
        /// <param name="tzinfo">Any time zone in the world.</param>
        public DATE_TIME(DateTime datetime, TimeZoneInfo tzinfo)
        {
            FULLYEAR = (uint)datetime.Year;
            MONTH = (uint)datetime.Year;
            MDAY = (uint)datetime.Day;
            HOUR = (uint)datetime.Hour;
            MINUTE = (uint)datetime.Minute;
            SECOND = (uint)datetime.Second;
            Form = datetime.Kind.AsTIME_FORM(tzinfo);
            TimeZoneId = tzinfo != null
                ? new TZID(null, tzinfo.Id)
                : default(TZID);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct with the specified <see cref="DateTimeOffset"/>.
        /// <para/>
        /// Note: By using this constructor, the new instance of <see cref="DATE_TIME"/> shall always
        ///       be initialized as UTC time.
        /// </summary>
        /// <param name="datetime">
        /// A point in time, typically expressed as date and time of day, relative to Coordinate
        /// Universal Time (UTC).
        /// </param>
        public DATE_TIME(DateTimeOffset datetime)
        {
            FULLYEAR = (uint)datetime.UtcDateTime.Year;
            MONTH = (uint)datetime.UtcDateTime.Year;
            MDAY = (uint)datetime.UtcDateTime.Day;
            HOUR = (uint)datetime.UtcDateTime.Hour;
            MINUTE = (uint)datetime.UtcDateTime.Minute;
            SECOND = (uint)datetime.UtcDateTime.Second;
            Form = TIME_FORM.UTC;
            TimeZoneId = null;
        }

        /// <summary>
        /// Initializes a new instance of the the <see cref="DATE_TIME"/> structure with a string value.
        /// </summary>
        /// <param name="value">
        /// The string representation of a <see cref="DATE_TIME"/> to initialize the new <see
        /// cref="DATE_TIME"/> instance with.
        /// </param>
        public DATE_TIME(string value)
        {
            var datetime = Parse(value);
            FULLYEAR = datetime.FULLYEAR;
            MONTH = datetime.MONTH;
            MDAY = datetime.MDAY;
            HOUR = datetime.HOUR;
            MINUTE = datetime.MINUTE;
            SECOND = datetime.SECOND;
            Form = datetime.Form;
            TimeZoneId = datetime.TimeZoneId;
        }

        private static DATE_TIME Parse(string value)
        {
            var fullyear = 1u;
            var month = 1u;
            var mday = 1u;
            var hour = 0u;
            var minute = 0u;
            var second = 0u;
            ITZID tzid = null;
            var form = TIME_FORM.UNSPECIFIED;

            const string pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})T(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})(?<utc>Z)?$";
            const RegexOptions options = RegexOptions.IgnoreCase
                                         | RegexOptions.CultureInvariant
                                         | RegexOptions.ExplicitCapture
                                         | RegexOptions.Compiled;

            foreach (Match match in Regex.Matches(value, pattern, options))
            {
                if (match.Groups["year"].Success) fullyear = uint.Parse(match.Groups["year"].Value);
                if (match.Groups["month"].Success) month = uint.Parse(match.Groups["month"].Value);
                if (match.Groups["day"].Success) mday = uint.Parse(match.Groups["day"].Value);
                if (match.Groups["hour"].Success) hour = uint.Parse(match.Groups["hour"].Value);
                if (match.Groups["min"].Success) minute = uint.Parse(match.Groups["min"].Value);
                if (match.Groups["sec"].Success) second = uint.Parse(match.Groups["sec"].Value);
                if (match.Groups["utc"].Success) form = TIME_FORM.UTC;
                else if (match.Groups["tzid"].Success)
                {
                    tzid = new TZID(match.Groups["tzid"].Value);
                    form = TIME_FORM.LOCAL_TIMEZONE_REF;
                }
                else form = TIME_FORM.LOCAL;
            }

            return new DATE_TIME(fullyear, month, mday, hour, minute, second, form, tzid);
        }

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of seconds
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddSeconds(double value) => AsDateTime().AddSeconds(value).AsDATE_TIME();

        /// <summary>
        /// Adds the specified number of minutes to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of minutes
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddMinutes(double value) => AsDateTime().AddMinutes(value).AsDATE_TIME();

        /// <summary>
        /// Adds the specified number of hours to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of hours
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddHours(double value) => AsDateTime().AddHours(value).AsDATE_TIME();

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="System.DateTime"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="System.DateTime"/> respresentation of this date instance.
        /// </returns>
        public DateTime AsDateTime() => this == default(DATE_TIME)
            ? default(DateTime)
            : new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY, (int)HOUR, (int)MINUTE, (int)SECOND, Form.AsDateTimeKind(TimeZoneId));

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="System.DateTimeOffset"/> representation.
        /// </summary>
        /// <param name="func">Function to determine the offset from the time zone reference.</param>
        /// <returns>
        /// The equivalent <see cref="System.DateTimeOffset"/> respresentation of this date instance.
        /// </returns>
        public DateTimeOffset AsDateTimeOffset(Func<ITZID, IUTC_OFFSET> func = null)
        {
            if (this == default(DATE_TIME)) return default(DateTimeOffset);

            if (Form == TIME_FORM.LOCAL || Form == TIME_FORM.UTC)
                return new DateTimeOffset(AsDateTime(), TimeSpan.Zero);

            if (Form == TIME_FORM.LOCAL_TIMEZONE_REF && TimeZoneId != null && func != null)
            {
                var offset = func(TimeZoneId);
                return new DateTimeOffset(AsDateTime(), new TimeSpan(offset.HOURS, offset.MINUTES, offset.SECONDS));
            }
            //Unspecified time form
            return new DateTimeOffset(AsDateTime());
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DATE_TIME other)
        {
            return FULLYEAR == other.FULLYEAR
                && MONTH == other.MONTH
                && MDAY == other.MDAY
                && HOUR == other.HOUR
                && MINUTE == other.MINUTE
                && SECOND == other.SECOND
                && Form == other.Form
                && Equals(TimeZoneId, other.TimeZoneId);
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
            return obj is DATE_TIME && Equals((DATE_TIME)obj);
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
                var hashCode = (int)FULLYEAR;
                hashCode = (hashCode * 397) ^ (int)MONTH;
                hashCode = (hashCode * 397) ^ (int)MDAY;
                hashCode = (hashCode * 397) ^ (int)HOUR;
                hashCode = (hashCode * 397) ^ (int)MINUTE;
                hashCode = (hashCode * 397) ^ (int)SECOND;
                hashCode = (hashCode * 397) ^ (int)Form;
                hashCode = (hashCode * 397) ^ (TimeZoneId?.GetHashCode() ?? 0);
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
            if (obj == null) return 1;
            if (obj is DATE_TIME) return CompareTo((DATE_TIME)obj);
            throw new ArgumentException(nameof(obj) + " is not a date time");
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
                    writer.WriteValue($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                case TIME_FORM.UTC:
                    writer.WriteValue($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z");
                    break;

                case TIME_FORM.LOCAL_TIMEZONE_REF:
                    writer.WriteValue($"{TimeZoneId}:{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
                    break;

                default:
                    writer.WriteValue($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}");
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
                    var datetime = Parse(inner.Value);
                    FULLYEAR = datetime.FULLYEAR;
                    MONTH = datetime.MONTH;
                    MDAY = datetime.MDAY;
                    HOUR = datetime.HOUR;
                    MINUTE = datetime.MINUTE;
                    SECOND = datetime.SECOND;
                    Form = datetime.Form;
                    TimeZoneId = datetime.TimeZoneId;
                }
            }

            inner.Close();
        }

        public static DATE_TIME operator +(DATE_TIME left, DURATION duration) => left.Add(duration);

        public static DATE_TIME operator -(DATE_TIME left, DURATION duration) => left.Subtract(duration);

        public static DURATION operator -(DATE_TIME left, DATE_TIME right) => left.Subtract(right);

        /// <summary>
        /// Explicitly converts the specified <see cref="DateTime"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static implicit operator DateTime(DATE_TIME datetime) => datetime.AsDateTime();

        public static implicit operator DATE_TIME(DateTime datetime) => new DATE_TIME(datetime);

        /// <summary>
        /// Implicitly converts the specified <see cref="DATE"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="DATE"/> instance to convert.</param>
        public static implicit operator DATE_TIME(DATE date) => new DATE_TIME(date);

        /// <summary>
        /// Explicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static explicit operator DATE(DATE_TIME datetime) => new DATE(datetime.FULLYEAR, datetime.MONTH, datetime.MDAY);

        /// <summary>
        /// Implicitly converts the specified <see cref="TIME"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="time">The <see cref="TIME"/> instance to convert.</param>
        public static implicit operator DATE_TIME(TIME time) => new DATE_TIME(time);

        /// <summary>
        /// Implicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static explicit operator TIME(DATE_TIME datetime) => new TIME(datetime.HOUR, datetime.MINUTE, datetime.SECOND, datetime.Form, datetime.TimeZoneId);

        /// <summary>
        /// Determines whether one specified <see cref="DATE_TIME"/> is earlier than another
        /// specified <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator <(DATE_TIME left, DATE_TIME right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE_TIME"/> is later than another specified
        /// <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator >(DATE_TIME left, DATE_TIME right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE_TIME"/> is earlier than or the same as
        /// another specified <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator <=(DATE_TIME left, DATE_TIME right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE_TIME"/> is later than or the same as
        /// another specified <see cref="DATE_TIME"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator >=(DATE_TIME left, DATE_TIME right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE_TIME"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> represent the same date time
        /// instance; otherwise false.
        /// </returns>
        public static bool operator ==(DATE_TIME left, DATE_TIME right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE_TIME"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> do not represent the same
        /// date time instance; otherwise false.
        /// </returns>
        public static bool operator !=(DATE_TIME left, DATE_TIME right) => !left.Equals(right);

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            switch (Form)
            {
                case TIME_FORM.LOCAL:
                    return $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";

                case TIME_FORM.UTC:
                    return $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z";

                case TIME_FORM.LOCAL_TIMEZONE_REF:
                    return $"{TimeZoneId}:{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";

                default:
                    return $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";
            }
        }
    }
}
