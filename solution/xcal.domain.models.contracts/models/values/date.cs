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
}
