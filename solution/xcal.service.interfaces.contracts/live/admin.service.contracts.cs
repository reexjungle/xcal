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
    public interface IAdminService
    {
        void Post(FlushDatabase request);
    }

}
