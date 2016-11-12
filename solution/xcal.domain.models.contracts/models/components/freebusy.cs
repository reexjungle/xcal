using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.properties;
using reexjungle.xcal.core.domain.contracts.models.values;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.components
{
    public interface IFREEBUSY
    {
        string Uid { get; }

        DATE_TIME Datestamp { get; }

        DATE_TIME Start { get; }

        IORGANIZER Organizer { get; }

        IURL Url { get; }

        DATE_TIME End { get; }

        List<IATTACH> Attachments { get; }

        List<IATTENDEE> Attendees { get; }

        List<ICOMMENT> Comments { get; }

        List<IFREEBUSY> FreeBusies { get; }

        List<IREQUEST_STATUS> RequestStatuses { get; }
    }

}
