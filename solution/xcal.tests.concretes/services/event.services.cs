using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xcal.tests.contracts.services;

namespace reexjungle.xcal.tests.concretes.services
{
    public class EventTestService: IEventTestService
    {

        public VEVENT RandomlyAttend(VEVENT @event, IEnumerable<ATTENDEE> attendees)
        {
            var max = attendees.Count();
            var atts = attendees as IList<ATTENDEE> ?? attendees.ToList();
            @event.Attendees.AddRange(Pick<ATTENDEE>
                .UniqueRandomList(With.Between(1, max)).From(atts));

            return @event;
        }



        public IEnumerable<VEVENT> RandomlyAttend(IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees)
        {
            var max = attendees.Count();
            var atts = attendees as IList<ATTENDEE> ?? attendees.ToList();
            foreach (var @event in events)
            {
                @event.Attendees.AddRange(Pick<ATTENDEE>
                    .UniqueRandomList(With.Between(1, max)).From(atts));
            }

            return events;
        }
    }
}
