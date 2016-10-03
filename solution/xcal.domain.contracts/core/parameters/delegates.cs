using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<ICAL_ADDRESS> Addresses { get; set; }
    }
}
