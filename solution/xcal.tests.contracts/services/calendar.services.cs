using System.Collections.Generic;
using reexjungle.xcal.domain.models;

namespace reexjungle.xcal.tests.contracts.services
{
    public interface ICalendarTestService
    {
        VCALENDAR RandomlySchedule(VCALENDAR calendar, IEnumerable<VEVENT> events);

        IEnumerable<VCALENDAR> RandomlySchedule(IEnumerable<VCALENDAR> calendars, IEnumerable<VEVENT> events);
    }
}