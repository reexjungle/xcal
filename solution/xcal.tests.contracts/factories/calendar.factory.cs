using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface ICalendarFactory
    {
        VCALENDAR Create();

        IEnumerable<VCALENDAR> Create(int quantity);
    }
}