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
        VCALENDAR Post(AddCalendar request);

        List<VCALENDAR> Post(AddCalendars request);

        VCALENDAR Put(UpdateCalendar request);

        List<VCALENDAR> Put(UpdateCalendars request);

        VCALENDAR Patch(PatchCalendar request);
        
        List<VCALENDAR> Patch(PatchCalendars request);

        void Delete(DeleteCalendar request);

        void Delete(DeleteCalendars request);

        VCALENDAR Get(GetCalendar request);

        List<VCALENDAR> Get(GetCalendars request);
    }
}
