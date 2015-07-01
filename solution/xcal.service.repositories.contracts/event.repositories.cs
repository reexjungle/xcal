using reexjungle.technical.data.contracts;
using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.repositories.contracts
{
    /// <summary>
    /// Specifies a general interface for a repository of events
    /// </summary>
    public interface IEventRepository :
        IReadRepository<VEVENT, Guid>,
        IWriteRepository<VEVENT, Guid>,
        IReadRepositoryKeys<Guid>
    {
        /// <summary>
        /// Populates a sparse event entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse event entity to be populated</param>
        /// <returns>The populated event entity</returns>
        VEVENT Hydrate(VEVENT dry);

        /// <summary>
        /// Populates events with aggregate entities
        /// </summary>
        /// <param name="dry">The sparse events entities to be populated</param>
        /// <returns>Populated event entities</returns>
        IEnumerable<VEVENT> HydrateAll(IEnumerable<VEVENT> dry);

        /// <summary>
        /// Depopulates aggregate entities from event
        /// </summary>
        /// <param name="full">The event to depopulate</param>
        /// <returns>Depopulated event</returns>
        VEVENT Dehydrate(VEVENT full);

        /// <summary>
        /// Depopulates aggregate entities from respective events
        /// </summary>
        /// <param name="full">The events to depopulate</param>
        /// <returns>Depopulated events</returns>
        IEnumerable<VEVENT> DehydrateAll(IEnumerable<VEVENT> full);
    }
}