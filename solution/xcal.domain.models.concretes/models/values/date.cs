using reexjungle.xcal.core.domain.concretes.extensions;
using reexjungle.xcal.core.domain.contracts.io.readers;
using reexjungle.xcal.core.domain.contracts.io.writers;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xcal.core.domain.contracts.serialization;
using reexjungle.xmisc.foundation.concretes;
using System;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.concretes.models.values
{
    /// <summary>
    /// Represents a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    public struct DATE : IDATE, IDATE<DATE>, IEquatable<DATE>, IComparable, IComparable<DATE>, ICalendarSerializable
    {
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
        public DATE(IDATE other)
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

        /// <summary>
        /// Adds the specified number of days to the value of the <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="value">The number of days to add.</param>
        /// <returns>
        /// A new <see cref="DATE"/>&gt; that adds the specified number of days to the value of this instance.
        /// </returns>
        public DATE AddDays(double value) => new DATE(AsDateTime().AddDays(value));

        /// <summary>
        /// Adds the specified number of weeks to the value of the <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="value">The number of weeks to add.</param>
        /// <returns>
        /// A new <see cref="DATE"/> that adds the specified number of weeks to the value of this instance.
        /// </returns>
        public DATE AddWeeks(int value) => new DATE(AsDateTime().AddWeeks(value));

        /// <summary>
        /// Adds the specified number of months to the value of the <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="value">The number of months to add.</param>
        /// <returns>
        /// A new instance of type <see cref="DATE"/> that adds the specified number of months to the
        /// value of this instance.
        /// </returns>
        public DATE AddMonths(int value) => new DATE(AsDateTime().AddMonths(value));

        /// <summary>
        /// Adds the specified number of years to the value of the <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="value">The number of years to add.</param>
        /// <returns>
        /// A new instance of type <see cref="DATE"/> that adds the specified number of years to the
        /// value of this instance.
        /// </returns>
        public DATE AddYears(int value) => new DATE(AsDateTime().AddYears(value));

        /// <summary>
        /// Gets the week day of the <see cref="DATE"/> instance.
        /// </summary>
        /// <returns>The weekday of the</returns>
        public WEEKDAY GetWeekday() => AsDateTime().DayOfWeek.AsWEEKDAY();

        /// <summary>
        /// Converts this date instance to an equivalent <see cref="DateTime"/> instance.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        public DateTime AsDateTime() => this == default(DATE)
            ? default(DateTime)
            : new DateTime((int)FULLYEAR, (int)MONTH, (int)MDAY);

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

        /// <summary>
        /// Converts this date instance explicitly to an equivalent <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is explicitly converted to the <see
        /// cref="DateTime"/> instance.
        /// </param>
        public static explicit operator DateTime(DATE date) => date.AsDateTime();

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

        public static bool operator ==(DATE left, DATE right) => left.Equals(right);

        public static bool operator !=(DATE left, DATE right) => !left.Equals(right);

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

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}";
    }
}
