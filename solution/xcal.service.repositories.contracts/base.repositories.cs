using ServiceStack.OrmLite;
using ServiceStack.Redis;

namespace reexjungle.xcal.service.repositories.contracts
{
    /// <summary>
    /// Specifies a contract for a repository hosted on a relational data source connected through an ORM connector.
    /// </summary>
    public interface IOrmRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; }
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