namespace xcal.domain.contracts.core.values
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
    /// Extends the <see cref="IDATE"/> contract with methods.
    /// </summary>
    /// <typeparam name="TDATE">The type of <see cref="IDATE"/> that is extended.</typeparam>
    public interface IDATE<out TDATE> where TDATE : IDATE
    {
        /// <summary>
        /// Adds the specified number of days to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="value">The number of days to add.</param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of days to the value of this instance.</returns>
        TDATE AddDays(double value);

        /// <summary>
        /// Adds the specified number of weeks to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="value">The number of weeks to add.</param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of weeks to the value of this instance.</returns>
        TDATE AddWeeks(int value);

        /// <summary>
        /// Adds the specified number of months to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="value">The number of months to add.</param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of months to the value of this instance.</returns>
        TDATE AddMonths(int value);

        /// <summary>
        /// Adds the specified number of years to the value of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <param name="value">The number of years to add.</param>
        /// <returns>A new instance of type <typeparamref name="TDATE"/> that adds the specified number of years to the value of this instance.</returns>
        TDATE AddYears(int value);

        /// <summary>
        /// Gets the week day of the <typeparamref name="TDATE"/> instance.
        /// </summary>
        /// <returns>The weekday of the </returns>
        WEEKDAY GetWeekday();

    }
}
