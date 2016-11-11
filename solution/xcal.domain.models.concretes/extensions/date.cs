using reexjungle.xcal.core.domain.concretes.models.values;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xmisc.foundation.concretes;
using System;

namespace reexjungle.xcal.core.domain.concretes.extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> value to the date value of type <typeparamref name="TDATE"/>.
        /// </summary>
        /// <typeparam name="TDATE">The type of date that implements <see cref="IDATE"/>.</typeparam>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <param name="func">The conversion function that relays the conversion of the <see cref="DateTime"/> value to the <typeparamref name="TDATE"/> value.</param>
        /// <returns>The <typeparamref name="TDATE"/> value that results from the conversion. </returns>
        public static IDATE AsDATE(this DateTime datetime, Func<DateTime, IDATE> func) => func(datetime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="DATE"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="DATE"/> value that results from the conversion.</returns>
        public static DATE AsDATE(this DateTime datetime) => new DATE(datetime);

        public static DateTime AsDateTime(this IDATE date) => date.Equals(default(IDATE))
            ? default(DateTime)
            : new DateTime((int)date.FULLYEAR, (int)date.MONTH, (int)date.MDAY);

        /// <summary>
        /// Gets the week day of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <returns>The weekday of the </returns>
        public static WEEKDAY GetWeekday(this IDATE date) => date.AsDateTime().DayOfWeek.AsWEEKDAY();

        /// <summary>
        /// Adds the specified number of days to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">The number of days to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of days to the value of this instance.</returns>
        public static IDATE AddDays(this IDATE date, double value, Func<DateTime, IDATE> func) => date.AsDateTime().AddDays(value).AsDATE(func);

        public static DATE AddDays(this IDATE date, double value) => date.AsDateTime().AddDays(value).AsDATE();

        /// <summary>
        /// Adds the specified number of weeks to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">The number of weeks to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of weeks to the value of this instance.</returns>
        public static IDATE AddWeeks(this IDATE date, int value, Func<DateTime, IDATE> func) => date.AsDateTime().AddWeeks(value).AsDATE(func);

        public static DATE AddWeeks(this IDATE date, int value) => date.AsDateTime().AddWeeks(value).AsDATE();

        /// <summary>
        /// Adds the specified number of months to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">The number of months to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of months to the value of this instance.</returns>
        public static IDATE AddMonths(this IDATE date, int value, Func<DateTime, IDATE> func) => date.AsDateTime().AddMonths(value).AsDATE(func);

        public static DATE AddMonths(this IDATE date, int value) => date.AsDateTime().AddMonths(value).AsDATE();

        /// <summary>
        /// Adds the specified number of years to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">The number of years to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of years to the value of this instance.</returns>
        public static IDATE AddYears(this IDATE date, int value, Func<DateTime, IDATE> func) => date.AsDateTime().AddYears(value).AsDATE(func);

        public static DATE AddYears(this IDATE date, int value) => date.AsDateTime().AddYears(value).AsDATE();

        /// <summary>
        /// Adds a duration to the 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IDATE Add(this IDATE date, IDURATION duration, Func<DateTime, IDATE> func) => date.AsDateTime().Add(duration.AsTimeSpan()).AsDATE(func);

        public static DATE Add(this IDATE date, IDURATION duration) => date.AsDateTime().Add(duration.AsTimeSpan()).AsDATE();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IDATE Subtract(this IDATE date, IDURATION duration, Func<DateTime, IDATE> func) => date.AsDateTime().Subtract(duration.AsTimeSpan()).AsDATE(func);

        public static DATE Subtract(this IDATE date, IDURATION duration) => date.AsDateTime().Subtract(duration.AsTimeSpan()).AsDATE();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="other"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IDURATION Subtract(this IDATE date, IDATE other, Func<TimeSpan, IDURATION> func) => date.AsDateTime().Subtract(other.AsDateTime()).AsDURATION(func);

        public static DURATION Subtract(this IDATE date, IDATE other) => date.AsDateTime().Subtract(other.AsDateTime()).AsDURATION();
    }
}
