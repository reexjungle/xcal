using reexjungle.infrastructure.contracts;
using reexjungle.technical.data.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.repositories.contracts
{
    public interface IAdminRepository : IRepository
    {
        void Flush(FlushMode mode = FlushMode.soft);
    }

    public interface IAdminOrmLiteRepository : IAdminRepository
    {
        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; set; }
    }

    /// <summary>
    /// Specifies an interface for a repository of events connected to a NoSQL Redis source
    /// </summary>
    public interface IAdminRedisRepository : IAdminRepository
    {
        /// <summary>
        /// Gets or sets the Redis Client manager
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; set; }
    }
}