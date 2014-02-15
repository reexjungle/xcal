﻿using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.contracts
{
    
    /// <summary>
    /// Specifies a general interface for a repository of events
    /// </summary>
    public interface IEventRepository:
        IReadRepository<VEVENT, string>,
        IWriteRepository<VEVENT, string>,
        IPaged<int>
    {

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
        IEnumerable<VEVENT> Rehydrate(IEnumerable<VEVENT> dry);

        /// <summary>
        /// Depopulates an event entity of its constituent details
        /// </summary>
        /// <param name="full">The event entity to be depopulated</param>
        /// <returns>A sparse event entity</returns>
        VEVENT Dehydrate(VEVENT full);

        /// <summary>
        /// Depopulates event entities of constituent event details
        /// </summary>
        /// <param name="full">The events entities to be depopulated</param>
        /// <returns>Populated event entities</returns>
        IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> full);
    } 

    /// <summary>
    /// Specifies an interface for a repository of events connected to an ORMlite source
    /// </summary>
    public interface IEventOrmLiteRepository: IEventRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; }
    }


    /// <summary>
    /// Specifies an interface for a repository of events connected to a NoSQL Redis source
    /// </summary>
    public interface IEventRedisRepository : IEventRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; }
    }

}
