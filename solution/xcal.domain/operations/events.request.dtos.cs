using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.infrastructure.io.contracts;

namespace reexmonkey.xcal.domain.operations
{

    [DataContract]
    [Route("/calendars/{CalendarId}/events/add", "POST")]
    public class AddEvent : IReturnVoid
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public VEVENT Event { get; set; }
    }

    [DataContract]
    [Route("/calendars/{CalendarId}/events/batch/add", "POST")]
    public class AddEvents : IReturnVoid
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/update", "PUT")]
    public class UpdateEvent : IReturnVoid
    {
        [DataMember]
        public VEVENT Event { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/update", "PUT")]
    public class UpdateEvents : IReturnVoid
    {
        [DataMember]
        public List<VEVENT> Events { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/{EventId}/patch", "PATCH")]
    public class PatchEvent : IReturnVoid
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public GEO Position { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        public LOCATION Location { get; set; }

        [DataMember]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public PRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        public TRANSP Transparency { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        [DataMember]
        public List<ATTACH_URI> AttachmentUris { get; set; }

        [DataMember]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        public CATEGORIES Categories { get; set; }

        [DataMember]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        public List<CONTACT> Contacts { get; set; }

        [DataMember]
        public List<EXDATE> ExceptionDates { get; set; }

        [DataMember]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        public List<RESOURCES> Resources { get; set; }

        [DataMember]
        public List<RELATEDTO> RelatedTos { get; set; }

        [DataMember]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        [DataMember]
        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        [DataMember]
        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        [DataMember]
        public Dictionary<string, IANA_PROPERTY> IANAProperties { get; set; }

        [DataMember]
        public Dictionary<string, X_PROPERTY> XProperties { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/patch", "PATCH")]
    public class PatchEvents : IReturnVoid
    {

        [DataMember]
        public List<string> EventIds { get; set; }

        [DataMember]
        public DATE_TIME Datestamp { get; set; }

        [DataMember]
        public DATE_TIME Start { get; set; }

        [DataMember]
        public CLASS Classification { get; set; }

        [DataMember]
        public DATE_TIME Created { get; set; }

        [DataMember]
        public DESCRIPTION Description { get; set; }

        [DataMember]
        public GEO Position { get; set; }

        [DataMember]
        public DATE_TIME LastModified { get; set; }

        [DataMember]
        public LOCATION Location { get; set; }

        [DataMember]
        public ORGANIZER Organizer { get; set; }

        [DataMember]
        public PRIORITY Priority { get; set; }

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public STATUS Status { get; set; }

        [DataMember]
        public SUMMARY Summary { get; set; }

        [DataMember]
        public TRANSP Transparency { get; set; }

        [DataMember]
        public URI Url { get; set; }

        [DataMember]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        [DataMember]
        public List<ATTACH_URI> AttachmentUris { get; set; }

        [DataMember]
        public List<ATTENDEE> Attendees { get; set; }

        [DataMember]
        public CATEGORIES Categories { get; set; }

        [DataMember]
        public List<COMMENT> Comments { get; set; }

        [DataMember]
        public List<CONTACT> Contacts { get; set; }

        [DataMember]
        public List<EXDATE> ExceptionDates { get; set; }

        [DataMember]
        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        [DataMember]
        public List<RESOURCES> Resources { get; set; }

        [DataMember]
        public List<RELATEDTO> RelatedTos { get; set; }

        [DataMember]
        public List<RDATE> RecurrenceDates { get; set; }

        [DataMember]
        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        [DataMember]
        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        [DataMember]
        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        [DataMember]
        public Dictionary<string, IANA_PROPERTY> IANAProperties { get; set; }

        [DataMember]
        public Dictionary<string, X_PROPERTY> XProperties { get; set; }

    }

    [DataContract]
    [Route("/calendars/events/{EventId}/delete", "DELETE")]
    public class DeleteEvent : IReturnVoid
    {
        [DataMember]
        public string EventId { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/delete", "DELETE")]
    public class DeleteEvents : IReturnVoid
    {
        [DataMember]
        public List<string> EventIds { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/{EventId}/find", "GET")]
    public class FindEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string EventId { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/find", "POST")]
    [Route("/calendars/events/batch/find/{Page}/{Size}", "POST")]
    [Route("/calendars/events/batch/find/page/{Page}/{Size}", "POST")]
    [Route("/calendars/events/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindEvents : IReturn<List<VEVENT>>, IPaginated<int>
    {
        [DataMember]
        public List<string> EventIds { get; set; }

        [DataMember]
        public int? Page { get; set; }

        [DataMember]
        public int? Size { get; set; }

    }

    [DataContract]
    [Route("/calendars/events/{Page}/{Size}", "GET")]
    [Route("/calendars/events/page/{Page}/size/{Size}", "GET")]
    public class GetEvents : IReturn<List<VEVENT>>, IPaginated<int>
    {
        [DataMember]
        public int? Page { get; set; }

        [DataMember]
        public int? Size { get; set; }
    }


}
