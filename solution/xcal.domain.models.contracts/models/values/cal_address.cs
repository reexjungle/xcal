using System;

namespace reexjungle.xcal.core.domain.contracts.models.values
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

        /// <summary>
        /// Converts this calendar address into an equivalent <see cref="Uri"/> instance.
        /// </summary>
        /// <returns>The equivalent <see cref="Uri"/> respresentation of this calendar address instance.</returns>
        Uri AsUri();
    }

}
