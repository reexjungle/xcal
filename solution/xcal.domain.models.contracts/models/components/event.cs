using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.properties;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.components
{
    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    public interface IEVENT
    {
        /// <summary>
        ///
        /// </summary>
        string Uid { get; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME Datestamp { get; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME Start { get; }

        /// <summary>
        ///
        /// </summary>
        CLASS Classification { get; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME Created { get; }

        /// <summary>
        ///
        /// </summary>
        IDESCRIPTION Description { get; }

        /// <summary>
        ///
        /// </summary>
        IGEO GeoPosition { get; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME LastModified { get; }

        /// <summary>
        ///
        /// </summary>
        ILOCATION Location { get; }

        /// <summary>
        ///
        /// </summary>
        IORGANIZER Organizer { get; }

        /// <summary>
        ///
        /// </summary>
        IPRIORITY Priority { get; }

        /// <summary>
        ///
        /// </summary>
        int Sequence { get; }

        /// <summary>
        ///
        /// </summary>
        STATUS Status { get; }

        /// <summary>
        ///
        /// </summary>
        ISUMMARY Summary { get; }

        /// <summary>
        ///
        /// </summary>
        TRANSP Transparency { get; }

        /// <summary>
        ///
        /// </summary>
        IURL Url { get; }

        /// <summary>
        ///
        /// </summary>
        IRECURRENCE_ID RecurrenceId { get; }

        /// <summary>
        ///
        /// </summary>
        RECUR RecurrenceRule { get; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME End { get; }

        /// <summary>
        ///
        /// </summary>
        DURATION Duration { get; }

        /// <summary>
        ///
        /// </summary>
        List<IATTACH> Attachments { get; }

        /// <summary>
        ///
        /// </summary>
        List<IATTENDEE> Attendees { get; }

        /// <summary>
        ///
        /// </summary>
        ICATEGORIES Categories { get; }

        /// <summary>
        ///
        /// </summary>
        List<ICOMMENT> Comments { get; }

        /// <summary>
        ///
        /// </summary>
        List<ICONTACT> Contacts { get; }

        /// <summary>
        ///
        /// </summary>
        List<IEXDATE> ExceptionDates { get; }

        /// <summary>
        ///
        /// </summary>
        List<IREQUEST_STATUS> RequestStatuses { get; }

        /// <summary>
        ///
        /// </summary>
        List<IRESOURCES> Resources { get; }

        /// <summary>
        ///
        /// </summary>
        List<IRELATED_TO> RelatedTos { get; }

        /// <summary>
        ///
        /// </summary>
        List<IRDATE> RecurrenceDates { get; }

        /// <summary>
        ///
        /// </summary>
        List<IALARM> Alarms { get; }
    }
}
