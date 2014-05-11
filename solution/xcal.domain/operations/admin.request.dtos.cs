using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using reexmonkey.infrastructure.io.contracts;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.operations
{
    [DataContract]
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase: IReturnVoid 
    {
        [DataMember]
        public bool? Reset { get; set; }
    }
}
