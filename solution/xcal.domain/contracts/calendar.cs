using reexjungle.xcal.domain.contracts;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.models
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
        /// Gets or sets the events of the iCalendar core object
        /// </summary>
        List<VEVENT> Events { get; set; }

        /// <summary>
        /// Gets or sets the to-dos of the iCalendar core object
        /// </summary>
        List<VTODO> ToDos { get; set; }

        /// <summary>
        /// Gets or sets the free or busy time information groups of the iCalendar core object
        /// </summary>
        List<VFREEBUSY> FreeBusies { get; set; }

        /// <summary>
        /// Gets or sets the journals of the iCalendar core object
        /// </summary>
        List<VJOURNAL> Journals { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        List<VTIMEZONE> TimeZones { get; set; }

        /// <summary>
        /// Gets or sets the timezones of the iCalendar core object
        /// </summary>
        List<IANA_COMPONENT> IanaComponents { get; set; }

        /// <summary>
        /// Gets or sets the X-components of the iCalendar core object
        /// </summary>
        List<X_COMPONENT> XComponents { get; set; }
    }
}