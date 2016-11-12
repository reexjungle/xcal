namespace reexjungle.xcal.core.domain.contracts.models.values
{

    public class UTC_OFFSET
    {
        /// <summary>
        /// Gets the offset hours from UTC to local time.
        /// </summary>
        public int HOURS { get; }

        /// <summary>
        /// Gets the offset minutes from UTC to local time.
        /// </summary>
        public int MINUTES { get; }

        /// <summary>
        /// Gets the offset seconds from UTC to local time.
        /// </summary>
        public int SECONDS { get; }
    }
}
