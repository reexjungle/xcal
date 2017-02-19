using System;
using NodaTime;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.extensions
{
    /// <summary>
    /// Provides features that extend date and time functionalities.
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


        public static TIME_FORM AsTIME_FORM(this DateTimeKind kind) 
            => kind == DateTimeKind.Utc ? TIME_FORM.UTC : TIME_FORM.LOCAL;

        /// <summary>
        /// Converts a <see cref="TIME_FORM"/> value to a <see cref="DateTimeKind"/> value.
        /// </summary>
        /// <param name="form">The form in which the time is expressed.</param>
        /// <returns>The <see cref="DateTimeKind"/> value that results from the conversion.</returns>
        public static DateTimeKind AsDateTimeKind(this TIME_FORM form) 
            => form == TIME_FORM.UTC ? DateTimeKind.Utc : DateTimeKind.Local;

        /// <summary>
        /// Converts a <see cref="WEEKDAY"/> value to a NodaTime "DayOfWeek" value.
        /// </summary>
        /// <param name="weekday">The <see cref="WEEKDAY"/> value that is to be converted.</param>
        /// <returns>The NodaTime "DayOfWeek" value that results from the conversion.</returns>
        public static int AsNodaTimeDayOfWeek(this WEEKDAY weekday)
        {
            switch (weekday)
            {
                case WEEKDAY.SU: return 7;
                case WEEKDAY.MO: return 1;
                case WEEKDAY.TU: return 2;
                case WEEKDAY.WE: return 3;
                case WEEKDAY.TH: return 4;
                case WEEKDAY.FR: return 5;
                case WEEKDAY.SA: return 6;
                default: return 7; //SU
            }
        }

        /// <summary>
        /// Converts a NodaTime "DayOfWeek" value to a <see cref="WEEKDAY"/> value.
        /// </summary>
        /// <param name="dayofweek">The NodaTime "DayOfWeek" value to be converted.</param>
        /// <returns>The <see cref="WEEKDAY"/> value that results from the conversion.</returns>
        public static WEEKDAY AsWEEKDAY(this int dayofweek)
        {
            switch (dayofweek)
            {
                case 7: return WEEKDAY.SU;
                case 1: return WEEKDAY.MO;
                case 2: return WEEKDAY.TU;
                case 3: return WEEKDAY.WE;
                case 4: return WEEKDAY.TH;
                case 5: return WEEKDAY.FR;
                case 6: return WEEKDAY.SA;
                default: return default(WEEKDAY);
            }
        }
    }
}
