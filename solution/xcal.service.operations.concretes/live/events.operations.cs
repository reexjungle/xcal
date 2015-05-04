using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.ServiceHost;
using System.Collections.Generic;

namespace reexjungle.xcal.service.operations.concretes.live
{
    #region Search-Create-Update-Patch-Delete(SCRUPD) operations

    [Route("/calendars/{CalendarId}/events/add", "POST")]
    public class AddEvent : IReturnVoid
    {
        public string CalendarId { get; set; }

        public VEVENT Event { get; set; }
    }

    [Route("/calendars/{CalendarId}/events/batch/add", "POST")]
    public class AddEvents : IReturnVoid
    {
        public string CalendarId { get; set; }

        public List<VEVENT> Events { get; set; }
    }

    [Route("/calendars/events/update", "PUT")]
    public class UpdateEvent : IReturnVoid
    {
        public VEVENT Event { get; set; }
    }

    [Route("/calendars/events/batch/update", "PUT")]
    public class UpdateEvents : IReturnVoid
    {
        public List<VEVENT> Events { get; set; }
    }

    [Route("/calendars/events/{EventId}/patch", "PATCH")]
    public class PatchEvent : IReturnVoid
    {
        public string EventId { get; set; }

        public DATE_TIME Datestamp { get; set; }

        public DATE_TIME Start { get; set; }

        public CLASS Classification { get; set; }

        public DATE_TIME Created { get; set; }

        public DESCRIPTION Description { get; set; }

        public GEO Position { get; set; }

        public DATE_TIME LastModified { get; set; }

        public LOCATION Location { get; set; }

        public ORGANIZER Organizer { get; set; }

        public PRIORITY Priority { get; set; }

        public int Sequence { get; set; }

        public STATUS Status { get; set; }

        public SUMMARY Summary { get; set; }

        public TRANSP Transparency { get; set; }

        public URI Url { get; set; }

        public RECUR RecurrenceRule { get; set; }

        public DATE_TIME End { get; set; }

        public DURATION Duration { get; set; }

        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        public List<ATTACH_URI> AttachmentUris { get; set; }

        public List<ATTENDEE> Attendees { get; set; }

        public CATEGORIES Categories { get; set; }

        public List<COMMENT> Comments { get; set; }

        public List<CONTACT> Contacts { get; set; }

        public List<EXDATE> ExceptionDates { get; set; }

        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        public List<RESOURCES> Resources { get; set; }

        public List<RELATEDTO> RelatedTos { get; set; }

        public List<RDATE> RecurrenceDates { get; set; }

        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        public Dictionary<string, IANA_PROPERTY> IANAProperties { get; set; }

        public Dictionary<string, X_PROPERTY> XProperties { get; set; }
    }

    [Route("/calendars/events/batch/patch", "PATCH")]
    public class PatchEvents : IReturnVoid
    {
        public List<string> EventIds { get; set; }

        public DATE_TIME Datestamp { get; set; }

        public DATE_TIME Start { get; set; }

        public CLASS Classification { get; set; }

        public DATE_TIME Created { get; set; }

        public DESCRIPTION Description { get; set; }

        public GEO Position { get; set; }

        public DATE_TIME LastModified { get; set; }

        public LOCATION Location { get; set; }

        public ORGANIZER Organizer { get; set; }

        public PRIORITY Priority { get; set; }

        public int Sequence { get; set; }

        public STATUS Status { get; set; }

        public SUMMARY Summary { get; set; }

        public TRANSP Transparency { get; set; }

        public URI Url { get; set; }

        public RECUR RecurrenceRule { get; set; }

        public DATE_TIME End { get; set; }

        public DURATION Duration { get; set; }

        public List<ATTACH_BINARY> AttachmentBinaries { get; set; }

        public List<ATTACH_URI> AttachmentUris { get; set; }

        public List<ATTENDEE> Attendees { get; set; }

        public CATEGORIES Categories { get; set; }

        public List<COMMENT> Comments { get; set; }

        public List<CONTACT> Contacts { get; set; }

        public List<EXDATE> ExceptionDates { get; set; }

        public List<REQUEST_STATUS> RequestStatuses { get; set; }

        public List<RESOURCES> Resources { get; set; }

        public List<RELATEDTO> RelatedTos { get; set; }

        public List<RDATE> RecurrenceDates { get; set; }

        public List<AUDIO_ALARM> AudioAlarms { get; set; }

        public List<DISPLAY_ALARM> DisplayAlarms { get; set; }

        public List<EMAIL_ALARM> EmailAlarms { get; set; }

        public Dictionary<string, IANA_PROPERTY> IANAProperties { get; set; }

        public Dictionary<string, X_PROPERTY> XProperties { get; set; }
    }

    [Route("/calendars/events/{EventId}/delete", "DELETE")]
    public class DeleteEvent : IReturnVoid
    {
        public string EventId { get; set; }
    }

    [Route("/calendars/events/batch/delete", "DELETE")]
    public class DeleteEvents : IReturnVoid
    {
        public List<string> EventIds { get; set; }
    }

    [Route("/calendars/events/{EventId}/find", "GET")]
    public class FindEvent : IReturn<VEVENT>
    {
        public string EventId { get; set; }
    }

    [Route("/calendars/events/batch/find", "POST")]
    [Route("/calendars/events/batch/find/{Page}/{Size}", "POST")]
    [Route("/calendars/events/batch/find/page/{Page}/{Size}", "POST")]
    [Route("/calendars/events/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindEvents : IReturn<List<VEVENT>>, IPaginated<int>
    {
        public List<string> EventIds { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/calendars/events/{Page}/{Size}", "GET")]
    [Route("/calendars/events/page/{Page}/size/{Size}", "GET")]
    public class GetEvents : IReturn<List<VEVENT>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/calendars/events/keys/{Page}/{Size}", "GET")]
    [Route("/calendars/events/keys/page/{Page}/size/{Size}", "GET")]
    public class GetEventKeys : IReturn<List<string>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    #endregion Search-Create-Update-Patch-Delete(SCRUPD) operations
}