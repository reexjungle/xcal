using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    public interface IJOURNAL
    {
        string Uid { get; set; }

        DATE_TIME Datestamp { get; set; }

        DATE_TIME Start { get; set; }

        CLASS Classification { get; set; }

        DATE_TIME Created { get; set; }

        DESCRIPTION Description { get; set; }

        DATE_TIME LastModified { get; set; }

        ORGANIZER Organizer { get; set; }

        int Sequence { get; set; }

        STATUS Status { get; set; }

        SUMMARY Summary { get; set; }

        URL Url { get; set; }

        RECURRENCE_ID RecurrenceId { get; set; }

        RECUR RecurrenceRule { get; set; }

        List<IATTACH> Attachments { get; set; }

        List<ATTENDEE> Attendees { get; set; }

        CATEGORIES Categories { get; set; }

        List<COMMENT> Comments { get; set; }

        List<CONTACT> Contacts { get; set; }

        List<EXDATE> ExceptionDates { get; set; }

        List<REQUEST_STATUS> RequestStatuses { get; set; }

        List<RESOURCES> Resources { get; set; }

        List<RELATEDTO> RelatedTos { get; set; }

        List<RDATE> RecurrenceDates { get; set; }

        List<IANA_PROPERTY> IANA { get; set; }

        List<X_PROPERTY> NonStandard { get; set; }
    }
}