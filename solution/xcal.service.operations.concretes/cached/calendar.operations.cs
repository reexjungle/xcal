using reexjungle.infrastructure.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.operations.concretes.cached
{
    [Route("/cached/calendars/{CalendarId}/find", "GET")]
    public class FindCalendarCached : IReturn<VCALENDAR>
    {
        public string CalendarId { get; set; }
    }

    [Route("/cached/calendars/batch/find", "POST")]
    [Route("/cached/calendars/batch/find/{Page}/{Size}", "POST")]
    [Route("/cached/calendars/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindCalendarsCached : IReturn<List<VCALENDAR>>, IPaginated<int>
    {
        public List<string> CalendarIds { get; set; }

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
}