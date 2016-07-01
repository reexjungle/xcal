using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.orm;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using reexjungle.xcal.infrastructure.serialization;
using xs = System.Transactions;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// ORMLite Repository for Calendars
    /// </summary>
    public class CalendarOrmRepository : ICalendarRepository, IOrmRepository
    {
        private readonly IDbConnectionFactory factory;
        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IEventRepository eventrepository;

        /// <summary>
        /// Gets the connection factory of ORMLite datasources
        /// </summary>
        public IDbConnectionFactory DbConnectionFactory => factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOrmRepository"/> class.
        /// </summary>
        /// <param name="keygenerator">The generator for generating keys</param>
        /// <param name="eventrepository">The repository for calendar events</param>
        /// <param name="factory">The database connection factory</param>
        public CalendarOrmRepository(IKeyGenerator<Guid> keygenerator, IEventRepository eventrepository, IDbConnectionFactory factory)
        {
            if (keygenerator == null) throw new ArgumentNullException(nameof(keygenerator));
            if (eventrepository == null) throw new ArgumentNullException(nameof(eventrepository));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            this.keygenerator = keygenerator;
            this.eventrepository = eventrepository;
            this.factory = factory;
        }

        /// <summary>
        /// Populates a sparse calendar entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The sparse calendar entity to be populated</param>
        /// <returns>
        /// The populated calendar entity
        /// </returns>
        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            using (var db = factory.OpenDbConnection())
            {
                var okey = db.SelectParam<VCALENDAR, Guid>(q => q.Id, p => p.Id == dry.Id).FirstOrDefault();
                if (okey != Guid.Empty)
                {
                    var revents = db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == okey);
                    if (!revents.NullOrEmpty())
                    {
                        var events = eventrepository.FindAll(revents.Select(x => x.EventId).ToList());
                        dry.Events.MergeRange(eventrepository.HydrateAll(events));
                    }
                }

                return dry; 
            }
        }

        /// <summary>
        /// Populates respective calendars with aggregate entities
        /// </summary>
        /// <param name="dry">The sparse calendar entities to be populated</param>
        /// <returns>
        /// Populated calendar entities
        /// </returns>
        public IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry)
        {
            using (var db = factory.OpenDbConnection())
            {
                var full = dry.ToList();
                var keys = full.Select(q => q.Id).ToArray();
                //var okeys = db.SelectParam<VCALENDAR, string>(q => q.Id, p => Sql.In(p.Id, keys));
                if (!keys.NullOrEmpty())
                {
                    var revents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                    if (!revents.NullOrEmpty())
                    {
                        var events = eventrepository.FindAll(revents.Select(x => x.EventId)).ToList();
                        full.ForEach(x =>
                        {
                            var xevents = from y in events
                                          join r in revents on y.Id equals r.EventId
                                          join c in full on r.CalendarId equals c.Id
                                          where c.Id == x.Id
                                          select y;
                            if (!xevents.NullOrEmpty()) x.Events.MergeRange(xevents);
                        });
                    }
                }
                return full; 
            }
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public VCALENDAR Find(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                var dry = db.Select<VCALENDAR>(q => q.Id == key).FirstOrDefault();
                return dry != null ? Hydrate(dry) : dry; 
            }
        }

        /// <summary>
        /// Finds entities in the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">Unique identifiers for retrieving the entities</param>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns>
        /// Found entities from the repository
        /// </returns>
        public IEnumerable<VCALENDAR> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            IEnumerable<VCALENDAR> dry;
            using (var db = factory.OpenDbConnection())
            {
                if (skip == null && take == null) dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                else dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()), skip, take); 
            }
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            IEnumerable<VCALENDAR> dry;
            using (var db = factory.OpenDbConnection())
            {
                if (skip == null && take == null) dry = db.Select<VCALENDAR>();
                else dry = db.Select<VCALENDAR>(skip, take); 
            }
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(VCALENDAR entity)
        {
            using (var db = factory.OpenDbConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Save(entity, transaction);
                        if (!entity.Events.NullOrEmpty())
                        {
                            eventrepository.SaveAll(entity.Events.Distinct());
                            var revents = entity.Events.Select(x => new REL_CALENDARS_EVENTS
                            {
                                Id = keygenerator.GetNext(),
                                CalendarId = entity.Id,
                                EventId = x.Id
                            });
                            var orevents = db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == entity.Id);
                            db.MergeAll<REL_CALENDARS_EVENTS, Guid>(revents, orevents, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (ArgumentNullException) { transaction.Rollback(); throw; }
                    catch (InvalidOperationException) { transaction.Rollback(); throw; }
                    catch (ApplicationException) { transaction.Rollback(); throw; }
                } 
            }
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(VCALENDAR source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            Expression<Func<VCALENDAR, object>> primitives = x => new
            {
                x.Version,
                x.Method,
                x.Calscale
            };

            Expression<Func<VCALENDAR, object>> relations = x => new
            {
                x.Events,
                x.ToDos,
                x.FreeBusies,
                x.Journals,
                x.TimeZones,
            };

            var selection = fields as IList<string> ?? fields.ToList();

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            using (var db = factory.OpenDbConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var okeys = (!keys.NullOrEmpty())
                            ? db.SelectParam<VCALENDAR, Guid>(q => q.Id, p => Sql.In(p.Id, keys))
                            : db.SelectParam<VCALENDAR, Guid>(q => q.Id);
                        if (!srelation.NullOrEmpty())
                        {
                            Expression<Func<VCALENDAR, object>> eventsexpr = y => y.Events;
                            if (selection.Contains(eventsexpr.GetMemberName()))
                            {
                                eventrepository.SaveAll(source.Events.Distinct());
                                var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, okeys));
                                if (!source.Events.NullOrEmpty())
                                {
                                    var revents =
                                        okeys.SelectMany(x => source.Events.Select(y => new REL_CALENDARS_EVENTS
                                        {
                                            Id = keygenerator.GetNext(),
                                            CalendarId = x,
                                            EventId = y.Id
                                        }));
                                    db.RemoveAll(orevents, transaction);
                                    db.SaveAll(revents, transaction);
                                }
                                else db.RemoveAll(orevents, transaction);
                            }
                        }

                        if (sprimitives.Any())
                        {
                            var patchstr = $"f => new {{ {string.Join(", ", sprimitives.Select(x => $"f.{x}"))} }}";

                            var patchexpr = patchstr.CompileToExpressionFunc<VCALENDAR, object>(
                                CodeDomLanguage.csharp,
                                "System.dll", 
                                "System.Core.dll", 
                                typeof(CalendarWriter).Assembly.Location,
                                typeof (VCALENDAR).Assembly.Location,
                                typeof (IContainsKey<Guid>).Assembly.Location);

                            if (okeys.Any())
                                db.UpdateOnly(source, patchexpr, q => Sql.In(q.Id, okeys));
                            else
                                db.UpdateOnly(source, patchexpr);
                        }

                        transaction.Commit();
                    }
                    catch (ArgumentNullException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (InvalidOperationException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (ApplicationException)
                    {
                        transaction.Rollback(); throw;
                    }
                } 
            }
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Delete<VCALENDAR>(q => q.Id == key);
                        transaction.Commit();
                    }
                    catch (ArgumentNullException) { transaction.Rollback(); throw; }
                    catch (InvalidOperationException) { transaction.Rollback(); throw; }
                    catch (ApplicationException) { transaction.Rollback(); throw; }
                } 
            }
        }

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository
        /// </summary>
        /// <param name="entities">The entities to save</param>
        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            using (var db = factory.OpenDbConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var keys = entities.Select(x => x.Id).ToArray();
                        db.SaveAll(entities.Distinct(), transaction);

                        var events = entities.Where(x => !x.Events.NullOrEmpty()).SelectMany(x => x.Events);
                        if (!events.NullOrEmpty())
                        {
                            eventrepository.SaveAll(events.Distinct());
                            var revents =
                                entities.Where(x => !x.Events.NullOrEmpty())
                                    .SelectMany(c => c.Events.Select(x => new REL_CALENDARS_EVENTS
                                    {
                                        Id = keygenerator.GetNext(),
                                        CalendarId = c.Id,
                                        EventId = x.Id
                                    })).ToList();
                            var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                            db.MergeAll(revents, orevents, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (ArgumentNullException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (InvalidOperationException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (ApplicationException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                } 
            }
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        if (!keys.NullOrEmpty()) db.Delete<VCALENDAR>(q => Sql.In(q.Id, keys));
                        else db.DeleteAll<VCALENDAR>();

                        transaction.Commit();
                    }
                    catch (ArgumentNullException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (InvalidOperationException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    catch (ApplicationException)
                    {
                        transaction.Rollback(); 
                        throw;
                    }
                } 
            }
        }

        /// <summary>
        /// Checks if the repository contains an entity
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        /// <returns>
        /// True if the entity is found in the repository, otherwise false
        /// </returns>
        public bool ContainsKey(Guid key)
        {
            using (var db = factory.OpenDbConnection())
            {
                return db.Count<VCALENDAR>(q => q.Id == key) != 0; 
            }
        }

        /// <summary>
        /// Checks if the repository contains entities
        /// </summary>
        /// <param name="keys">The unique identifiers of the entities</param>
        /// <param name="mode">How the search is performed. Optimistic if at least one entity found, Pessimistic if all entities are found</param>
        /// <returns>
        /// True if the entities are found, otherwise false
        /// </returns>
        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var dkeys = keys.Distinct().ToArray();
            using (var db = factory.OpenDbConnection())
            {
                if (mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown)
                    return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) != 0; 
            }
        }

        /// <summary>
        /// Depopulates aggregate entities from respective calendars.
        /// </summary>
        /// <param name="full">The full.</param>
        /// <returns></returns>
        public IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full)
        {
            var calendars = full.ToList();
            foreach (var vcalendar in calendars)
            {
                Dehydrate(vcalendar);
            }

            return calendars;
        }

        /// <summary>
        /// Depopulates aggregate entities from a calendar
        /// </summary>
        /// <param name="full">The calendar to be depopulated</param>
        /// <returns>
        /// A depopulated calendar
        /// </returns>
        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            var dry = full;
            if (!dry.Events.NullOrEmpty()) dry.Events.Clear();
            if (!dry.ToDos.NullOrEmpty()) dry.ToDos.Clear();
            if (!dry.Journals.NullOrEmpty()) dry.Journals.Clear();
            if (!dry.TimeZones.NullOrEmpty()) dry.TimeZones.Clear();
            return dry;
        }

        /// <summary>
        /// Gets the keys from the non-relational repository
        /// </summary>
        /// <param name="skip">The number of results to skip</param>
        /// <param name="take">The number of results per page to retrieve</param>
        /// <returns></returns>
        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            using (var db = factory.OpenDbConnection())
            {
                return db.SelectParam<VCALENDAR, Guid>(q => q.Id, skip, take); 
            }
        }

    }
}