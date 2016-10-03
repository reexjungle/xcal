namespace xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain a duration of time
    /// Format: &quot;+&quot;/&quot;-&quot;&quot;P&quot;[days &quot;D &quot;/[&quot;T&quot; [hours &quot;H&quot; / minutes &quot;M&quot; /seconds &quot;S&quot;] weeks &quot;W&quot;]]
    /// </summary>
    public interface IDURATION
    {
        /// <summary>
        /// Gets the duration in weeks
        /// </summary>
        int WEEKS { get; }

        /// <summary>
        /// Gets the duration in hours
        /// </summary>
        int HOURS { get; }

        /// <summary>
        /// Gets the duration in minutes
        /// </summary>
        int MINUTES { get; }

        /// <summary>
        /// Gets the duration in seconds
        /// </summary>
        int SECONDS { get; }

        /// <summary>
        /// Gets the duration in days
        /// </summary>
        int DAYS { get; }
    }
}
