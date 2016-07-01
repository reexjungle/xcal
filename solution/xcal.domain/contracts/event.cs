using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
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
        DATE_TIME Datestamp { get; set; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME Start { get; set; }

        /// <summary>
        ///
        /// </summary>
        CLASS Classification { get; set; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME Created { get; set; }

        /// <summary>
        ///
        /// </summary>
        DESCRIPTION Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        GEO GeoPosition { get; set; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME LastModified { get; set; }

        /// <summary>
        ///
        /// </summary>
        LOCATION Location { get; set; }

        /// <summary>
        ///
        /// </summary>
        ORGANIZER Organizer { get; set; }

        /// <summary>
        ///
        /// </summary>
        PRIORITY Priority { get; set; }

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
        SUMMARY Summary { get; set; }

        /// <summary>
        ///
        /// </summary>
        TRANSP Transparency { get; set; }

        /// <summary>
        ///
        /// </summary>
        URL Url { get; set; }

        /// <summary>
        ///
        /// </summary>
        RECURRENCE_ID RecurrenceId { get; set; }

        /// <summary>
        ///
        /// </summary>
        RECUR RecurrenceRule { get; set; }

        /// <summary>
        ///
        /// </summary>
        DATE_TIME End { get; set; }

        /// <summary>
        ///
        /// </summary>
        DURATION Duration { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<ATTACH> Attachments { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<ATTENDEE> Attendees { get; set; }

        /// <summary>
        ///
        /// </summary>
        CATEGORIES Categories { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<COMMENT> Comments { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<CONTACT> Contacts { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<EXDATE> ExceptionDates { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<REQUEST_STATUS> RequestStatuses { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<RESOURCES> Resources { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<RELATEDTO> RelatedTos { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<RDATE> RecurrenceDates { get; set; }

        /// <summary>
        ///
        /// </summary>
        List<VALARM> Alarms { get; set; }
    }
}