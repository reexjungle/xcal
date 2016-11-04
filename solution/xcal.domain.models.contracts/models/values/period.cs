namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Specifies a contract for identifying properties that contain a precise period of time
    /// </summary>
    public interface IPERIOD
    {
        /// <summary>
        /// Gets or sets the start of the period
        /// </summary>
        IDATE_TIME Start { get; }
    }

    public interface IPERIOD_EXPLICIT : IPERIOD
    {
        /// <summary>
        /// Gets or sets the end of the period.
        /// </summary>
        IDATE_TIME End { get; }
    }

    public interface IPERIOD_START : IPERIOD
    {
        /// <summary>
        /// Gets or sets the duration of the period.
        /// </summary>
        IDURATION Duration { get; }
    }
}
