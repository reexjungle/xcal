using System.Collections.Generic;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.parameters
{
    /// <summary>
    /// Group or List Membership.
    /// Specifies the group or list membership of the calendar user specified by a property
    /// </summary>
    public interface IMEMBER
    {
        /// <summary>
        /// Gets the calendar addresses of the group members
        /// </summary>
        List<ICAL_ADDRESS> Addresses { get; }
    }
}
