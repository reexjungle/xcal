using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
{
    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    public interface IEVENT
    {
        /// <summary>
        ///
        /// </summary>
        string Uid { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDATE_TIME Datestamp { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDATE_TIME Start { get; set; }

        /// <summary>
        ///
        /// </summary>
        CLASS Classification { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDATE_TIME Created { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDESCRIPTION Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        IGEO GeoPosition { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDATE_TIME LastModified { get; set; }

        /// <summary>
        ///
        /// </summary>
        ILOCATION Location { get; set; }

        /// <summary>
        ///
        /// </summary>
        IORGANIZER Organizer { get; set; }

        /// <summary>
        ///
        /// </summary>
        IPRIORITY Priority { get; set; }

        /// <summary>
        ///
        /// </summary>
        int Sequence { get; set; }

        /// <summary>
        ///
        /// </summary>
        STATUS Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        ISUMMARY Summary { get; set; }

        /// <summary>
        ///
        /// </summary>
        TRANSP Transparency { get; set; }

        /// <summary>
        ///
        /// </summary>
        IURL Url { get; set; }

        /// <summary>
        ///
        /// </summary>
        IRECURRENCE_ID RecurrenceId { get; set; }

        /// <summary>
        ///
        /// </summary>
        IRECUR RecurrenceRule { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDATE_TIME End { get; set; }

        /// <summary>
        ///
        /// </summary>
        IDURATION Duration { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IATTACH> Attachments { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IATTENDEE> Attendees { get; set; }

        /// <summary>
        ///
        /// </summary>
        ICATEGORIES Categories { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<ICOMMENT> Comments { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<ICONTACT> Contacts { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IEXDATE> ExceptionDates { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IREQUEST_STATUS> RequestStatuses { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IRESOURCES> Resources { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IRELATED_TO> RelatedTos { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IRDATE> RecurrenceDates { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<IALARM> Alarms { get; set; }
    }
}
