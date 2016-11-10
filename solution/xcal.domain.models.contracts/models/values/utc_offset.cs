namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain an offset from UTC to local time.
    /// </summary>
    public interface IUTC_OFFSET
    {
        /// <summary>
        /// Gets the offset hours from UTC to local time.
        /// </summary>
        int HOURS { get; }

        /// <summary>
        /// Gets the offset minutes from UTC to local time.
        /// </summary>
        int MINUTES { get; }

        /// <summary>
        /// Gets the offset seconds from UTC to local time.
        /// </summary>
        int SECONDS { get; }
    }
}
