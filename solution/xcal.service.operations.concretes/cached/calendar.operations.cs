using reexjungle.xcal.domain.models;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.operations.concretes.cached
{
    [Route("/cached/calendars/{CalendarId}/find", "GET")]
    public class FindCalendarCached : IReturn<VCALENDAR>
    {
        public Guid CalendarId { get; set; }
    }

    [Route("/cached/calendars/batch/find", "POST")]
    [Route("/cached/calendars/batch/find/{Page}/{Size}", "POST")]
    [Route("/cached/calendars/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindCalendarsCached : IReturn<List<VCALENDAR>>, IPaginated<int>
    {
        public List<Guid> CalendarIds { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/cached/calendars/{Page}/{Size}", "GET")]
    [Route("/cached/calendars/page/{Page}/size/{Size}", "GET")]
    public class GetCalendarsCached : IReturn<List<VCALENDAR>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/cached/calendars/keys/{Page}/{Size}", "GET")]
    [Route("/cached/calendars/keys/page/{Page}/size/{Size}", "GET")]
    public class GetCalendarKeysCached : IReturn<List<string>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }
}