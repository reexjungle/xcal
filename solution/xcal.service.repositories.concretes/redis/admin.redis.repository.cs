using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.Redis;
using System;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class AdminRedisRepository : IAdminRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private IRedisClient client;

        private IRedisClient redis
        {
            get
            {
                return client ?? (client = manager.GetClient());
            }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return manager; }
        }

        public AdminRedisRepository(IRedisClientsManager manager)
        {
            if (manager == null) throw new ArgumentNullException("manager");
            this.manager = manager;
        }

        public void Flush(FlushMode mode = FlushMode.soft)
        {
            if (mode == FlushMode.soft) client.FlushDb();
            else client.FlushAll();
        }
    }
}