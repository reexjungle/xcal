using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    public interface IFREEBUSY
    {
        string Uid { get; set; }

        DATE_TIME Datestamp { get; set; }

        DATE_TIME Start { get; set; }

        ORGANIZER Organizer { get; set; }

        URL Url { get; set; }

        DATE_TIME End { get; set; }

        List<IATTACH> Attachments { get; set; }

        List<ATTENDEE> Attendees { get; set; }

        List<COMMENT> Comments { get; set; }

        List<FREEBUSY> FreeBusyProperties { get; set; }

        List<REQUEST_STATUS> RequestStatuses { get; set; }

        List<IANA_PROPERTY> IANA { get; set; }

        List<X_PROPERTY> NonStandard { get; set; }
    }
}