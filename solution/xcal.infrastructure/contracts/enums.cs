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
    public enum CalendarNodeType
    {

        /// <summary>
        /// Represents an unknown component
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// Represents an iCalendar VALUE node e.g. a DATE-TIME node
        /// </summary>
        VALUE,

        /// <summary>
        /// Represents an iCalendar PARAMETER node e.g. a Time Zone ID node
        /// </summary>
        PARAMETER,

        /// <summary>
        /// Represents an icalendar PROPERTY e.g. an Attendee node
        /// </summary>
        PROPERTY,

        /// <summary>
        /// Represents an icalendar COMPONENT e.g. an Event node
        /// </summary>
        COMPONENT
    }

    public enum ReadState
    {
        IDLE,
        READING,
        ERROR,
        EOF,
        CLOSED
    }
}
