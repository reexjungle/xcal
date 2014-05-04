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
    public interface IEventService
    {
        VEVENT Post(AddEvent request);

        List<VEVENT> Post(AddEvents request);

        VEVENT Put(UpdateEvent request);

        List<VEVENT> Put(UpdateEvents request);

        VEVENT Patch(PatchEvent request);

        List<VEVENT> Patch(PatchEvents request);

        void Delete(DeleteEvent request);

        void Delete(DeleteEvents request);

        VEVENT Get(FindEvent request);

        List<VEVENT> Get(FindEvents request);
    }
}
