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
    public interface IEventService
    {
        void Post(AddEvent request);

        void Post(AddEvents request);

        void Put(UpdateEvent request);

        void Put(UpdateEvents request);

        void Patch(PatchEvent request);

        void Patch(PatchEvents request);

        void Delete(DeleteEvent request);

        void Delete(DeleteEvents request);

        VEVENT Get(FindEvent request);

        List<VEVENT> Post(FindEvents request);

        List<VEVENT> Get(GetEvents request);
    }
}
