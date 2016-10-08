namespace xcal.domain.contracts.core.properties
{
    /// <summary>
    /// Specifies an interface for a return status code
    /// </summary>
    public interface ISTATCODE
    {
        /// <summary>
        /// First level of granularity
        /// </summary>
        uint L1 { get; }

        /// <summary>
        /// Second level of granularity
        /// </summary>
        uint L2 { get; }

        /// <summary>
        /// Third level of granularity
        /// </summary>
        uint? L3 { get; }
    }
}
