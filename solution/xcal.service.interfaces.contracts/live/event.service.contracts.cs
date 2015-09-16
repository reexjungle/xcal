using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.live;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.interfaces.contracts.live
{
    public interface IEventWebService
    {
        void Post(AddEvent request);

        void Post(AddEvents request);

        void Put(UpdateEvent request);

        void Put(UpdateEvents request);

        void Post(PatchEvent request);

        void Post(PatchEvents request);

        void Delete(DeleteEvent request);

        void Post(DeleteEvents request);

        VEVENT Get(FindEvent request);

        List<VEVENT> Post(FindEvents request);

        List<VEVENT> Get(GetEvents request);

        List<Guid> Get(GetEventKeys request);
    }
}