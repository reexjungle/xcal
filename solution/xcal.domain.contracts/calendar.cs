using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexmonkey.xcal.domain.contracts
{
    /// <summary>
    /// Specifies a contract for the iCalendar Core Object and Scheduling
    /// </summary>
    public interface ICALENDAR
    {
        /// <summary>
        /// Gets or sets the identifier for the product that created the iCalendar object.
        /// This property is REQUIRED. This identifier should be guaranteed to be a globally unique identifier (GUID)
        /// </summary>
        string ProdId { get; set; }

        /// <summary>
        /// Gets or sets the identifier corresponding to the highest version number or the minimum and maximum range of the iCalendar
        /// specification that is required in order to interpret the iCalendar object.
        /// This property is REQUIRED.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Gets or sets the calendar scale used for the calendar information specified in the iCalendar object.
        /// This property is OPTIONAL. Its default value is &quot;GREGORIAN&quot;
        /// </summary>
        CALSCALE Calscale { get; set; }


        /// <summary>
        /// Gets or sets the calendar method used for the calendar information specified in the iCalendar object.
        /// </summary>
        METHOD Method { get; set; }

        /// <summary>
        /// Gets or sets the components of the iCalendar core object
        /// </summary>
        List<ICOMPONENT> Components { get; set; }

    }





}
