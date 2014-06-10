using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using reexmonkey.xcal.domain.models;
using reexmonkey.technical.data.contracts;

namespace reexmonkey.xcal.service.repositories.contracts
{
    public interface ICalendarRepository:
        IReadRepository<VCALENDAR, string>,
        IWriteRepository<VCALENDAR, string>,
        IReadRepositoryKeys<string>
    {
        /// <summary>
        /// Gets or sets the repository of addressing the event aggregate root
        /// </summary>
        IEventRepository EventRepository { get; set; }

        /// <summary>
        /// Populates a sparse calendar entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse calendar entity to be populated</param>
        /// <returns>The populated calendar entity</returns>
        VCALENDAR Hydrate(VCALENDAR dry);

        /// <summary>
        /// Populates respective calendars with aggregate entities
        /// </summary>
        /// <param name="dry">The sparse calendar entities to be populated</param>
        /// <returns>Populated calendar entities</returns>
        IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry);

        /// <summary>
        /// Depopulates aggregate entities from a calendar 
        /// </summary>
        /// <param name="full">The calendar to be depopulated</param>
        /// <returns>A depopulated calendar</returns>
        VCALENDAR Dehydrate(VCALENDAR full);

        IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full);
    }

    /// <summary>
    /// Specifies an interface for a repository of calendars hosted on a relational data source
    /// </summary>
    public interface ICalendarOrmLiteRepository : ICalendarRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; set; }
    }


    /// <summary>
    /// Specifies an interface for a repository of calendars hosted on a NoSQL Redis data source
    /// </summary>
    public interface ICalendarRedisRepository : ICalendarRepository
    {
        /// <summary>
        /// Gets the Redis client manager
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; set; }
    }
}
