using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.properties;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.components
{
    public interface ITODO
    {
        string Uid { get; }

        DATE_TIME Datestamp { get; }

        DATE_TIME Start { get; }

        CLASS Classification { get; }

        DATE_TIME Completed { get; }

        DATE_TIME Created { get; }

        IDESCRIPTION Description { get; }

        IGEO Position { get; }

        DATE_TIME LastModified { get; }

        ILOCATION Location { get; }

        IORGANIZER Organizer { get; }

        int Percent { get; }

        IPRIORITY Priority { get; }

        int Sequence { get; }

        STATUS Status { get; }

        ISUMMARY Summary { get; }

        IURL Url { get; }

        IRECURRENCE_ID RecurrenceId { get; }

        RECUR RecurrenceRule { get; }

        DATE_TIME Due { get; }

        DURATION Duration { get; }

        List<IATTACH> Attachments { get; }

        List<IATTENDEE> Attendees { get; }

        ICATEGORIES Categories { get; }

        List<ICOMMENT> Comments { get; }

        List<ICONTACT> Contacts { get; }

        List<IEXDATE> ExceptionDates { get; }

        List<IREQUEST_STATUS> RequestStatuses { get; }

        List<IRESOURCES> Resources { get; }

        List<IRELATED_TO> RelatedTos { get; }

        List<IRDATE> RecurrenceDates { get; }

        List<IALARM> Alarms { get; }
    }

}
