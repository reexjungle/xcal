using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.contracts
{
    public interface ICalendarUnitTests : IUnitTests
    {
        IEnumerable<VCALENDAR> GenerateCalendarsOfSize(int n);
    }
}