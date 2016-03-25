using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.infrastructure.contracts
{
    /// <summary>
    /// Specifies the type of icalendar fragments.
    /// </summary>
    public enum CalendarFragmentType
    {
        /// <summary>
        /// Represents an iCalendar VALUE fragment e.g. a DATE-TIME fragment
        /// </summary>
        VALUE,

        /// <summary>
        /// Represents an iCalendar PARAMETER fragment e.g. a Time Zone ID fragment
        /// </summary>
        PARAMETER,

        /// <summary>
        /// Represents an icalendar PROPERTY e.g. an Attendee fragment
        /// </summary>
        PROPERTY,

        /// <summary>
        /// Represents an icalendar COMPONENT e.g. an Event fragment
        /// </summary>
        COMPONENT,

        /// <summary>
        /// Represents an unknown component
        /// </summary>
        UNKNOWN
    }
}
