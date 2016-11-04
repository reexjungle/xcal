using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.parameters
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
