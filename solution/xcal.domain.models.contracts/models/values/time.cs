using reexjungle.xcal.core.domain.contracts.models.parameters;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    /// <summary>
    /// Specifies a point in time, typically expressed as time of day.
    /// <para /> Format 1 (Local Time): [HHMMSS]
    /// <para />Format 2 (UTC Time): [HHMMSS]Z
    /// <para />where HH is 2-digit hour, MM is 2-digit minute, SS is 2-digit second and Z is UTC zone indicator.
    /// </summary>
    public interface ITIME
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour.
        /// </summary>
        uint HOUR { get; }

        /// <summary>
        /// Gets the 2-digit representatio of a minute.
        /// </summary>
        uint MINUTE { get; }

        /// <summary>
        /// Gets the 2-digit representation of a second.
        /// </summary>
        uint SECOND { get; }

        /// <summary>
        /// Gets the form in which the <see cref="ITIME"/> instance is expressed. 
        /// </summary>
        TIME_FORM Form { get; }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        ITZID TimeZoneId { get; }

    }

}
