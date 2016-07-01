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

        List<ATTACH> Attachments { get; set; }

        List<ATTENDEE> Attendees { get; set; }

        List<COMMENT> Comments { get; set; }

        List<FREEBUSY> FreeBusyIntervals { get; set; }

        List<REQUEST_STATUS> RequestStatuses { get; set; }
    }
}