using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using reexmonkey.xcal.domain.models;
using reexmonkey.technical.data.contracts;

namespace reexmonkey.xcal.service.repositories.contracts
{
    public interface IAdminRepository: IRepository
    {
        void FlushDb(bool force=false);
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
