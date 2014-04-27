using System.Collections.Generic;
using reexmonkey.xcal.domain.models;


namespace reexmonkey.xcal.domain.contracts
{
    public interface IFREEBUSY: ICOMPONENT
    {
        string Uid { get; set; }

        DATE_TIME Datestamp { get; set; }

        DATE_TIME Start { get; set; }

        ORGANIZER Organizer { get; set; }

        URI Url { get; set; }

        DATE_TIME End { get; set; }

        List<IATTACH> Attachments { get; set; }

        List<ATTENDEE> Attendees { get; set; }

        List<COMMENT> Comments { get; set; }

        List<FREEBUSY_INFO> FreeBusyProperties { get; set; }

        List<REQUEST_STATUS> RequestStatuses { get; set; }

        List<IANA_PROPERTY> IANA { get; set; }

        List<X_PROPERTY> NonStandard { get; set; }
    }
}
