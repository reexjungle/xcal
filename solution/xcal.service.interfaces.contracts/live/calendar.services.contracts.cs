using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;

namespace reexjungle.xcal.service.interfaces.contracts.live
{
    public interface ICalendarService
    {
        void Post(AddCalendar request);

        void Post(AddCalendars request);

        void Put(UpdateCalendar request);

        void Put(UpdateCalendars request);

        void Patch(PatchCalendar request);
        
        void Patch(PatchCalendars request);

        void Delete(DeleteCalendar request);

        void Delete(DeleteCalendars request);

        VCALENDAR Get(FindCalendar request);

        List<VCALENDAR> Post(FindCalendars request);
    }
}
