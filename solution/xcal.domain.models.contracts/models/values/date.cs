using System;

namespace reexjungle.xcal.core.domain.contracts.models.values
{

    /// <summary>
    /// Specifies a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    public interface IDATE
    {
        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013
        /// </summary>
        uint FULLYEAR { get; }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        uint MONTH { get; }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        uint MDAY { get; }
    }

    /// <summary>
    /// Extends the <see cref="IDATE"/> interface for for a type that implements the <see cref="IDATE"/> interface.
    /// </summary>
    /// <typeparam name="T">The type that implements the <see cref="IDATE"/> interface.</typeparam>
    public interface IDATE<out T, out TDURATION>
        where T : IDATE
        where TDURATION : IDURATION
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
        /// Converts this date instance to an equivalent <see cref="DateTime"/> instance.
        /// </summary>
        /// <returns>The equivalent <see cref="DateTime"/> respresentation of this date instance. </returns>
        DateTime AsDateTime();

        T Add(IDURATION duration);

        T Subtract(IDURATION duration);

        TDURATION Subtract(IDATE end);
    }
}
