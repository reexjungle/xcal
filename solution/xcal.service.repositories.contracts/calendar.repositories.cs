using reexjungle.technical.data.contracts;
using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;

namespace reexjungle.xcal.service.repositories.contracts
{
    /// <summary>
    /// Specifies a contract for calendar repositories
    /// </summary>
    public interface ICalendarRepository :
        IReadRepository<VCALENDAR, Guid>,
        IWriteRepository<VCALENDAR, Guid>,
        IReadRepositoryKeys<Guid>
    {
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
}