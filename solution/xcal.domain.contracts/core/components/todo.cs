using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
{
    public interface IVTODO
    {
        string Uid { get; }

        IDATE_TIME Datestamp { get; }

        IDATE_TIME Start { get; }

        CLASS Classification { get; }

        IDATE_TIME Completed { get; }

        IDATE_TIME Created { get; }

        IDESCRIPTION Description { get; }

        IGEO Position { get; }

        IDATE_TIME LastModified { get; }

        ILOCATION Location { get; }

        IORGANIZER Organizer { get; }

        int Percent { get; }

        IPRIORITY Priority { get; }

        int Sequence { get; }

        STATUS Status { get; }

        ISUMMARY Summary { get; }

        IURL Url { get; }

        IRECURRENCE_ID RecurrenceId { get; }

        IRECUR RecurrenceRule { get; }

        IDATE_TIME Due { get; }

        IDURATION Duration { get; }

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

        List<IVALARM> Alarms { get; }
    }

}
