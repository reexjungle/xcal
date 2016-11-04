namespace reexjungle.xcal.core.domain.contracts.models.parameters
{

    /// <summary>
    /// Time Zone Identifier.
    /// Specifies the identifier for the time zone definition for a time component in a property value.
    /// </summary>
    /// <remarks>
    /// This paramter MUST be specified on the DATE_TIME, DATE_TIME, DUE, EXDATE and RDATE propertiws when either a DATE-TIME
    /// or TIME value type is specified and when the value is neither a UTC or &quot; floating time &quot; time.
    /// </remarks>
    public interface ITZID
    {
        /// <summary>
        /// Specifies whether the time-zone prefix should be included
        /// </summary>
        bool GloballyUnique { get; }

        /// <summary>
        /// Gets  the identifier for the time zone definition for a time component
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets  the identifier for the time zone definition for a time component
        /// </summary>
        string Suffix { get; }
    }
}
