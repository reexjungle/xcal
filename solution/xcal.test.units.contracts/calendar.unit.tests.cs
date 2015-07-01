using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.contracts
{
    public interface ICalendarUnitTest
    {
        IEnumerable<VCALENDAR> GenerateCalendarsOfSize(int n);
    }
}