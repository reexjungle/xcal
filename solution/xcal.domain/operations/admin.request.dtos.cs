using reexjungle.infrastructure.contracts;

using reexjungle.infrastructure.contracts;

using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.operations
{
    [DataContract]
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase : IReturnVoid
    {
        [DataMember]
        public FlushMode? Mode { get; set; }
    }
}