using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;
using xcal.domain.contracts.core.properties;
using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.components
{
    public interface IVFREEBUSY
    {
        string Uid { get; }

        IDATE_TIME Datestamp { get; }

        IDATE_TIME Start { get; }

        IORGANIZER Organizer { get; }

        IURL Url { get; }

        IDATE_TIME End { get; }

        List<IATTACH> Attachments { get; }

        List<IATTENDEE> Attendees { get; }

        List<ICOMMENT> Comments { get; }

        List<IVFREEBUSY> FreeBusies { get; }

        List<IREQUEST_STATUS> RequestStatuses { get; }
    }

}
