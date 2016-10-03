using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
{
    public interface IFREEBUSY
    {
        string Uid { get; set; }

        IDATE_TIME Datestamp { get; set; }

        IDATE_TIME Start { get; set; }

        IORGANIZER Organizer { get; set; }

        IURL Url { get; set; }

        IDATE_TIME End { get; set; }

        List<IATTACH> Attachments { get; set; }

        List<IATTENDEE> Attendees { get; set; }

        List<ICOMMENT> Comments { get; set; }

        List<IFREEBUSY> FreeBusyIntervals { get; set; }

        List<IREQUEST_STATUS> RequestStatuses { get; set; }
    }

}
