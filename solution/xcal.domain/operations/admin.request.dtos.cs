using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexjungle.infrastructure.io.contracts;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;

namespace reexjungle.xcal.domain.operations
{
    [DataContract]
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase: IReturnVoid 
    {
        [DataMember]
        public FlushMode? Mode { get; set; }
    }
}
