using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using System.Data;

namespace reexjungle.xcal.service.repositories.contracts
{
    /// <summary>
    /// Specifies a contract for a repository hosted on a relational data source connected through the lightweight ORMLite connector.
    /// </summary>
    public interface IOrmLiteRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; }
    }

    /// <summary>
    /// Speicifies a contract for a repository based on a relational data source connected through the lighweight Dapper ORM connector
    /// </summary>
    public interface IDapperRepository
    {
        /// <summary>
        ///
        /// </summary>
        IFactory<IDbConnection> DbConnectionFactory { get; }
    }

    /// <summary>
    /// Specifies an interface for a repository hosted on a NoSQL Redis data source
    /// </summary>
    public interface IRedisRepository
    {
        /// <summary>
        /// Gets the Redis client manager
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; }
    }
}