using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.contracts
{
    public interface IEventUnitTests : IUnitTests
    {
        IEnumerable<VEVENT> GenerateEventsOfSize(int n);

        void RandomlyAttendEvents(ref IEnumerable<VEVENT> events, IEnumerable<ATTENDEE> attendees);
    }
}