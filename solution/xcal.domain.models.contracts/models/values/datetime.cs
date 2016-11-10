using reexjungle.xcal.core.domain.contracts.models.parameters;
using System;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDATE_TIME : IDATE, ITIME
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDATE_TIME<out T> where T : IDATE_TIME
    {
        /// <summary>
        /// Adds the specified number of days to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of days to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of days to the value of this instance.</returns>
        T AddDays(double value);

        /// <summary>
        /// Adds the specified number of weeks to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of weeks to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of weeks to the value of this instance.</returns>
        T AddWeeks(int value);

        /// <summary>
        /// Adds the specified number of months to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of months to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of months to the value of this instance.</returns>
        T AddMonths(int value);

        /// <summary>
        /// Adds the specified number of years to the value of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <param name="value">The number of years to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of years to the value of this instance.</returns>
        T AddYears(int value);

        /// <summary>
        /// Gets the week day of the <typeparamref name="T"/> instance.
        /// </summary>
        /// <returns>The weekday of the </returns>
        WEEKDAY GetWeekday();

        /// <summary>
        /// Adds the specified number of seconds to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of seconds to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of seconds to the value of this instance.</returns>
        T AddSeconds(double value);

        /// <summary>
        /// Adds the specified number of minutes to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of mintes to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of minutes to the value of this instance.</returns>
        T AddMinutes(double value);

        /// <summary>
        /// Adds the specified number of hours to the value of the <typeparamref name="T"/> instance. 
        /// </summary>
        /// <param name="value">The number of hours to add.</param>
        /// <returns>A new instance of type <typeparamref name="T"/> that adds the specified number of hours to the value of this instance.</returns>
        T AddHours(double value);

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance.</returns>
        DateTime AsDateTime();

        /// <summary>
        /// Converts this date time instance to its equivalent <see cref="DateTimeOffset"/> representation.
        /// </summary>
        /// <param name="func">Function to determine the offset from the time zone reference.</param>
        /// <returns>The equivalent <see cref="DateTimeOffset"/> respresentation of this date instance.</returns>
        DateTimeOffset AsDateTimeOffset(Func<ITZID, IUTC_OFFSET> func = null);
    }
}
