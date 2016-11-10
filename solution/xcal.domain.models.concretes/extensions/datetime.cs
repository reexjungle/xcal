using reexjungle.xcal.core.domain.concretes.models.values;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System;


namespace reexjungle.xcal.core.domain.concretes.extensions
{
    /// <summary>
    /// Provides features that extend common date and time related features
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="WEEKDAY"/> value to a <see cref="DayOfWeek"/> value.
        /// </summary>
        /// <param name="weekday">The <see cref="WEEKDAY"/> value that is to be converted.</param>
        /// <returns>The <see cref="DayOfWeek"/> value that results from the conversion.</returns>
        public static DayOfWeek AsDayOfWeek(this WEEKDAY weekday)
        {
            switch (weekday)
            {
                case WEEKDAY.SU: return DayOfWeek.Sunday;
                case WEEKDAY.MO: return DayOfWeek.Monday;
                case WEEKDAY.TU: return DayOfWeek.Tuesday;
                case WEEKDAY.WE: return DayOfWeek.Wednesday;
                case WEEKDAY.TH: return DayOfWeek.Thursday;
                case WEEKDAY.FR: return DayOfWeek.Friday;
                case WEEKDAY.SA: return DayOfWeek.Saturday;
                default: return default(DayOfWeek);
            }
        }

        /// <summary>
        /// Converts a <see cref="DayOfWeek"/> value to a <see cref="WEEKDAY"/> value.
        /// </summary>
        /// <param name="dayofweek">The <see cref="DayOfWeek"/> value to be converted.</param>
        /// <returns>The <see cref="WEEKDAY"/> value that results from the conversion.</returns>
        public static WEEKDAY AsWEEKDAY(this DayOfWeek dayofweek)
        {
            switch (dayofweek)
            {
                case DayOfWeek.Sunday: return WEEKDAY.SU;
                case DayOfWeek.Monday: return WEEKDAY.MO;
                case DayOfWeek.Tuesday: return WEEKDAY.TU;
                case DayOfWeek.Wednesday: return WEEKDAY.WE;
                case DayOfWeek.Thursday: return WEEKDAY.TH;
                case DayOfWeek.Friday: return WEEKDAY.FR;
                case DayOfWeek.Saturday: return WEEKDAY.SA;
                default: return default(WEEKDAY);
            }
        }

        /// <summary>
        /// Converts a <see cref="DateTimeKind"/> value of a <see cref="DateTime"/> to a <see cref="TIME_FORM"/> value.
        /// </summary>
        /// <param name="kind">The <see cref="DateTimeKind"/> value to be converted.</param>
        /// <param name="tzinfo">Optional: the time zone reference associated with local <see cref="DateTime"/>.</param>
        /// <returns>The <see cref="TIME_FORM"/> value that results from the conversion.</returns>
        public static TIME_FORM AsTIME_FORM(this DateTimeKind kind, TimeZoneInfo tzinfo = null)
        {
            if (kind == DateTimeKind.Local) return TIME_FORM.LOCAL;
            if (kind == DateTimeKind.Utc) return TIME_FORM.UTC;
            if (kind == DateTimeKind.Unspecified && tzinfo != null)
                return TIME_FORM.LOCAL_TIMEZONE_REF;
            return TIME_FORM.UNSPECIFIED;
        }

        /// <summary>
        /// Converts a <see cref="TIME_FORM"/> value to a <see cref="DateTimeKind"/> value.
        /// </summary>
        /// <param name="form">The form in which the time is expressed.</param>
        /// <param name="tzid">Optional: the time zone identifier.</param>
        /// <returns>The <see cref="DateTimeKind"/> value that results from the conversion.</returns>
        public static DateTimeKind AsDateTimeKind(this TIME_FORM form, ITZID tzid = null)
        {
            if (form == TIME_FORM.LOCAL) return DateTimeKind.Local;
            if (form == TIME_FORM.UTC) return DateTimeKind.Utc;
            if (form == TIME_FORM.LOCAL_TIMEZONE_REF && tzid != null) return DateTimeKind.Local;
            return DateTimeKind.Unspecified;
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to the date value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of date that implements <see cref="IDATE"/>.</typeparam>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <param name="func">The conversion function that relays the conversion of the <see cref="DateTime"/> value to the <typeparamref name="T"/> value.</param>
        /// <returns>The <typeparamref name="T"/> value that results from the conversion. </returns>
        public static T AsDATE<T>(this DateTime datetime, Func<DateTime, T> func)
            where T : IDATE => func(datetime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="DATE"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="DATE"/> value that results from the conversion.</returns>
        public static DATE AsDATE(this DateTime datetime) => datetime.AsDATE(x => new DATE(x));

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to the time value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of time that implements <see cref="ITIME"/>.</typeparam>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <param name="func">The conversion function that relays the conversion of the <see cref="DateTime"/> value to the <typeparamref name="T"/> value.</param>
        /// <returns>The <typeparamref name="T"/> value that results from the conversion.</returns>
        public static T AsTIME<T>(this DateTime datetime, Func<DateTime, T> func)
            where T : ITIME => func(datetime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="TIME"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="TIME"/> value that results from the conversion.</returns>
        public static TIME AsTIME(this DateTime datetime) => datetime.AsTIME(x => new TIME(x));

        /// <summary>
        ///  Converts a <see cref="DateTime"/> value to the date time value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of date time that implements <see cref="IDATE_TIME"/>.</typeparam>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <param name="func">The conversion function that relays the conversion of the <see cref="DateTime"/> value to the <typeparamref name="T"/> value.</param>
        /// <returns>The <typeparamref name="T"/> value that results from the conversion.</returns>
        public static T AsDATE_TIME<T>(this DateTime datetime, Func<DateTime, T> func) where T : IDATE_TIME => func(datetime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="DATE_TIME"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="DATE_TIME"/> value that results from the conversion.</returns>
        public static DATE_TIME AsDATE_TIME(this DateTime datetime) => datetime.AsDATE_TIME(x => new DATE_TIME(x));

        public static T AsDURATION<T>(this TimeSpan timespan, Func<TimeSpan, T> func) where T : IDURATION => func(timespan);

        public static DURATION AsDURATION(this TimeSpan timespan) => timespan.AsDURATION(x => new DURATION(x));
    }
}
