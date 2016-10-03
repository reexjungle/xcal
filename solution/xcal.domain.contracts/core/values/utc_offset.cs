namespace xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain an offset from UTC to local time
    /// </summary>
    public interface IUTC_OFFSET
    {
        /// <summary>
        /// Gets or sets the 2-digit representation of an hour
        /// </summary>
        int HOUR { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a minute
        /// </summary>
        int MINUTE { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second
        /// </summary>
        int SECOND { get; }
    }
}
