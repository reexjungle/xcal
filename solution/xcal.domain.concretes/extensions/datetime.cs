using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core;

namespace xcal.domain.concretes.extensions
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
                case WEEKDAY.NONE: return DayOfWeek.Sunday;
                default: return DayOfWeek.Sunday;
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
                default: return WEEKDAY.NONE;
            }
        }
    }
}
