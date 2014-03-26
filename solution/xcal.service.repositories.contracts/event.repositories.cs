using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using reexmonkey.xcal.domain.models;
using reexmonkey.technical.data.contracts;

namespace reexmonkey.xcal.service.repositories.contracts
{
    
    /// <summary>
    /// Specifies a general interface for a repository of events
    /// </summary>
    public interface IEventRepository: 
        IReadRepository<VEVENT, string, string>,
        IWriteRepository<VEVENT, string>,
        IReadRepositoryKeys<string, string>,
        IPaginated<int>
    {
        /// <summary>
        /// Gets or sets the audio alarm repository
        /// </summary>
        IAudioAlarmRepository AudioAlarmRepository { get; set; }

        /// <summary>
        /// Gets or sets the display alarm repository
        /// </summary>
        IDisplayAlarmRepository DisplayAlarmRepository { get; set; }

        /// <summary>
        /// Gets or sets the email alarm repository
        /// </summary>
        IEmailAlarmRepository EmailAlarmRepository { get; set; }

        /// <summary>
        /// Populates a sparse event entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse event entity to be populated</param>
        /// <returns>The populated event entity</returns>
        VEVENT Hydrate(VEVENT dry);

        /// <summary>
        /// Populates event entities with details from respective constituent entities
        /// </summary>
        /// <param name="dry">The sparse events entities to be populated</param>
        /// <returns>Populated event entities</returns>
        IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry);
    } 

    /// <summary>
    /// Specifies an interface for a repository of events connected to an ORMlite source
    /// </summary>
    public interface IEventOrmLiteRepository: IEventRepository
    {
        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; set; }
    }


    /// <summary>
    /// Specifies an interface for a repository of events connected to a NoSQL Redis source
    /// </summary>
    public interface IEventRedisRepository : IEventRepository
    {
        /// <summary>
        /// Gets or sets the Redis Client manager
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; set; }
    }

}
