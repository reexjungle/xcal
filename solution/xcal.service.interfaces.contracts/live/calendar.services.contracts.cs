using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Logging;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;

namespace reexmonkey.xcal.service.interfaces.contracts.live
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

        List<VCALENDAR> Get(FindCalendars request);
    }
}
