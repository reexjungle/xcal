using reexjungle.xcal.domain.models;
using System.Collections.Generic;
using reexjungle.xcal.tests.contracts.factories;

namespace reexjungle.xcal.tests.contracts.services
{
    public interface IEventTestService
    {
        VEVENT RandomlyAttend(VEVENT @event, IEnumerable<ATTENDEE> attendees);

        IEnumerable<VEVENT> RandomlyAttend(IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees);
    }
}