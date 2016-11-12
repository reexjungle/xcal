using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.parameters
{
    /// <summary>
    /// Delegators
    /// Specfies the calendar users that have delegated their participation to the calendar user specfied by the property.
    /// </summary>
    public interface IDELEGATE
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        List<CAL_ADDRESS> Addresses { get; }
    }

    public interface IDELEGATE_FROM : IDELEGATE
    {

    }

    public interface IDELEGATE_TO : IDELEGATE
    {

    }

}
