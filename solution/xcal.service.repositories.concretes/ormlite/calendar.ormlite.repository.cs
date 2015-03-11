using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.contracts;
using reexjungle.infrastructure.io.concretes;
using reexjungle.technical.data.concretes.extensions.ormlite;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using xs = System.Transactions;

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// ORMLite Repository for Calendars
    /// </summary>
    public class CalendarOrmLiteRepository : ICalendarOrmLiteRepository, IDisposable
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private IKeyGenerator<string> keygen;
        private IEventRepository eventrepository;

        private IDbConnection db
        {
            get { return (this.conn) ?? (this.conn = factory.OpenDbConnection()); }
        }

        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Null factory</exception>
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return this.factory; }
            set
            {
                if (value == null) throw new ArgumentNullException("Null factory");
                this.factory = value;
            }
        }

        /// <summary>
        /// Gets or sets the provider of identifiers
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Null KeyGenerator</exception>
        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        /// <summary>
        /// Gets or sets the repository of addressing the event aggregate root
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Null EventRepository</exception>
        public IEventRepository EventRepository
        {
            get { return this.eventrepository; }
            set
            {
                if (value == null) throw new ArgumentNullException("EventRepository");
                this.eventrepository = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOrmLiteRepository"/> class.
        /// </summary>
        public CalendarOrmLiteRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOrmLiteRepository"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public CalendarOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
            this.conn = this.factory.OpenDbConnection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarOrmLiteRepository"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        public CalendarOrmLiteRepository(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            this.conn = connection;
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
            VCALENDAR full = dry;
            try
            {
                var okey = db.SelectParam<VCALENDAR, string>(q => q.Id, p => p.Id == dry.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(okey))
                {
                    var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == okey);
                    if (!revents.NullOrEmpty())
                    {
                        var events = this.EventRepository.FindAll(revents.Select(x => x.EventId).ToList());
                        full.Events.MergeRange(this.EventRepository.HydrateAll(events));
                    }
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
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
            var full = dry.ToList();
            try
            {
                var keys = full.Select(q => q.Id).ToArray();
                //var okeys = db.SelectParam<VCALENDAR, string>(q => q.Id, p => Sql.In(p.Id, keys));
                if (!keys.NullOrEmpty())
                {
                    var revents = this.db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                    if (!revents.NullOrEmpty())
                    {
                        var events = this.EventRepository.FindAll(revents.Select(x => x.EventId)).ToList();
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
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            return full ?? dry;
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public VCALENDAR Find(string key)
        {
            try
            {
                var dry = db.Select<VCALENDAR>(q => q.Id == key).FirstOrDefault();
                return dry != null ? this.Hydrate(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
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
        public IEnumerable<VCALENDAR> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                IEnumerable<VCALENDAR> dry = null;
                if (skip == null && take == null) dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                else dry = db.Select<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()), skip.Value, take.Value);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            try
            {
                IEnumerable<VCALENDAR> dry = null;
                if (skip == null && take == null) dry = db.Select<VCALENDAR>();
                else dry = db.Select<VCALENDAR>(skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(VCALENDAR entity)
        {
            using (var transaction = this.db.BeginTransaction())
            {
                try
                {
                    db.Save<VCALENDAR>(entity, transaction);
                    if (!entity.Events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(entity.Events.Distinct());
                        var revents = entity.Events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            CalendarId = entity.Id,
                            EventId = x.Id
                        });
                        var orevents = db.Select<REL_CALENDARS_EVENTS>(q => q.CalendarId == entity.Id);
                        db.MergeAll(revents, orevents, transaction);
                    }

                    transaction.Commit();
                }
                catch (ArgumentNullException) { transaction.Rollback(); throw; }
                catch (InvalidOperationException) { transaction.Rollback(); throw; }
                catch (ApplicationException) { transaction.Rollback(); throw; }
            }
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames().ToArray();

            Expression<Func<VCALENDAR, object>> primitives = x => new
            {
                x.Version,
                x.Method,
                x.Calscale
            };

            Expression<Func<VCALENDAR, object>> relations = x => new
            {
                Events = x.Events,
                Todos = x.ToDos,
                FreeBusies = x.FreeBusies,
                Journals = x.Journals,
                TimeZones = x.TimeZones,
                IanaComponents = x.IanaComponents,
                XComponents = x.XComponents
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            using (var transaction = this.db.BeginTransaction())
            {
                try
                {
                    var okeys = (!keys.NullOrEmpty())
                        ? db.SelectParam<VCALENDAR, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray()))
                        : db.SelectParam<VCALENDAR>(q => q.Id);
                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<VCALENDAR, object>> eventsexpr = y => y.Events;
                        if (selection.Contains(eventsexpr.GetMemberName()))
                        {
                            this.EventRepository.SaveAll(source.Events.Distinct());
                            if (!source.Events.NullOrEmpty())
                            {
                                var revents = okeys.SelectMany(x => source.Events.Select(y => new REL_CALENDARS_EVENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    CalendarId = x,
                                    EventId = y.Id
                                }));
                                var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, okeys));
                                db.MergeAll(revents, orevents, transaction);
                            }
                        }
                    }

                    if (!sprimitives.NullOrEmpty())
                    {
                        var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));

                        var patchexpr = patchstr.CompileToExpressionFunc<VCALENDAR, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(VCALENDAR).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                        if (!okeys.NullOrEmpty()) db.UpdateOnly<VCALENDAR, object>(source, patchexpr, q => Sql.In(q.Id, okeys));
                        else db.UpdateOnly<VCALENDAR, object>(source, patchexpr);
                    }

                    transaction.Commit();
                }
                catch (ArgumentNullException) { transaction.Rollback(); throw; }
                catch (InvalidOperationException) { transaction.Rollback(); throw; }
                catch (ApplicationException) { transaction.Rollback(); throw; }
            }
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(string key)
        {
            using (var transaction = this.db.BeginTransaction())
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

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository
        /// </summary>
        /// <param name="entities">The entities to save</param>
        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            using (var transaction = this.db.BeginTransaction())
            {
                try
                {
                    var keys = entities.Select(x => x.Id).ToArray();
                    db.SaveAll(entities.Distinct(), transaction);

                    var events = entities.Where(x => !x.Events.NullOrEmpty()).SelectMany(x => x.Events);
                    if (!events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(events.Distinct());
                        var revents = entities.Where(x => !x.Events.NullOrEmpty()).SelectMany(c => c.Events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            CalendarId = c.Id,
                            EventId = x.Id
                        }));
                        var orevents = db.Select<REL_CALENDARS_EVENTS>(q => Sql.In(q.CalendarId, keys));
                        db.MergeAll(revents, orevents, transaction);
                    }

                    transaction.Commit();
                }
                catch (ArgumentNullException) { transaction.Rollback(); throw; }
                catch (InvalidOperationException) { transaction.Rollback(); throw; }
                catch (ApplicationException) { transaction.Rollback(); throw; }
            }
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<string> keys = null)
        {
            using (var transaction = this.db.BeginTransaction())
            {
                try
                {
                    if (!keys.NullOrEmpty()) db.Delete<VCALENDAR>(q => Sql.In(q.Id, keys.ToArray()));
                    else db.DeleteAll<VCALENDAR>();

                    transaction.Commit();
                }
                catch (ArgumentNullException) { transaction.Rollback(); throw; }
                catch (InvalidOperationException) { transaction.Rollback(); throw; }
                catch (ApplicationException) { transaction.Rollback(); throw; }
            }
        }

        /// <summary>
        /// Checks if the repository contains an entity
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        /// <returns>
        /// True if the entity is found in the repository, otherwise false
        /// </returns>
        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<VCALENDAR>(q => q.Id == key) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Checks if the repository contains entities
        /// </summary>
        /// <param name="keys">The unique identifiers of the entities</param>
        /// <param name="mode">How the search is performed. Optimistic if at least one entity found, Pessimistic if all entities are found</param>
        /// <returns>
        /// True if the entities are found, otherwise false
        /// </returns>
        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var dkeys = keys.Distinct().ToArray();
                if (mode == ExpectationMode.pessimistic || mode == ExpectationMode.unknown)
                    return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<VCALENDAR>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Depopulates aggregate entities from respective calendars.
        /// </summary>
        /// <param name="full">The full.</param>
        /// <returns></returns>
        public IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full)
        {
            try
            {
                var pquery = full.AsParallel();
                pquery.ForAll(x => this.Dehydrate(x));
                return pquery.AsEnumerable();
            }
            catch (ArgumentNullException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (AggregateException) { throw; }
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
            try
            {
                var dry = full;
                if (!dry.Events.NullOrEmpty()) dry.Events.Clear();
                if (!dry.ToDos.NullOrEmpty()) dry.ToDos.Clear();
                if (!dry.Journals.NullOrEmpty()) dry.Journals.Clear();
                if (!dry.TimeZones.NullOrEmpty()) dry.TimeZones.Clear();
                if (!dry.IanaComponents.NullOrEmpty()) dry.IanaComponents.Clear();
                if (!dry.XComponents.NullOrEmpty()) dry.XComponents.Clear();
                return dry;
            }
            catch (ArgumentNullException) { throw; }
        }

        /// <summary>
        /// Gets the keys from the non-relational repository
        /// </summary>
        /// <param name="skip">The number of results to skip</param>
        /// <param name="take">The number of results per page to retrieve</param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            try
            {
                return db.SelectParam<VCALENDAR>(q => q.Id, skip, take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }
    }
}