using reexjungle.infrastructure.contracts;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;

namespace reexjungle.xcal.service.operations.concretes.live
{
    [DataContract]
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase : IReturnVoid
    {
        [DataMember]
        public FlushMode? Mode { get; set; }
    }
}