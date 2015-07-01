using reexjungle.technical.data.contracts;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.OrmLite;
using ServiceStack.Redis;

namespace reexjungle.xcal.service.repositories.contracts
{
    public interface IAdminRepository
    {
        void Flush(FlushMode mode = FlushMode.soft);
    }
}