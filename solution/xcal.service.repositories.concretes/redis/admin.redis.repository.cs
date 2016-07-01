using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Redis;
using System;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class AdminRedisRepository : IAdminRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private IRedisClient client;

        private IRedisClient redis => client ?? (client = manager.GetClient());

        public IRedisClientsManager RedisClientsManager => manager;

        public AdminRedisRepository(IRedisClientsManager manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
        }

        public void Flush(bool force)
        {
            if (force) client.FlushDb();
            else client.FlushAll();
        }
    }
}