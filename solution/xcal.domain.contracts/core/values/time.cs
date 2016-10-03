using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifies the contract for identifying properties that contain a time of the day
    /// Format 1 (Local Time): [HHMMSS]
    /// Format 2 (UTC Time): [HHMMSS]&quot;Z&quot;
    /// where HH is 2-digit hour, MM is 2-digit minute, SS is 2-digit second and Z is UTC zone indicator
    /// </summary>
    public interface ITIME
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour
        /// </summary>
        uint HOUR { get; }

        /// <summary>
        /// Gets or sets the 2-digit representatio of a minute
        /// </summary>
        uint MINUTE { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second
        /// </summary>
        uint SECOND { get; }

        TimeType Type { get; }

        ITZID TimeZoneId { get; }

    }

    public interface ITIME<out TTIME> where TTIME : ITIME
    {
        TTIME AddSeconds(double value);
        TTIME AddMinutes(double value);
        TTIME AddHours(double value);
    }
}
