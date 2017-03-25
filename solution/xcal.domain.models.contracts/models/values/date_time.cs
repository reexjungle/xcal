using reexjungle.xcal.core.domain.contracts.extensions;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NodaTime;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Represents a calendar date with reference to the time of the day.
    /// </summary>
    [DataContract]
    public struct DATE_TIME : IEquatable<DATE_TIME>, IComparable, IComparable<DATE_TIME>
    {
        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013
        /// </summary>
        [DataMember]
        public int FULLYEAR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        [DataMember]
        public int MONTH { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        [DataMember]
        public int MDAY { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        [DataMember]
        public int HOUR { get; private set; }

        /// <summary>
        /// Gets the 2-digit representatio of a minute.
        /// </summary>
        [DataMember]
        public int MINUTE { get; private set; }

        /// <summary>
        /// Gets the 2-digit representation of a second.
        /// </summary>
        [DataMember]
        public int SECOND { get; private set; }

        /// <summary>
        /// Specifies whether a System.DateTime object represents a local time or a Coordinated
        /// Universal Time (UTC).
        /// </summary>
        [DataMember]
        public TIME_FORM Form { get; private set; }

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
        public DATE_TIME(int fullyear, int month, int mday, int hour, int minute, int second, TIME_FORM form = TIME_FORM.LOCAL)
        {
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with an <see
        /// cref="IDATE"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="IDATE"/> instance that initializes this instance.</param>
        public DATE_TIME(DATE other) : this(other.FULLYEAR, other.MONTH, other.MDAY, 0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with an <see
        /// cref="ITIME"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="ITIME"/> instance that initializes this instance.</param>
        public DATE_TIME(TIME other) : this(1, 1, 1, other.HOUR, other.MINUTE, other.SECOND, other.Form)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> structure with a <see
        /// cref="DateTime"/> instance
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> that initializes this instance.</param>
        /// <param name="tzinfo">Any time zone in the world.</param>
        public DATE_TIME(DateTime datetime) : this(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Kind.AsTIME_FORM())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct with the specified
        /// instance of <see cref="DateTimeOffset"/>.
        /// <para/>
        /// Note: By using this constructor, the new instance of <see cref="DATE_TIME"/> shall always
        ///       be initialized as UTC time.
        /// </summary>
        /// <param name="datetime">
        /// A point in time, typically expressed as date and time of day, relative to Coordinate
        /// Universal Time (UTC).
        /// </param>
        public DATE_TIME(DateTimeOffset datetime)
            : this(datetime.UtcDateTime.Year, datetime.UtcDateTime.Month, datetime.UtcDateTime.Day, datetime.UtcDateTime.Hour, datetime.UtcDateTime.Minute, datetime.UtcDateTime.Second, TIME_FORM.UTC)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct with the specified
        /// instance of <see cref="LocalDate"/>.
        /// </summary>
        /// <param name="date">
        /// A date within the calendar, with no reference to a particular time zone or time of day.
        /// </param>
        public DATE_TIME(LocalDate date) : this(date.Year, date.Month, date.Day, 0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct with the specified
        /// instance of <see cref="LocalDateTime"/>.
        /// </summary>
        /// <param name="datetime">A date and time in a particular calendar system</param>
        public DATE_TIME(LocalDateTime datetime) : this(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DATE_TIME"/> struct with the specified
        /// instance of <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="datetime">
        /// A local date time in a specific time zone and with a particular offset to distinguish
        /// between otherwise-ambiguous instants
        /// </param>
        public DATE_TIME(ZonedDateTime datetime) : this(
            datetime.Year, 
            datetime.Month, 
            datetime.Day, 
            datetime.Hour, 
            datetime.Minute, 
            datetime.Second, 
            datetime.Zone.Id.Equals("UTC", StringComparison.OrdinalIgnoreCase) ? TIME_FORM.UTC : TIME_FORM.LOCAL)
        {
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
            var fullyear = 1;
            var month = 1;
            var mday = 1;
            var hour = 0;
            var minute = 0;
            var second = 0;
            var form = TIME_FORM.LOCAL;

            const string pattern = @"^((?<tzid>TZID=(\w+)?/(\w+)):)?(?<year>\d{4,})(?<month>\d{2})(?<day>\d{2})T(?<hour>\d{2})(?<min>\d{2})(?<sec>\d{2})(?<utc>Z)?$";
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
                if (match.Groups["hour"].Success) hour = int.Parse(match.Groups["hour"].Value);
                if (match.Groups["min"].Success) minute = int.Parse(match.Groups["min"].Value);
                if (match.Groups["sec"].Success) second = int.Parse(match.Groups["sec"].Value);
                if (match.Groups["utc"].Success) form = TIME_FORM.UTC;
            }
            FULLYEAR = fullyear;
            MONTH = month;
            MDAY = mday;
            HOUR = hour;
            MINUTE = minute;
            SECOND = second;
            Form = form;
        }

        /// <summary>
        /// Adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole days. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the sum of the date represented by
        /// this instance and the number of days represented by <paramref name="value"/>.
        /// </returns>
        public DATE_TIME AddDays(int value) => AsDateTime().AddDays(value);

        /// <summary>
        /// Adds the specified number of weeks to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole weeks. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the sum of the date time represented
        /// by this instance and the number of weeks represented by <paramref name="value"/>.
        /// </returns>
        public DATE_TIME AddWeeks(int value) => AsDateTime().AddDays(7 * value);

        /// <summary>
        /// Adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole months. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the sum of the date time represented
        /// by this instance and the number of months represented by <paramref name="value"/>.
        /// </returns>
        public DATE_TIME AddMonths(int value) => AsDateTime().AddMonths(value);

        /// <summary>
        /// Adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">
        /// A number of whole years. The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the sum of the date time represented
        /// by this instance and the number of years represented by <paramref name="value"/>.
        /// </returns>
        public DATE_TIME AddYears(int value) => AsDateTime().AddYears(value);

        /// <summary>
        /// Adds the value of the specified <see cref="DURATION"/> instance to the value of this instance.
        /// </summary>
        /// <param name="duration">A positive or negative duration of time.</param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the sum of the date time represented
        /// by this instance and duration of time represented by <paramref name="duration"/>.
        /// </returns>
        public DATE_TIME Add(DURATION duration) => AsDateTime().Add(duration.AsTimeSpan());

        /// <summary>
        /// Subtracts the value of the specified <see cref="DURATION"/> from the value of this instance.
        /// </summary>
        /// <param name="duration">A positive or negative duration of time.</param>
        /// <returns>
        /// A <see cref="DATE_TIME"/> instance, whose value is the date time represented by this
        /// instance minus the duration of time represented by <paramref name="duration"/>.
        /// </returns>
        public DATE_TIME Subtract(DURATION duration) => AsDateTime().Subtract(duration.AsTimeSpan());

        /// <summary>
        /// Subtract the specified date time from this instance.
        /// </summary>
        /// <param name="other">The date to subtract.</param>
        /// <returns>
        /// A duration of time that is equal to the date time represented by this instance minus the
        /// date time represented by <paramref name="other"/>.
        /// </returns>
        public DURATION Subtract(DATE_TIME other) => Period
            .Between(other.AsLocalDateTime(), AsLocalDateTime(), PeriodUnits.Weeks | PeriodUnits.Days| PeriodUnits.Hours | PeriodUnits.Minutes | PeriodUnits.Seconds);

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of seconds
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddSeconds(int value) => AsDateTime().AddSeconds(value);

        /// <summary>
        /// Adds the specified number of minutes to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of minutes
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddMinutes(int value) => AsDateTime().AddMinutes(value);

        /// <summary>
        /// Adds the specified number of hours to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> that adds the specified number of hours
        /// to the value of this instance.
        /// </returns>
        public DATE_TIME AddHours(int value) => AsDateTime().AddHours(value);

        /// <summary>
        /// Returns the latest of two dates.
        /// </summary>
        /// <param name="left">The first date to compare.</param>
        /// <param name="right">The second date to compare.</param>
        /// <returns><paramref name="left"/> if it is later than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static DATE_TIME Max(DATE_TIME left, DATE_TIME right) => left > right ? left : right;

        /// <summary>
        /// Returns the earliest of two dates.
        /// </summary>
        /// <param name="left">The first date to compare. </param>
        /// <param name="right">The second date to compare.</param>
        /// <returns><paramref name="left"/> if it is earlier than <paramref name="right"/>; otherwise <paramref name="right."/>.</returns>
        public static DATE_TIME Min(DATE_TIME left, DATE_TIME right) => left < right ? left : right;

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        public DateTime AsDateTime() => this == default(DATE_TIME)
            ? default(DateTime)
            : new DateTime(FULLYEAR, MONTH, MDAY, HOUR, MINUTE, SECOND, Form.AsDateTimeKind());

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="DateTimeOffset"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="DateTimeOffset"/> respresentation of this date instance.
        /// </returns>
        public DateTimeOffset AsDateTimeOffset() => this == default(DATE_TIME)
            ? default(DateTimeOffset)
            : new DateTimeOffset(FULLYEAR, MONTH, MDAY, HOUR, MINUTE,
                SECOND, TimeSpan.Zero);

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="LocalDate"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="LocalDate"/> respresentation of this date instance.</returns>
        public LocalDate AsLocalDate() => this == default(DATE_TIME)
            ? default(LocalDate)
            : new LocalDate(FULLYEAR, MONTH, MDAY);

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="LocalDateTime"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="LocalDateTime"/> respresentation of this date instance.
        /// </returns>
        public LocalDateTime AsLocalDateTime() => this == default(DATE_TIME)
            ? default(LocalDateTime)
            : new LocalDateTime(FULLYEAR, MONTH, MDAY, HOUR, MINUTE, SECOND);

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="ZonedDateTime"/> representation.
        /// </summary>
        /// <returns>
        /// The equivalent <see cref="ZonedDateTime"/> respresentation of this date instance.
        /// </returns>
        public ZonedDateTime AsZonedDateTime()
        {
            return this == default(DATE_TIME)
                ? default(ZonedDateTime)
                : new ZonedDateTime(new LocalDateTime(FULLYEAR, MONTH, MDAY, HOUR, MINUTE, SECOND),
                    DateTimeZoneProviders.Tzdb["UTC"],
                    Offset.FromHours(0));
        }


        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="DATE"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DATE"/> respresentation of this date instance.</returns>
        public DATE AsDate() => this == default(DATE_TIME)
            ? default(DATE)
            : new DATE(FULLYEAR, MONTH, MDAY);

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="TIME"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="TIME"/> respresentation of this date instance.</returns>
        public TIME AsTime() => this == default(DATE_TIME)
            ? default(TIME)
            : new TIME(HOUR, MINUTE, SECOND, Form);

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
                && Form == other.Form;
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
                var hashCode = FULLYEAR;
                hashCode = (hashCode * 397) ^ MONTH;
                hashCode = (hashCode * 397) ^ MDAY;
                hashCode = (hashCode * 397) ^ HOUR;
                hashCode = (hashCode * 397) ^ MINUTE;
                hashCode = (hashCode * 397) ^ SECOND;
                hashCode = (hashCode * 397) ^ (int)Form;
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
        /// Adds the value of the specified <see cref="DURATION"/> instance to the specified of <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The date time, to which the duration is added.</param>
        /// <param name="duration">The duration of time to add.</param>
        /// <returns>
        /// A date, whose value is the value of <paramref name="datetime"/> plus <paramref name="duration"/>.
        /// </returns>
        public static DATE_TIME operator +(DATE_TIME datetime, DURATION duration) => datetime.Add(duration);

        /// <summary>
        /// Subtracts the value of the specified <see cref="DURATION"/> instance from the specified
        /// of <see cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="left">The date, from which the duration is subtracted.</param>
        /// <param name="duration">The duration of time to subtract.</param>
        /// <returns>
        /// A date, whose value is the value of <paramref name="left"/> minus <paramref name="duration"/>.
        /// </returns>
        public static DATE_TIME operator -(DATE_TIME left, DURATION duration) => left.Subtract(duration);

        /// <summary>
        /// Subtracts a specified <see cref="DATE_TIME"/> instance from another.
        /// </summary>
        /// <param name="left">The first date to subtract from (the minuend).</param>
        /// <param name="right">The second date that is subtracted (the subtrahend).</param>
        /// <returns>
        /// A duration of time, whose value is the value of <paramref name="left"/> minus <paramref name="right"/>.
        /// </returns>
        public static DURATION operator -(DATE_TIME left, DATE_TIME right) => left.Subtract(right);

        /// <summary>
        /// Converts implicitly the specified <see cref="DateTime"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static implicit operator DateTime(DATE_TIME datetime) => datetime.AsDateTime();

        /// <summary>
        /// Converts the <see cref="DateTime"/> implicitly to an equivalent <see cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">
        /// The <see cref="DateTime"/> instance that is implicitly converted to the <see
        /// cref="DATE_TIME"/> instance.
        /// </param>
        public static implicit operator DATE_TIME(DateTime datetime) => new DATE_TIME(datetime);

        /// <summary>
        /// Converts implicitly the specified <see cref="DATE"/> instance to an equivalent <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="DATE"/> instance to convert.</param>
        public static implicit operator DATE_TIME(DATE date) => new DATE_TIME(date);

        /// <summary>
        /// Explicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="DATE"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static explicit operator DATE(DATE_TIME datetime) => datetime.AsDate();

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
        public static explicit operator TIME(DATE_TIME datetime) => datetime.AsTime();

        /// <summary>
        /// Implicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="LocalDate"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert</param>
        public static explicit operator LocalDate(DATE_TIME datetime) => datetime.AsDate();

        /// <summary>
        /// Implicitly converts the specified <see cref="LocalDate"/> instance to a <see
        /// cref="DateTime"/> instance.
        /// </summary>
        /// <param name="date">The <see cref="LocalDate"/> instance to convert.</param>
        public static implicit operator DATE_TIME(LocalDate date) => new DATE_TIME(date);

        /// <summary>
        /// Implicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="LocalDateTime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static implicit operator LocalDateTime(DATE_TIME datetime) => datetime.AsLocalDateTime();

        /// <summary>
        /// Explicitly converts the specified <see cref="LocalDateTime"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static explicit operator DATE_TIME(LocalDateTime datetime) => new DATE_TIME(datetime);

        /// <summary>
        /// Explicitly converts the specified <see cref="DATE_TIME"/> instance to a <see
        /// cref="ZonedDateTime"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="DATE_TIME"/> instance to convert.</param>
        public static explicit operator ZonedDateTime(DATE_TIME datetime) => datetime.AsZonedDateTime();

        /// <summary>
        /// Explicitly converts the specified <see cref="ZonedDateTime"/> instance to a <see
        /// cref="DATE_TIME"/> instance.
        /// </summary>
        /// <param name="datetime">The <see cref="ZonedDateTime"/> instance to convert.</param>
        public static explicit operator DATE_TIME(ZonedDateTime datetime) => new DATE_TIME(datetime);

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
            return Form == TIME_FORM.UTC
                ? $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}Z"
                : $"{FULLYEAR:D4}{MONTH:D2}{MDAY:D2}T{HOUR:D2}{MINUTE:D2}{SECOND:D2}";
        }
    }
}