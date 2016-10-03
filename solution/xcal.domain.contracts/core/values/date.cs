namespace xcal.domain.contracts.core.values
{

    /// <summary>
    /// Specifies the contract for identifying properties that contain a calendar date.
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

    public interface IDATE<out TDATE> where TDATE : IDATE
    {
        TDATE AddDays(double value);

        TDATE AddWeeks(int value);

        TDATE AddMonths(int value);
        TDATE AddYears(int value);

        WEEKDAY GetWeekday();

    }
}
