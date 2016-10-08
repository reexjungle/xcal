using System;

namespace xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifies a calendar user address.
    /// </summary>
    public interface ICAL_ADDRESS
    {
        /// <summary>
        /// Gets the value of the calendar user address.
        /// </summary>
        Uri Value { get; }
    }
}
