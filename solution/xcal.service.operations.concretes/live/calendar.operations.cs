using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.operations.concretes.live
{
    [Route("/calendars/add", "POST")]
    public class AddCalendar : IReturnVoid
    {
        public VCALENDAR Calendar { get; set; }
    }

    [Route("/calendars/batch/add", "POST")]
    public class AddCalendars : IReturnVoid
    {
        public List<VCALENDAR> Calendars { get; set; }
    }

    [Route("/calendars/update", "PUT")]
    public class UpdateCalendar : IReturnVoid
    {
        public VCALENDAR Calendar { get; set; }
    }

    [Route("/calendars/batch/update", "PUT")]
    public class UpdateCalendars : IReturnVoid
    {
        public List<VCALENDAR> Calendars { get; set; }
    }

    [Route("/calendars/{CalendarId}/patch", "POST")]
    public class PatchCalendar : IReturnVoid
    {
        public Guid CalendarId { get; set; }

        public string Version { get; set; }

        public CALSCALE Scale { get; set; }

        public METHOD Method { get; set; }

        public string ProductId { get; set; }

        public List<VEVENT> Events { get; set; }

        public List<VTODO> ToDos { get; set; }

        public List<VFREEBUSY> FreeBusies { get; set; }

        public List<VJOURNAL> Journals { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }

        public List<IANA_COMPONENT> IanaComponents { get; set; }

        public List<X_COMPONENT> XComponents { get; set; }
    }

    [Route("/calendars/batch/patch", "POST")]
    public class PatchCalendars : IReturnVoid
    {
        public List<Guid> CalendarIds { get; set; }

        public string Version { get; set; }

        public CALSCALE Scale { get; set; }

        public METHOD Method { get; set; }

        public string ProductId { get; set; }

        public List<VEVENT> Events { get; set; }

        public List<VTODO> ToDos { get; set; }

        public List<VFREEBUSY> FreeBusies { get; set; }

        public List<VJOURNAL> Journals { get; set; }

        public List<VTIMEZONE> TimeZones { get; set; }

        public List<IANA_COMPONENT> IanaComponents { get; set; }

        public List<X_COMPONENT> XComponents { get; set; }
    }

    [Route("/calendars/{CalendarId}/delete", "DELETE")]
    public class DeleteCalendar : IReturnVoid
    {
        public Guid CalendarId { get; set; }
    }

    [Route("/calendars/batch/delete", "POST")]
    public class DeleteCalendars : IReturnVoid
    {
        public List<Guid> CalendarIds { get; set; }
    }

    [Route("/calendars/{CalendarId}/find", "GET")]
    public class FindCalendar : IReturn<VCALENDAR>
    {
        public Guid CalendarId { get; set; }
    }

    [Route("/calendars/batch/find", "POST")]
    [Route("/calendars/batch/find/{Page}/{Size}", "POST")]
    [Route("/calendars/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindCalendars : IReturn<List<VCALENDAR>>, IPaginated<int>
    {
        public List<Guid> CalendarIds { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/calendars/{Page}/{Size}", "GET")]
    [Route("/calendars/page/{Page}/size/{Size}", "GET")]
    public class GetCalendars : IReturn<List<VCALENDAR>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/calendars/keys/{Page}/{Size}", "GET")]
    [Route("/calendars/keys/page/{Page}/size/{Size}", "GET")]
    public class GetCalendarKeys : IReturn<List<Guid>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }
}