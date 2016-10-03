using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
{
    public interface ITODO
    {
        string Uid { get; set; }

        IDATE_TIME Datestamp { get; set; }

        IDATE_TIME Start { get; set; }

        CLASS Classification { get; set; }

        IDATE_TIME Completed { get; set; }

        IDATE_TIME Created { get; set; }

        IDESCRIPTION Description { get; set; }

        IGEO Position { get; set; }

        IDATE_TIME LastModified { get; set; }

        ILOCATION Location { get; set; }

        IORGANIZER Organizer { get; set; }

        int Percent { get; set; }

        IPRIORITY Priority { get; set; }

        int Sequence { get; set; }

        STATUS Status { get; set; }

        ISUMMARY Summary { get; set; }

        IURL Url { get; set; }

        IRECURRENCE_ID RecurrenceId { get; set; }

        IRECUR RecurrenceRule { get; set; }

        IDATE_TIME Due { get; set; }

        IDURATION Duration { get; set; }

        List<IATTACH> Attachments { get; set; }

        List<IATTENDEE> Attendees { get; set; }

        ICATEGORIES Categories { get; set; }

        List<ICOMMENT> Comments { get; set; }

        List<ICONTACT> Contacts { get; set; }

        List<IEXDATE> ExceptionDates { get; set; }

        List<IREQUEST_STATUS> RequestStatuses { get; set; }

        List<IRESOURCES> Resources { get; set; }

        List<IRELATED_TO> RelatedTos { get; set; }

        List<IRDATE> RecurrenceDates { get; set; }

        List<IALARM> Alarms { get; set; }
    }

}
