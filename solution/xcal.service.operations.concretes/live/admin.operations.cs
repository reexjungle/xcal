using reexjungle.infrastructure.contracts;
using ServiceStack.ServiceHost;

namespace reexjungle.xcal.service.operations.concretes.live
{
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase : IReturnVoid
    {
        public FlushMode? Mode { get; set; }
    }
}