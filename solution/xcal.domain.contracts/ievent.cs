using System;
using System.Collections.Generic;

namespace reexmonkey.xcal.domain.contracts
{

    /// <summary>
    /// Specifes a contract for the VEVENT component of the iCalendar Core Object
    /// </summary>
    public interface IEVENT: ICOMPONENT
    {
        //Required
        string Uid { get; set; }
        IDATE_TIME Datestamp { get; set; }
        IDATE_TIME Start { get; set; }
        //Optional
        CLASS Classification { get; set; }
        IDATE_TIME Created { get; set; }
        ITEXT Description { get; set; }
        IGEO Geo { get; set; }
        IDATE_TIME LastModified { get; set; }
        ITEXT Location { get; set; }
        IORGANIZER Organizer { get; set; }
        IPRIORITY Priority { get; set; }
        int Sequence { get; set; }
        STATUS Status { get; set; }
        ITEXT Summary { get; set; }
        TRANSP Transparency { get; set; }
        IURL Url { get; set; }
        IRECURRENCE_ID RecurrenceId { get; set; }
        IRECUR RecurrenceRule { get; set; }
        IDATE_TIME End { get; set; }
        IDURATION Duration { get; set; }
        List<IATTACH> Attachments { get; set; }
        List<IATTENDEE> Attendees { get; set; }
        ICATEGORIES Categories { get; set; }
        List<ITEXT> Comments { get; set; }
        List<ICONTACT> Contacts { get; set; }
        List<IEXDATE> ExceptionDates { get; set; }
        List<IREQUEST_STATUS> RequestStatuses { get; set; }
        List<IRESOURCES> Resources { get; set; }
        List<IRELATEDTO> RelatedTos { get; set; }
        List<IRDATE> RecurrenceDates { get; set; }
        List<IALARM> Alarms { get; set; }
        List<IIANA_PROPERTY> IANAProperties { get; set; }
        List<IXPROPERTY> XProperties { get; set; }

    }

 
}
