using reexjungle.xcal.core.domain.concretes.models.values;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;
using System;

namespace reexjungle.xcal.core.domain.concretes.extensions
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> value to the time value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of time that implements <see cref="ITIME"/>.</typeparam>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <param name="func">The conversion function that relays the conversion of the <see cref="DateTime"/> value to the <typeparamref name="T"/> value.</param>
        /// <returns>The <typeparamref name="T"/> value that results from the conversion.</returns>
        public static ITIME AsTIME(this DateTime datetime, Func<DateTime, ITIME> func) => func(datetime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="TIME"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="TIME"/> value that results from the conversion.</returns>
        public static TIME AsTIME(this DateTime datetime) => new TIME(datetime);

        public static ITIME AsTIME(this DateTimeOffset datetime, Func<DateTimeOffset, ITIME> func) => func(datetime);

        public static TIME AsTIME(this DateTimeOffset datetime) => new TIME(datetime);

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        public static DateTime AsDateTime(this ITIME time) => time.Equals(default(ITIME))
            ? default(DateTime)
            : new DateTime(1, 1, 1, (int)time.HOUR, (int)time.MINUTE, (int)time.SECOND, time.Form.AsDateTimeKind(time.TimeZoneId));

        /// <summary>
        /// Converts this time instance to its equivalent <see cref="DateTimeOffset"/> representation.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="func">Function to determine the offset from the time zone reference.</param>
        /// <returns>The equivalent <see cref="DateTimeOffset"/> respresentation of this date instance.</returns>
        public static DateTimeOffset AsDateTimeOffset(this ITIME time, Func<ITZID, IUTC_OFFSET> func = null)
        {
            if (time.Equals(default(TIME))) return default(DateTimeOffset);

            if (time.Form == TIME_FORM.LOCAL || time.Form == TIME_FORM.UTC)
                return new DateTimeOffset(time.AsDateTime(), TimeSpan.Zero);

            if (time.Form == TIME_FORM.LOCAL_TIMEZONE_REF && time.TimeZoneId != null && func != null)
            {
                var offset = func(time.TimeZoneId);
                return new DateTimeOffset(time.AsDateTime(), new TimeSpan(offset.HOURS, offset.MINUTES, offset.SECONDS));
            }
            //Unspecified time form
            return new DateTimeOffset(time.AsDateTime());
        }


        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value">The number of seconds to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of seconds to the value of this instance.</returns>
        public static ITIME AddSeconds(this ITIME time, double value, Func<DateTime, ITIME> func) => time.AsDateTime().AddSeconds(value).AsTIME(func);

        public static TIME AddSeconds(this ITIME time, double value) => time.AsDateTime().AddSeconds(value).AsTIME();

        /// <summary>
        /// Adds the specified number of minutes to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value">The number of mintes to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of minutes to the value of this instance.</returns>
        public static ITIME AddMinutes(this ITIME time, double value, Func<DateTime, ITIME> func) => time.AsDateTime().AddMinutes(value).AsTIME(func);

        public static TIME AddMinutes(this ITIME time, double value) => time.AsDateTime().AddMinutes(value).AsTIME();

        /// <summary>
        /// Adds the specified number of hours to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value">The number of hours to add.</param>
        /// <param name="func"></param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of hours to the value of this instance.</returns>
        public static ITIME AddHours(this ITIME time, double value, Func<DateTime, ITIME> func) => time.AsDateTime().AddHours(value).AsTIME(func);

        public static TIME AddHours(this ITIME time, double value) => time.AsDateTime().AddHours(value).AsTIME();


        public static ITIME Add(this ITIME time, IDURATION duration, Func<DateTime, ITIME> func) => time.AsDateTime().Add(duration.AsTimeSpan()).AsTIME(func);

        public static TIME Add(this ITIME time, IDURATION duration) => time.AsDateTime().Add(duration.AsTimeSpan()).AsTIME();

        public static ITIME Subtract(this ITIME time, IDURATION duration, Func<DateTime, ITIME> func) => time.AsDateTime().Subtract(duration.AsTimeSpan()).AsTIME(func);

        public static TIME Subtract(this ITIME time, IDURATION duration) => time.AsDateTime().Subtract(duration.AsTimeSpan()).AsTIME();

        public static IDURATION Subtract(this ITIME time, ITIME other, Func<TimeSpan, IDURATION> func) => other.AsDateTime().Subtract(time.AsDateTime()).AsDURATION(func);

        public static DURATION Subtract(this ITIME time, ITIME other) => time.AsDateTime().Subtract(other.AsDateTime()).AsDURATION();
    }
}
