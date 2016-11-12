using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using reexjungle.xcal.core.domain.contracts.extensions;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.serialization;
using reexjungle.xmisc.foundation.concretes;

namespace reexjungle.xcal.core.domain.contracts.models.values
{

    /// <summary>
    /// Represents a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    [DataContract]
    public struct DATE : IEquatable<DATE>, IComparable, IComparable<DATE>, ICalendarSerializable
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
        /// Initializes a new instance of the <see cref="DATE"/> structure with the year, month and day.
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
        /// Initializes a new instance of the <see cref="DATE"/> structure with an <see
        /// cref="IDATE"/> instance
        /// </summary>
        /// <param name="other">The <see cref="IDATE"/> instance that initializes this instance.</param>
        public DATE(DATE other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            FULLYEAR = other.FULLYEAR;
            MONTH = other.MONTH;
            MDAY = other.MDAY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with a <see
        /// cref="DateTime"/> instance
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> that initializes this instance.</param>
        public DATE(DateTime date)
        {
            FULLYEAR = (uint)date.Year;
            MONTH = (uint)date.Year;
            MDAY = (uint)date.Day;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> struct with the specified <see cref="DateTimeOffset"/>.
        /// <para/>
        /// Note: By using this constructor, the new instance of <see cref="DATE"/> shall always be
        ///       initialized as UTC date time.
        /// </summary>
        /// <param name="datetime">
        /// A point in time, typically expressed as date and time of day, relative to Coordinate
        /// Universal Time (UTC).
        /// </param>
        public DATE(DateTimeOffset datetime)
        {
            FULLYEAR = (uint)datetime.UtcDateTime.Year;
            MONTH = (uint)datetime.UtcDateTime.Year;
            MDAY = (uint)datetime.UtcDateTime.Day;
        }

        /// <summary>
        /// Initializes a new instance of the the <see cref="DATE"/> structure with a string value.
        /// </summary>
        /// <param name="value">
        /// The string representation of a <see cref="DATE"/> to initialize the new <see
        /// cref="DATE"/> instance with.
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
            var fullyear = 1u;
            var month = 1u;
            var mday = 1u;
            const string pattern = @"^(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})$";
            const RegexOptions options = RegexOptions.IgnoreCase
                                         | RegexOptions.CultureInvariant
                                         | RegexOptions.ExplicitCapture
                                         | RegexOptions.Compiled;

            foreach (Match match in Regex.Matches(value, pattern, options))
            {
                if (match.Groups["year"].Success) fullyear = uint.Parse(match.Groups["year"].Value);
                if (match.Groups["month"].Success) month = uint.Parse(match.Groups["month"].Value);
                if (match.Groups["day"].Success) mday = uint.Parse(match.Groups["day"].Value);
            }

            return new DATE(fullyear, month, mday);
        }

        public  DateTime AsDateTime() => this == default(DATE)
            ? default(DateTime)
            : new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY);



        /// <summary>
        /// Gets the week day of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <returns>The weekday of the </returns>
        public  WEEKDAY GetWeekday() => AsDateTime().DayOfWeek.AsWEEKDAY();

        public  DATE AddDays(double value) => AsDateTime().AddDays(value).AsDATE();

        public  DATE AddWeeks(int value) => AsDateTime().AddWeeks(value).AsDATE();

        public  DATE AddMonths(int value) => AsDateTime().AddMonths(value).AsDATE();

        public  DATE AddYears(int value) => AsDateTime().AddYears(value).AsDATE();

        public  DATE Add(DURATION duration) => AsDateTime().Add(duration.AsTimeSpan()).AsDATE();

        public  DATE Subtract(DURATION duration) => AsDateTime().Subtract(duration.AsTimeSpan()).AsDATE();

        public  DURATION Subtract(DATE other) => AsDateTime().Subtract(other.AsDateTime()).AsDURATION();


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DATE other) => FULLYEAR == other.FULLYEAR && MONTH == other.MONTH && MDAY == other.MDAY;

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
        public int CompareTo(DATE other)
        {
            if (this == default(DATE) && other == default(DATE)) return 0;
            if (FULLYEAR < other.FULLYEAR) return -1;
            if (FULLYEAR > other.FULLYEAR) return 1;
            if (MONTH < other.MONTH) return -1;
            if (MONTH > other.MONTH) return 1;
            if (MDAY < other.MDAY) return -1;
            return MDAY > other.MDAY ? 1 : 0;
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
            return obj is DATE && Equals((DATE)obj);
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
            if (obj is DATE) return CompareTo((DATE)obj);
            throw new ArgumentException(nameof(obj) + " is not a date");
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
            writer.WriteValue($"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}");
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
                    var date = Parse(inner.Value);
                    FULLYEAR = date.FULLYEAR;
                    MONTH = date.MONTH;
                    MDAY = date.MDAY;
                }
            }
        }

        public static DATE operator +(DATE date, DURATION duration) => date.Add(duration);

        public static DATE operator -(DATE date, DURATION duration) => date.Subtract(duration);

        public static DURATION operator -(DATE left, DATE right) => left.Subtract(right);

        /// <summary>
        /// Converts the <see cref="DATE"/> implicitly to an equivalent <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is implicitly converted to the <see cref="DateTime"/> instance.
        /// </param>
        public static implicit operator DateTime(DATE date) => date.AsDateTime();

        /// <summary>
        /// Converts the <see cref="DATE_TIME"/> instance explicitly to an equivalent <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> instance that is explicitly converted to the <see cref="DATE"/> instance.</param>
        public static explicit operator DATE(DateTime datetime) => new DATE(datetime);

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is earlier than another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise false.</returns>
        public static bool operator <(DATE left, DATE right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is later than another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise false.</returns>
        public static bool operator >(DATE left, DATE right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is earlier than or the same as another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than or the same as <paramref name="right"/>; otherwise false.</returns>
        public static bool operator <=(DATE left, DATE right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is later than or the same as another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than or the same as <paramref name="right"/>; otherwise false.</returns>
        public static bool operator >=(DATE left, DATE right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> represent the same date instance; otherwise false.</returns>
        public static bool operator ==(DATE left, DATE right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> do not represent the same date instance; otherwise false.</returns>
        public static bool operator !=(DATE left, DATE right) => !left.Equals(right);


        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}";
    }
}
