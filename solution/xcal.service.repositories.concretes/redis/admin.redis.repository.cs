using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using reexmonkey.xcal.service.repositories.contracts;



namespace reexmonkey.xcal.service.repositories.concretes.redis
{
    public class AdminRedisRepository : IAdminRedisRepository
    {
        private IRedisClientsManager manager;
        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
                this.client = manager.GetClient();
            }
        }
               
        public AdminRedisRepository() { }

        public AdminRedisRepository(IRedisClientsManager manager)
        {
            this.RedisClientsManager = manager;
        }

        public AdminRedisRepository(IRedisClient client)
        {
            if (client == null) throw new ArgumentNullException("IRedisClient");
            this.client = client;
        }

        public void FlushDb(bool force = false)
        {
            this.client.FlushDb();
        }
    }
}
