using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
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


        public static TIME_FORM AsTIME_FORM(this DateTimeKind kind, TimeZoneInfo tzinfo = null)
        {
            if(kind == DateTimeKind.Local) return TIME_FORM.LOCAL;
            if(kind == DateTimeKind.Utc) return TIME_FORM.UTC;
            if(kind == DateTimeKind.Unspecified && tzinfo != null)
                return TIME_FORM.LOCAL_TIMEZONE_REF;
            return TIME_FORM.UNSPECIFIED;
        }


        public static DateTimeKind AsDateTimeKind(this TIME_FORM form, ITZID tzid = null)
        {
            if (form == TIME_FORM.LOCAL) return DateTimeKind.Local;
            if (form == TIME_FORM.UTC) return DateTimeKind.Utc;
            if (form == TIME_FORM.LOCAL_TIMEZONE_REF && tzid != null) return DateTimeKind.Local;
            return DateTimeKind.Unspecified;
        }

    }
}
