using reexjungle.xcal.core.domain.contracts.models.components;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models
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
        string ProdId { get; }

        /// <summary>
        /// Gets or sets the identifier corresponding to the highest version number or the minimum and maximum range of the iCalendar
        /// specification that is required in order to interpret the iCalendar object.
        /// This property is REQUIRED.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets or sets the calendar scale used for the calendar information specified in the iCalendar object.
        /// This property is OPTIONAL. Its default value is &quot;GREGORIAN&quot;
        /// </summary>
        CALSCALE Calscale { get; }

        /// <summary>
        /// Gets or sets the calendar method used for the calendar information specified in the iCalendar object.
        /// </summary>
        METHOD Method { get; }

        /// <summary>
        /// Gets or sets the events of the iCalendar core object
        /// </summary>
        List<IEVENT> Events { get; }

        /// <summary>
        /// Gets or sets the to-dos of the iCalendar core object
        /// </summary>
        List<ITODO> ToDos { get; }

        /// <summary>
        /// Gets or sets the free or busy time information groups of the iCalendar core object
        /// </summary>
        List<IFREEBUSY> FreeBusies { get; }

        /// <summary>
        /// Gets or sets the journals of the iCalendar core object
        /// </summary>
        List<IJOURNAL> Journals { get; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        List<ITIMEZONE> TimeZones { get; }
    }
}
