using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.operations
{

    [DataContract]
    [Route("/calendars/{CalendarId}/events/add", "POST")]
    public class AddEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public VEVENT Event { get; set; }
    }

    [DataContract]
    [Route("/calendars/{CalendarId}/events/batch/add", "POST")]
    public class AddEvents: IReturn<List<VEVENT>>
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/{EventId}/update", "PUT")]
    public class UpdateEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public VEVENT Event { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/update", "PUT")]
    public class UpdateEvents : IReturn<List<VEVENT>>
    {
        [DataMember]
        public List<string> EventIds { get; set; }

        [DataMember]
        public List<VEVENT> Events { get; set; }
    }

    [DataContract]
    [KnownType(typeof(ATTACH_BINARY))]
    [KnownType(typeof(ATTACH_URI))]
    [KnownType(typeof(AUDIO_ALARM))]
    [KnownType(typeof(DISPLAY_ALARM))]
    [KnownType(typeof(EMAIL_ALARM))]
    [Route("/calendars/events/{EventId}/patch", "PATCH")]
    public class PatchEvent: IReturn<VEVENT>
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
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public List<IATTACH> Attachments { get; set; }

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
        public List<IALARM> Alarms { get; set; }

        [DataMember]
        public List<IANA_PROPERTY> IANA { get; set; }

        [DataMember]
        public List<X_PROPERTY> NonStandard { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/patch", "PATCH")]
    public class PatchEvents : IReturn<List<VEVENT>>
    {
        [DataMember]
        public string CalendarId { get; set; }

        [DataMember]
        public List<string> EventIds { get; set; }

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
        public RECURRENCE_ID RecurrenceId { get; set; }

        [DataMember]
        public RECUR RecurrenceRule { get; set; }

        [DataMember]
        public DATE_TIME End { get; set; }

        [DataMember]
        public DURATION Duration { get; set; }

        [DataMember]
        public List<IATTACH> Attachments { get; set; }

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
        public List<IALARM> Alarms { get; set; }

        [DataMember]
        public List<IANA_PROPERTY> IANA { get; set; }

        [DataMember]
        public List<X_PROPERTY> NonStandard { get; set; }

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
    [Route("/calendars/events/{EventId}/get", "GET")]
    public class GetEvent : IReturn<VEVENT>
    {
        [DataMember]
        public string EventId { get; set; }
    }

    [DataContract]
    [Route("/calendars/events/batch/get", "GET")]
    [Route("/calendars/events/batch/get/{Page}/{Take}", "GET")]
    [Route("/calendars/events/batch/get/page/{Page}/{Take}", "GET")]
    [Route("/calendars/events/batch/get/page/{Page}/take/{Take}", "GET")]
    public class GetEvents : IReturn<List<VEVENT>>
    {
        [DataMember]
        public List<string> EventIds { get; set; }

        [DataMember]
        public int? Page { get; set; }

        [DataMember]
        public int? Take { get; set; }

    }


}
