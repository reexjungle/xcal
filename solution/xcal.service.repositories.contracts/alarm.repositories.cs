using reexjungle.technical.data.contracts;
using reexjungle.xcal.domain.models;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.repositories.contracts
{
    #region audio alarm repository

    /// <summary>
    /// Specifies a general interface for a repository of audio alerts
    /// </summary>
    public interface IAudioAlarmRepository :
        IReadRepository<AUDIO_ALARM, Guid>,
        IWriteRepository<AUDIO_ALARM, Guid>
    {
        /// <summary>
        /// Populates a sparse email alarm entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse email alarm entity to be populated</param>
        /// <returns>The populated alarm entity</returns>
        AUDIO_ALARM Hydrate(AUDIO_ALARM dry);

        /// <summary>
        /// Populates email alarm entities with details from respective constituent entities
        /// </summary>
        /// <param name="dry">The sparse email alarm entities to be populated</param>
        /// <returns>Populated alarm entities</returns>
        IEnumerable<AUDIO_ALARM> HydrateAll(IEnumerable<AUDIO_ALARM> dry);

        /// <summary>
        /// Depopulates aggregate entities from email alarm
        /// </summary>
        /// <param name="full">The email alarm entity to depopulate</param>
        /// <returns>Depopulated alarm</returns>
        AUDIO_ALARM Dehydrate(AUDIO_ALARM full);

        /// <summary>
        /// Depopulates aggregate entities from respective alarms
        /// </summary>
        /// <param name="full">The audio alarm entities to depopulate</param>
        /// <returns>Depopulated alarms</returns>
        IEnumerable<AUDIO_ALARM> DehydrateAll(IEnumerable<AUDIO_ALARM> full);
    }

    #endregion audio alarm repository

    #region display alarm repository

    /// <summary>
    /// Specifies a general interface for a repository of display alerts
    /// </summary>
    public interface IDisplayAlarmRepository :
        IReadRepository<DISPLAY_ALARM, Guid>,
        IWriteRepository<DISPLAY_ALARM, Guid> { }

    #endregion display alarm repository

    #region email alarm repository

    /// <summary>
    /// Specifies a general interface for a repository of email alerts
    /// </summary>
    public interface IEmailAlarmRepository :
        IReadRepository<EMAIL_ALARM, Guid>,
        IWriteRepository<EMAIL_ALARM, Guid>
    {
        /// <summary>
        /// Populates a sparse email alarm entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse email alarm entity to be populated</param>
        /// <returns>The populated alarm entity</returns>
        EMAIL_ALARM Hydrate(EMAIL_ALARM dry);

        /// <summary>
        /// Populates email alarm entities with details from respective constituent entities
        /// </summary>
        /// <param name="dry">The sparse email alarm entities to be populated</param>
        /// <returns>Populated alarm entities</returns>
        IEnumerable<EMAIL_ALARM> HydrateAll(IEnumerable<EMAIL_ALARM> dry);

        /// <summary>
        /// Depopulates aggregate entities from email alarm
        /// </summary>
        /// <param name="full">The email alarm entity to depopulate</param>
        /// <returns>Depopulated alarm</returns>
        EMAIL_ALARM Dehydrate(EMAIL_ALARM full);

        /// <summary>
        /// Depopulates aggregate entities from respective alarms
        /// </summary>
        /// <param name="full">The audio alarm entities to depopulate</param>
        /// <returns>Depopulated alarms</returns>
        IEnumerable<EMAIL_ALARM> DehydrateAll(IEnumerable<EMAIL_ALARM> full);
    }

    #endregion email alarm repository
}