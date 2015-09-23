using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.services;

namespace reexjungle.xcal.tests.concretes.services
{
    public class CalendarTestService: ICalendarTestService
    {

        public VCALENDAR RandomlySchedule(VCALENDAR calendar, IEnumerable<VEVENT> events)
        {
            var max = events.Count();
            var evs = events as IList<VEVENT> ?? events.ToList();

            calendar.Events.AddRange(Pick<VEVENT>.UniqueRandomList(With.Between(1, max)).From(evs));
            return calendar;
        }

        public IEnumerable<VCALENDAR> RandomlySchedule(IEnumerable<VCALENDAR> calendars, IEnumerable<VEVENT> events)
        {
            var max = events.Count();
            var evs = events as IList<VEVENT> ?? events.ToList();
            foreach (var calendar in calendars)
            {
                calendar.Events.AddRange(Pick<VEVENT>
                    .UniqueRandomList(With.Between(1, max)).From(evs));
            }

            return calendars;
        }
    }
}
