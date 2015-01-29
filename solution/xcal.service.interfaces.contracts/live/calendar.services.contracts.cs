using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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