using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.cached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.service.interfaces.contracts.cached
{
    public interface ICachedCalendarService
    {
        object Get(FindCalendarCached request);

        object Post(FindCalendarsCached request);

        object Get(GetCalendarsCached request);
    }
}