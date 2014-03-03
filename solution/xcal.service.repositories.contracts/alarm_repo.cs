using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.repositories.contracts
{

    #region audio alarm repository

    /// <summary>
    /// Specifies a general interface for a repository of audio alerts
    /// </summary>
    public interface IAudioAlarmRepository :
        IReadRepository<AUDIO_ALARM, string, string>,
        IWriteRepository<AUDIO_ALARM, string>,
        IPaged<int> {}

    /// <summary>
    /// Specifies an interface for a repository of audio alerts connected to an ORMlite source
    /// </summary>
    public interface IAudioAlarmOrmLiteRepository : IAudioAlarmRepository
    {
        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; set; }
    }


    /// <summary>
    /// Specifies an interface for a repository of audio alerts connected to a NoSQL Redis source
    /// </summary>
    public interface IAudioAlarmRedisRepository : IAudioAlarmRepository
    {
        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; set; }
    } 

    #endregion

    #region display alarm repository

    /// <summary>
    /// Specifies a general interface for a repository of display alerts
    /// </summary>
    public interface IDisplayAlarmRepository :
        IReadRepository<DISPLAY_ALARM, string, string>,
        IWriteRepository<DISPLAY_ALARM, string>,
        IPaged<int> {}

    /// <summary>
    /// Specifies an interface for a repository of display alerts connected to an ORMlite source
    /// </summary>
    public interface IDisplayAlarmOrmLiteRepository : IDisplayAlarmRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; }
    }


    /// <summary>
    /// Specifies an interface for a repository of display alerts connected to a NoSQL Redis source
    /// </summary>
    public interface IDisplayAlarmRedisRepository : IDisplayAlarmRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; }
    }

    #endregion

    #region email alarm repository


    /// <summary>
    /// Specifies a general interface for a repository of email alerts
    /// </summary>
    public interface IEmailAlarmRepository :
        IReadRepository<EMAIL_ALARM, string, string>,
        IWriteRepository<EMAIL_ALARM, string>,
        IPaged<int>
    {

        /// <summary>
        /// Populates a sparse event entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse event entity to be populated</param>
        /// <returns>The populated event entity</returns>
        EMAIL_ALARM Hydrate(EMAIL_ALARM dry);

        /// <summary>
        /// Populates event entities with details from respective constituent entities
        /// </summary>
        /// <param name="dry">The sparse events entities to be populated</param>
        /// <returns>Populated event entities</returns>
        IEnumerable<EMAIL_ALARM> Hydrate(IEnumerable<EMAIL_ALARM> dry);

    }

    /// <summary>
    /// Specifies an interface for a repository of email alerts connected to an ORMlite source
    /// </summary>
    public interface IEmailAlarmOrmLiteRepository : IEmailAlarmRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IDbConnectionFactory DbConnectionFactory { get; }
    }


    /// <summary>
    /// Specifies an interface for a repository of email alerts connected to a NoSQL Redis source
    /// </summary>
    public interface IEmailAlarmRedisRepository : IEmailAlarmRepository
    {
        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        IRedisClientsManager RedisClientsManager { get; }
    }


    #endregion

}
