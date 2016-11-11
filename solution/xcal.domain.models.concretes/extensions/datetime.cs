using reexjungle.xcal.core.domain.concretes.models.values;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using reexjungle.xmisc.foundation.concretes;
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
        /// Converts a <see cref="DateTimeKind"/> value of a <see cref="DateTime"/> to a <see
        /// cref="TIME_FORM"/> value.
        /// </summary>
        /// <param name="kind">The <see cref="DateTimeKind"/> value to be converted.</param>
        /// <param name="tzinfo">
        /// Optional: the time zone reference associated with local <see cref="DateTime"/>.
        /// </param>
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


        public static IDATE_TIME AsDATE_TIME(this DateTime datetime, Func<DateTime, IDATE_TIME> func) => func(datetime);

        public static DATE_TIME AsDATE_TIME(this DateTime datetime) => new DATE_TIME(datetime);

        public static IDATE_TIME AddDays(this IDATE_TIME date, double value, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().AddDays(value).AsDATE_TIME(func);

        public static DATE_TIME AddDays(this IDATE_TIME date, double value) => date.AsDateTime().AddDays(value).AsDATE_TIME();

        public static IDATE_TIME AddWeeks(this IDATE_TIME date, int value, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().AddWeeks(value).AsDATE_TIME(func);

        public static DATE_TIME AddWeeks(this IDATE_TIME date, int value) => date.AsDateTime().AddWeeks(value).AsDATE_TIME();

        public static IDATE_TIME AddMonths(this IDATE_TIME date, int value, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().AddMonths(value).AsDATE_TIME(func);

        public static DATE_TIME AddMonths(this IDATE_TIME date, int value) => date.AsDateTime().AddMonths(value).AsDATE_TIME();

        public static IDATE_TIME AddYears(this IDATE_TIME date, int value, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().AddYears(value).AsDATE_TIME(func);

        public static DATE_TIME AddYears(this IDATE_TIME date, int value) => date.AsDateTime().AddYears(value).AsDATE_TIME();

        public static IDATE_TIME Add(this IDATE_TIME date, IDURATION duration, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().Add(duration.AsTimeSpan()).AsDATE_TIME(func);

        public static DATE_TIME Add(this IDATE_TIME date, IDURATION duration) => date.AsDateTime().Add(duration.AsTimeSpan()).AsDATE_TIME();

        public static IDATE_TIME Subtract(this IDATE_TIME date, IDURATION duration, Func<DateTime, IDATE_TIME> func) => date.AsDateTime().Subtract(duration.AsTimeSpan()).AsDATE_TIME(func);

        public static DATE_TIME Subtract(this IDATE_TIME date, IDURATION duration) => date.AsDateTime().Subtract(duration.AsTimeSpan()).AsDATE_TIME();

        public static IDURATION Subtract(this IDATE_TIME date, IDATE_TIME other, Func<TimeSpan, IDURATION> func) => date.AsDateTime().Subtract(other.AsDateTime()).AsDURATION(func);

        public static DURATION Subtract(this IDATE_TIME date, IDATE_TIME other) => date.AsDateTime().Subtract(other.AsDateTime()).AsDURATION();
    }
}
