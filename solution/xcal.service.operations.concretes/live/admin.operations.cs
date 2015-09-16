using ServiceStack.ServiceHost;

namespace reexjungle.xcal.service.operations.concretes.live
{
    [Route("/admin/database/flush", "POST")]
    public class FlushDatabase : IReturnVoid
    {
        public bool? Force { get; set; }
    }
}