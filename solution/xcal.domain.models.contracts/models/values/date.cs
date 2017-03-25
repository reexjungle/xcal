using NodaTime;
using reexjungle.xcal.core.domain.contracts.extensions;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a calendar date without reference to the time of the day.
    /// </summary>
    [DataContract]
    public struct DATE : IEquatable<DATE>, IComparable, IComparable<DATE>
    {
        /// <summary>
        /// Gets the digit representation of a full year e.g. 2013
        /// </summary>
        [DataMember]
        public int FULLYEAR { get; private set; }

        /// <summary>
        /// Gets the digit representation of a month
        /// </summary>
        [DataMember]
        public int MONTH { get; private set; }

        /// <summary>
        /// Gets the digit representation of a day in a month
        /// </summary>
        [DataMember]
        public int MDAY { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with the year, month and day.
        /// </summary>
        /// <param name="fullyear">The 4-digit representation of a full year e.g. 2013.</param>
        /// <param name="month">The 2-digit representation of a month.</param>
        /// <param name="mday">The 2-digit representation of a day of the month.</param>
        public DATE(int fullyear, int month, int mday)
        {
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with an <see cref="DATE"/> instance
        /// </summary>
        /// <param name="other">The <see cref="DATE"/> instance that initializes this instance.</param>
        public DATE(DATE other) : this(other.FULLYEAR, other.MONTH, other.MDAY)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with a <see
        /// cref="DateTime"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> that initializes this instance.</param>
        public DATE(DateTime date) : this(date.Year, date.Month, date.Day)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with a <see
        /// cref="LocalDate"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="LocalDate"/> that initializes this instance.</param>
        public DATE(LocalDate date) : this(date.Year, date.Month, date.Day)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with a <see
        /// cref="LocalDateTime"/> instance
        /// </summary>
        /// <param name="date">The <see cref="LocalDateTime"/> that initializes this instance.</param>
        public DATE(LocalDateTime date) : this(date.Year, date.Month, date.Day)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE"/> structure with a <see
        /// cref="ZonedDateTime"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="ZonedDateTime"/> that initializes this instance.</param>
        public DATE(ZonedDateTime date) : this(date.Year, date.Month, date.Day)
        {
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
            var fullyear = 1;
            var month = 1;
            var mday = 1;
            const string pattern = @"^(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})$";
            const RegexOptions options = RegexOptions.IgnoreCase
                                         | RegexOptions.CultureInvariant
                                         | RegexOptions.ExplicitCapture
                                         | RegexOptions.Compiled;

            var regex = new Regex(pattern, options);
            foreach (Match match in regex.Matches(value))
            {
                if (match.Groups["year"].Success) fullyear = int.Parse(match.Groups["year"].Value);
                if (match.Groups["month"].Success) month = int.Parse(match.Groups["month"].Value);
                if (match.Groups["day"].Success) mday = int.Parse(match.Groups["day"].Value);
            }

            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
        }

        /// <summary>
        /// Converts this date instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="DateTime"/> respresentation of this date time instance.
        /// </returns>
        public DateTime AsDateTime() => this == default(DATE)
            ? default(DateTime)
            : new DateTime(FULLYEAR, MONTH, MDAY);

        /// <summary>
        /// Converts this date instance to its equivalent <see cref="LocalDate"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="LocalDate"/> respresentation of this date time instance.
        /// </returns>
        public LocalDate AsLocalDate() => this == default(DATE)
            ? default(LocalDate)
            : new LocalDate(FULLYEAR, MONTH, MDAY);

        /// <summary>
        /// Converts this date instance to its equivalent <see cref="LocalDateTime"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="LocalDateTime"/> respresentation of this date time instance.
        /// </returns>
        public LocalDateTime AsLocalDateTime() => this == default(DATE)
            ? default(LocalDateTime)
            : new LocalDateTime(FULLYEAR, MONTH, MDAY, 0, 0, 0);

        /// <summary>
        /// Converts this date instance to its equivalent <see cref="ZonedDateTime"/> representation.
        /// <para/>
        /// The <see cref="ZonedDateTime"/> representation is initialized to the "UTC" time zone.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="ZonedDateTime"/> respresentation of this date time instance.
        /// </returns>
        public ZonedDateTime AsZonedDateTime() => this == default(DATE)
            ? default(ZonedDateTime)
            : new ZonedDateTime(new LocalDateTime(FULLYEAR, MONTH, MDAY, 0, 0, 0),
                DateTimeZoneProviders.Tzdb["UTC"],
                Offset.FromHours(0));

        /// <summary>
        /// Gets the week day of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <returns>The weekday of the</returns>
        public WEEKDAY GetWeekday() => AsLocalDate().DayOfWeek.AsWEEKDAY();

        /// <summary>
        /// Adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole days. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the sum of the date represented by this
        /// instance and the number of days represented by <paramref name="value"/>.
        /// </returns>
        public DATE AddDays(int value) => (DATE)AsLocalDate().PlusDays(value);

        /// <summary>
        /// Adds the specified number of weeks to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole weeks. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the sum of the date represented by this
        /// instance and the number of weeks represented by <paramref name="value"/>.
        /// </returns>
        public DATE AddWeeks(int value) => (DATE)AsLocalDate().PlusWeeks(value);

        /// <summary>
        /// Adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole months. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the sum of the date represented by this
        /// instance and the number of months represented by <paramref name="value"/>.
        /// </returns>
        public DATE AddMonths(int value) => (DATE)AsLocalDate().PlusMonths(value);

        /// <summary>
        /// Adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole years. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the sum of the date represented by this
        /// instance and the number of years represented by <paramref name="value"/>.
        /// </returns>
        public DATE AddYears(int value) => (DATE)AsLocalDate().PlusYears(value);

        /// <summary>
        /// Adds the value of the specified <see cref="DURATION"/> instance to the value of this instance.
        /// </summary>
        /// <param name="duration">A positive or negative duration of time.</param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the sum of the date represented by this
        /// instance and duration of time represented by <paramref name="duration"/>.
        /// </returns>
        public DATE Add(DURATION duration) => (DATE)AsDateTime().Add(duration.AsTimeSpan());

        /// <summary>
        /// Subtracts the value of the specified <see cref="DURATION"/> from the value of this instance.
        /// </summary>
        /// <param name="duration">A positive or negative duration of time.</param>
        /// <returns>
        /// A <see cref="DATE"/> instance, whose value is the date represented by this instance minus
        /// the duration of time represented by <paramref name="duration"/>.
        /// </returns>
        public DATE Subtract(DURATION duration) => (DATE)AsDateTime().Subtract(duration.AsTimeSpan());

        /// <summary>
        /// Subtract the specified date from this instance.
        /// </summary>
        /// <param name="other">The date to subtract.</param>
        /// <returns>
        /// A duration of time that is equal to the date represented by this instance minus the date
        /// represented by <paramref name="other"/>.
        /// </returns>
        public DURATION Subtract(DATE other) => Period.Between(other.AsLocalDate(), AsLocalDate(),  PeriodUnits.Weeks | PeriodUnits.Days );

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
                var hashCode = FULLYEAR;
                hashCode = (hashCode * 397) ^ MONTH;
                hashCode = (hashCode * 397) ^ MDAY;
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
            throw new ArgumentException(nameof(obj) + " is not of type " + nameof(DATE));
        }

        /// <summary>
        /// Adds the value of the specified <see cref="DURATION"/> instance to the specified of <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="date">The date, to which the duration is added.</param>
        /// <param name="duration">The duration of time to add.</param>
        /// <returns>
        /// A date, whose value is the value of <paramref name="date"/> plus <paramref name="duration"/>.
        /// </returns>
        public static DATE operator +(DATE date, DURATION duration) => date.Add(duration);

        /// <summary>
        /// Subtracts the value of the specified <see cref="DURATION"/> instance from the specified
        /// of <see cref="DATE"/> instance.
        /// </summary>
        /// <param name="date">The date, from which the duration is subtracted.</param>
        /// <param name="duration">The duration of time to subtract.</param>
        /// <returns>
        /// A date, whose value is the value of <paramref name="date"/> minus <paramref name="duration"/>.
        /// </returns>
        public static DATE operator -(DATE date, DURATION duration) => date.Subtract(duration);

        /// <summary>
        /// Subtracts a specified <see cref="DATE"/> instance from another.
        /// </summary>
        /// <param name="left">The first date to subtract from (the minuend).</param>
        /// <param name="right">The second date that is subtracted (the subtrahend).</param>
        /// <returns>
        /// A duration of time, whose value is the value of <paramref name="left"/> minus <paramref name="right"/>.
        /// </returns>
        public static DURATION operator -(DATE left, DATE right) => left.Subtract(right);

        /// <summary>
        /// Converts the <see cref="DATE"/> instance implicitly to an equivalent <see
        /// cref="LocalDate"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is implicitly converted to the <see
        /// cref="LocalDate"/> instance.
        /// </param>
        public static implicit operator LocalDate(DATE date) => date.AsLocalDate();

        /// <summary>
        /// Converts the <see cref="LocalDate"/> instance explicitly to an equivalent <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is explicity converted to the <see cref="DATE"/> instance.
        /// </param>
        public static explicit operator DATE(LocalDate date) => new DATE(date);

        /// <summary>
        /// Converts the <see cref="DATE"/> implicitly to an equivalent <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is implicitly converted to the <see
        /// cref="DateTime"/> instance.
        /// </param>
        public static implicit operator DateTime(DATE date) => date.AsDateTime();

        /// <summary>
        /// Converts the <see cref="DATE_TIME"/> instance explicitly to an equivalent <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="datetime">
        /// The <see cref="DateTime"/> instance that is explicitly converted to the <see
        /// cref="DATE"/> instance.
        /// </param>
        public static explicit operator DATE(DateTime datetime) => new DATE(datetime);

        /// <summary>
        /// Converts the <see cref="DATE"/> implicitly to an equivalent <see cref="LocalDateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is implicitly converted to the <see
        /// cref="LocalDateTime"/> instance.
        /// </param>
        public static implicit operator LocalDateTime(DATE date) => date.AsLocalDateTime();

        /// <summary>
        /// Converts the <see cref="LocalDateTime"/> instance explicitly to an equivalent <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="LocalDateTime"/> instance that is explicitly converted to the <see
        /// cref="DATE"/> instance.
        /// </param>
        public static explicit operator DATE(LocalDateTime date) => new DATE(date);

        /// <summary>
        /// Converts the <see cref="DATE"/> explicitly to an equivalent <see cref="ZonedDateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is explicitly converted to the <see
        /// cref="ZonedDateTime"/> instance.
        /// </param>
        public static implicit operator ZonedDateTime(DATE date) => date.AsZonedDateTime();

        /// <summary>
        /// Converts the <see cref="DATE"/> explicitly to an equivalent <see cref="ZonedDateTime"/> instance.
        /// </summary>
        /// <param name="date">
        /// The <see cref="DATE"/> instance that is explicitly converted to the <see
        /// cref="ZonedDateTime"/> instance.
        /// </param>
        public static explicit operator DATE(ZonedDateTime date) => new DATE(date);

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is earlier than another specified
        /// <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator <(DATE left, DATE right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is later than another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise false.
        /// </returns>
        public static bool operator >(DATE left, DATE right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is earlier than or the same as
        /// another specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is earlier than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator <=(DATE left, DATE right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="DATE"/> is later than or the same as another
        /// specified <see cref="DATE"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> is later than or the same as <paramref name="right"/>;
        /// otherwise false.
        /// </returns>
        public static bool operator >=(DATE left, DATE right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE"/> are equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> represent the same date
        /// instance; otherwise false.
        /// </returns>
        public static bool operator ==(DATE left, DATE right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified instances of <see cref="DATE"/> are not equal.
        /// </summary>
        /// <param name="left">This first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// true if <paramref name="left"/> and <paramref name="right"/> do not represent the same
        /// date instance; otherwise false.
        /// </returns>
        public static bool operator !=(DATE left, DATE right) => !left.Equals(right);

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> containing a fully qualified type name.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}";
    }
}