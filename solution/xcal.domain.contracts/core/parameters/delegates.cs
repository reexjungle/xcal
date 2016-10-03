using System.Collections.Generic;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.parameters
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
        List<ICAL_ADDRESS> Addresses { get; }
    }

    public interface IDELEGATE_FROM : IDELEGATE
    {

    }

    public interface IDELEGATE_TO : IDELEGATE
    {

    }

}
