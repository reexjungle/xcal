using reexjungle.infrastructure.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.operations.concretes.cached
{
    [Route("/cached/calendars/events/{EventId}/find", "GET")]
    public class FindEventCached : IReturn<VEVENT>
    {
        public string EventId { get; set; }
    }

    [Route("/cached/calendars/events/batch/find", "POST")]
    [Route("/cached/calendars/events/batch/find/{Page}/{Size}", "POST")]
    [Route("/cached/calendars/events/batch/find/page/{Page}/size/{Size}", "POST")]
    public class FindEventsCached : IReturn<List<VEVENT>>, IPaginated<int>
    {
        public List<string> EventIds { get; set; }

        public int? Page { get; set; }

        public int? Size { get; set; }
    }

    [Route("/cached/calendars/events/{Page}/{Size}", "GET")]
    [Route("/cached/calendars/events/page/{Page}/size/{Size}", "GET")]
    public class GetEventsCached : IReturn<List<VEVENT>>, IPaginated<int>
    {
        public int? Page { get; set; }

        public int? Size { get; set; }
    }
}