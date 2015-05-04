using reexjungle.xcal.service.operations.concretes.live;

namespace reexjungle.xcal.service.interfaces.contracts.live
{
    public interface IAdminService
    {
        void Post(FlushDatabase request);
    }
}