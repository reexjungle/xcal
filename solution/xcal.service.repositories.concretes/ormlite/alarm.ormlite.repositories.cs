using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.io.concretes;
using reexjungle.infrastructure.operations.contracts;
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

namespace reexjungle.xcal.service.repositories.concretes.ormlite
{
    /// <summary>
    /// ORMLite repository for Audio Alarams
    /// </summary>
    public class AudioAlarmOrmLiteRepository : IAudioAlarmOrmLiteRepository, IDisposable
    {
        private IDbConnection conn = null;
        private IDbConnectionFactory factory = null;
        private IKeyGenerator<string> keygen;

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
        /// Gets the provider of identifiers
        /// </summary>
        /// <exception cref="System.ArgumentNullException">KeyGenerator</exception>
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
        /// Initializes a new instance of the <see cref="AudioAlarmOrmLiteRepository"/> class.
        /// </summary>
        public AudioAlarmOrmLiteRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioAlarmOrmLiteRepository"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public AudioAlarmOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public AUDIO_ALARM Find(string key)
        {
            using (var db = this.factory.OpenDbConnection())
            {
                try
                {
                    var dry = db.Select<AUDIO_ALARM>(q => q.Id == key).FirstOrDefault();
                    return dry != null ? this.Hydrate(dry) : dry;
                }
                catch (ArgumentNullException) { throw; }
                catch (InvalidOperationException) { throw; }
                catch (Exception) { throw; }
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
        public IEnumerable<AUDIO_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                var dry = db.Select<AUDIO_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<AUDIO_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                var dry = db.Select<AUDIO_ALARM>(skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(AUDIO_ALARM entity)
        {
            var attachbin = entity.AttachmentBinary;
            var attachuri = entity.AttachmentUri;
            try
            {
                db.Save(entity);
                if (attachbin != null)
                {
                    db.Save(attachbin);
                    var rattachbin = new REL_AALARMS_ATTACHBINS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        AlarmId = entity.Id,
                        AttachmentId = attachbin.Id
                    };
                    var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => q.AlarmId == entity.Id);
                    db.SynchronizeAll(rattachbin.ToSingleton(), orattachbins);
                }

                if (attachuri != null)
                {
                    db.Save(attachuri);
                    var rattachuri = new REL_AALARMS_ATTACHBINS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        AlarmId = entity.Id,
                        AttachmentId = attachbin.Id
                    };
                    var orattachuris = db.Select<REL_AALARMS_ATTACHBINS>(q => q.AlarmId == entity.Id);
                    db.SynchronizeAll(rattachuri.ToSingleton(), orattachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(string key)
        {
            try
            {
                db.Delete<AUDIO_ALARM>(q => q.Id == key);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository
        /// </summary>
        /// <param name="entities">The entities to save</param>
        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            var attachbins = entities.Where(x => x.AttachmentBinary != null).Select(x => x.AttachmentBinary);
            var attachuris = entities.Where(x => x.AttachmentUri != null).Select(x => x.AttachmentUri);
            var keys = entities.Select(x => x.Id).ToArray();

            try
            {
                db.SaveAll(entities.Distinct());
                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = entities.Where(x => x.AttachmentBinary != null).Select(x => new REL_AALARMS_ATTACHBINS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        AlarmId = x.Id,
                        AttachmentId = x.AttachmentBinary.Id
                    });
                    var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, keys));
                    db.SynchronizeAll(rattachbins, orattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = entities.Where(x => x.AttachmentUri != null).Select(x => new REL_AALARMS_ATTACHURIS
                    {
                        Id = this.KeyGenerator.GetNextKey(),
                        AlarmId = x.Id,
                        AttachmentId = x.AttachmentUri.Id
                    });
                    var orattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, keys));
                    db.SynchronizeAll(rattachuris, orattachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(AUDIO_ALARM source, Expression<Func<AUDIO_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Trigger,
                x.Duration,
                x.Repeat
            };

            Expression<Func<AUDIO_ALARM, object>> relations = x => new
            {
                x.AttachmentBinary,
                x.AttachmentUri
            };

            //3. Get list of selected relations
            var srelations = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //3. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            try
            {
                var okeys = (keys != null)
                    ? db.SelectParam<AUDIO_ALARM, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                    : db.SelectParam<AUDIO_ALARM>(q => q.Id).ToArray();

                if (!srelations.NullOrEmpty())
                {
                    Expression<Func<AUDIO_ALARM, object>> attachbinexpr = y => y.AttachmentBinary;
                    Expression<Func<AUDIO_ALARM, object>> attachuriexpr = y => y.AttachmentUri;

                    if (selection.Contains(attachbinexpr.GetMemberName()))
                    {
                        //get events-organizers relations
                        var attachbin = source.AttachmentBinary;
                        if (attachbin != null)
                        {
                            db.Save(attachbin);
                            var rattachbins = okeys.Select(x => new REL_AALARMS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                AlarmId = x,
                                AttachmentId = attachbin.Id
                            });
                            var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, okeys));
                            db.SynchronizeAll(rattachbins, orattachbins);
                        }
                    }

                    if (selection.Contains(attachuriexpr.GetMemberName()))
                    {
                        //get events-organizers relations
                        var attachuri = source.AttachmentBinary;
                        if (attachuri != null)
                        {
                            db.Save(attachuri);
                            var rattachuris = okeys.Select(x => new REL_AALARMS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                AlarmId = x,
                                AttachmentId = attachuri.Id
                            });
                            var orattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, okeys));
                            db.SynchronizeAll(rattachuris, orattachuris);
                        }
                    }
                }

                //4. Update matching event primitives
                if (!sprimitives.NullOrEmpty())
                {
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                    var patchexpr = patchstr.CompileToExpressionFunc<AUDIO_ALARM, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(AUDIO_ALARM).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                    if (!okeys.NullOrEmpty()) db.UpdateOnly<AUDIO_ALARM, object>(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                    else db.UpdateOnly<AUDIO_ALARM, object>(source, patchexpr);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<AUDIO_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<AUDIO_ALARM>();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
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
                return db.Count<AUDIO_ALARM>(q => q.Id == key) != 0;
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
                    return db.Count<AUDIO_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<AUDIO_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Populates a sparse audio alarm entity with details from its consitutent entities
        /// </summary>
        /// <param name="dry">The audio alarm entity to be populated</param>
        /// <returns>
        /// The populated audio alarm entity
        /// </returns>
        public AUDIO_ALARM Hydrate(AUDIO_ALARM dry)
        {
            var full = dry;
            try
            {
                var okey = db.SelectParam<AUDIO_ALARM, string>(q => q.Id, p => p.Id == dry.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(okey))
                {
                    var attachbins = db.Select<ATTACH_BINARY, AUDIO_ALARM, REL_AALARMS_ATTACHBINS>(
                        r => r.AttachmentId,
                        r => r.AlarmId,
                        a => a.Id == okey);
                    if (!attachbins.NullOrEmpty()) full.AttachmentBinary = attachbins.FirstOrDefault();

                    var attachuris = db.Select<ATTACH_URI, AUDIO_ALARM, REL_AALARMS_ATTACHURIS>(
                        r => r.AttachmentId,
                        r => r.AlarmId,
                        a => a.Id == okey);
                    if (!attachuris.NullOrEmpty()) full.AttachmentUri = attachuris.FirstOrDefault();
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            return full ?? dry;
        }

        /// <summary>
        /// Populates audio alarm entities with details from respective constituent entities
        /// </summary>
        /// <param name="dry">The sparse audio alarm entities to be populated</param>
        /// <returns>
        /// Populated audio alarm entities
        /// </returns>
        public IEnumerable<AUDIO_ALARM> HydrateAll(IEnumerable<AUDIO_ALARM> dry)
        {
            List<AUDIO_ALARM> full = null;
            try
            {
                full = dry.ToList();
                var keys = full.Select(q => q.Id).ToArray();
                var okeys = db.SelectParam<AUDIO_ALARM, string>(q => q.Id, p => Sql.In(p.Id, keys));

                if (!okeys.NullOrEmpty())
                {
                    #region 1. retrieve relationships

                    var rattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, okeys));
                    var rattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, okeys));

                    #endregion 1. retrieve relationships

                    #region 2. retrieve secondary entities

                    var attachbins = (!rattachbins.Empty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId).ToList())) : null;
                    var attachuris = (!rattachuris.Empty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId).ToList())) : null;

                    #endregion 2. retrieve secondary entities

                    #region 3. Use Linq to stitch secondary entities to primary entities

                    full.ForEach(x =>
                    {
                        if (!attachbins.NullOrEmpty())
                        {
                            var xattachbins = from y in attachbins
                                              join r in rattachbins on y.Id equals r.AttachmentId
                                              join a in full on r.AlarmId equals a.Id
                                              where a.Id == x.Id
                                              select y;
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinary = xattachbins.FirstOrDefault();
                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join a in full on r.AlarmId equals a.Id
                                              where a.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUri = xattachuris.FirstOrDefault();
                        }
                    });

                    #endregion 3. Use Linq to stitch secondary entities to primary entities
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
        }

        /// <summary>
        /// Depopulates aggregate entities from event
        /// </summary>
        /// <param name="full">The audio alarm entity to depopulate</param>
        /// <returns>
        /// Depopulated event
        /// </returns>
        public AUDIO_ALARM Dehydrate(AUDIO_ALARM full)
        {
            try
            {
                full.AttachmentBinary = null;
                full.AttachmentUri = null;
                return full;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Depopulates aggregate entities from respective events
        /// </summary>
        /// <param name="full">The audio alarm entities to depopulate</param>
        /// <returns>
        /// Depopulated events
        /// </returns>
        public IEnumerable<AUDIO_ALARM> Dehydrate(IEnumerable<AUDIO_ALARM> full)
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }
    }

    /// <summary>
    /// ORMLite repository for Display Alarams
    /// </summary>
    public class DisplayAlarmOrmLiteRepository : IDisplayAlarmOrmLiteRepository, IDisposable
    {
        private IDbConnectionFactory factory = null;
        private int? take = null;
        private IDbConnection conn = null;

        private IDbConnection db
        {
            get { return (this.conn) ?? (this.conn = factory.OpenDbConnection()); }
        }

        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        /// <exception cref="System.ArgumentNullException">DbConnectionFactory</exception>
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return this.factory; }
            set
            {
                if (value == null) throw new ArgumentNullException("DbConnectionFactory");
                this.factory = value;
            }
        }

        /// <summary>
        /// Gets or sets the provider of identifiers
        /// </summary>
        public IKeyGenerator<string> KeyGenerator { get; set; }

        /// <summary>
        /// Gets or sets the nummber of result rows to retrieve</param>
        /// <returns>
        /// </summary>
        /// <value>
        /// The number of results
        /// </value>
        /// <exception cref="System.ArgumentNullException">Take</exception>
        public int? Take
        {
            get { return this.take; }
            set
            {
                if (value == null) throw new ArgumentNullException("Take");
                this.take = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAlarmOrmLiteRepository"/> class.
        /// </summary>
        public DisplayAlarmOrmLiteRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAlarmOrmLiteRepository"/> class.
        /// </summary>
        /// <param name="factory">The database connection factory.</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        public DisplayAlarmOrmLiteRepository(IDbConnectionFactory factory, int? take)
        {
            this.DbConnectionFactory = factory;
            this.Take = take;
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public DISPLAY_ALARM Find(string key)
        {
            try
            {
                return db.Select<DISPLAY_ALARM>(q => q.Id == key).FirstOrDefault();
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
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
        public IEnumerable<DISPLAY_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                return db.Select<DISPLAY_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, Take);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                return db.Select<DISPLAY_ALARM>(skip, take);
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(DISPLAY_ALARM entity)
        {
            try
            {
                db.Save(entity);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository
        /// </summary>
        /// <param name="entities">The entities to save</param>
        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            try
            {
                db.SaveAll(entities);
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(DISPLAY_ALARM source, Expression<Func<DISPLAY_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<DISPLAY_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Trigger,
                x.Duration,
                x.Repeat,
                x.Description
            };

            //3. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            try
            {
                var okeys = (keys != null)
                    ? db.SelectParam<DISPLAY_ALARM, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                    : db.SelectParam<DISPLAY_ALARM>(q => q.Id).ToArray();

                if (!sprimitives.NullOrEmpty())
                {
                    //4. Update matching event primitives
                    var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                    var patchexpr = patchstr.CompileToExpressionFunc<DISPLAY_ALARM, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(DISPLAY_ALARM).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });
                    if (!okeys.NullOrEmpty()) db.UpdateOnly<DISPLAY_ALARM, object>(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                    else db.UpdateOnly<DISPLAY_ALARM, object>(source, patchexpr);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(string key)
        {
            try
            {
                db.Delete<DISPLAY_ALARM>(q => q.Id == key);
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty()) db.Delete<DISPLAY_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
                else db.DeleteAll<DISPLAY_ALARM>();
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
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
                return db.Count<DISPLAY_ALARM>(q => q.Id == key) != 0;
            }
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
                    return db.Count<DISPLAY_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<DISPLAY_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
            }
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

    public class EmailAlarmOrmLiteRepository : IEmailAlarmOrmLiteRepository, IDisposable
    {
        private IDbConnection conn = null;

        private IDbConnection db
        {
            get { return (this.conn) ?? (this.conn = factory.OpenDbConnection()); }
        }

        private IDbConnectionFactory factory = null;

        public IDbConnectionFactory DbConnectionFactory
        {
            get { return this.factory; }
            set
            {
                if (value == null) throw new ArgumentNullException("DbConnectionFactory");
                this.factory = value;
            }
        }

        public IKeyGenerator<string> KeyGenerator { get; set; }

        public EmailAlarmOrmLiteRepository()
        {
        }

        public EmailAlarmOrmLiteRepository(IDbConnectionFactory factory)
        {
            this.DbConnectionFactory = factory;
        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            EMAIL_ALARM full = null;
            try
            {
                full = db.Select<EMAIL_ALARM>(q => q.Id == dry.Id).FirstOrDefault();
                if (full != null)
                {
                    var attendees = db.Select<ATTENDEE, EMAIL_ALARM, REL_EALARMS_ATTENDEES>(
                        r => r.AttendeeId,
                        r => r.AlarmId,
                        a => a.Id == dry.Id);
                    if (!attendees.NullOrEmpty()) full.Attendees.AddRangeComplement(attendees);

                    var attachbins = db.Select<ATTACH_BINARY, EMAIL_ALARM, REL_EALARMS_ATTACHBINS>(
                        r => r.AttachmentId,
                        r => r.AlarmId,
                        a => a.Id == dry.Id);
                    if (!attachbins.NullOrEmpty()) full.AttachmentBinaries.AddRangeComplement(attachbins);

                    var attachuris = db.Select<ATTACH_URI, EMAIL_ALARM, REL_EALARMS_ATTACHURIS>(
                        r => r.AttachmentId,
                        r => r.AlarmId,
                        a => a.Id == dry.Id);
                    if (!attachuris.NullOrEmpty()) full.AttachmentUris.AddRangeComplement(attachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }

            return full ?? dry;
        }

        public IEnumerable<EMAIL_ALARM> HydrateAll(IEnumerable<EMAIL_ALARM> dry)
        {
            try
            {
                var keys = dry.Select(q => q.Id).ToArray();
                var full = db.Select<EMAIL_ALARM>(q => Sql.In(q.Id, keys));
                //var full = dry.ToList();
                // var okeys = db.SelectParam<VEVENT, string>(q => q.Id, p => Sql.In(p.Id, keys));

                //1. retrieve relationships
                if (!full.NullOrEmpty())
                {
                    var rattendees = db.Select<REL_EALARMS_ATTENDEES>(q => Sql.In(q.AlarmId, keys));
                    var rattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, keys));
                    var rattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, keys));

                    //2. retrieve secondary entities
                    var attendees = (!rattendees.NullOrEmpty()) ? db.Select<ATTENDEE>(q => Sql.In(q.Id, rattendees.Select(r => r.AttendeeId).ToArray())) : null;
                    var attachbins = (!rattachbins.NullOrEmpty()) ? db.Select<ATTACH_BINARY>(q => Sql.In(q.Id, rattachbins.Select(r => r.AttachmentId))) : null;
                    var attachuris = (!rattachuris.NullOrEmpty()) ? db.Select<ATTACH_URI>(q => Sql.In(q.Id, rattachuris.Select(r => r.AttachmentId))) : null;

                    //3. Use Linq to stitch secondary entities to primary entities
                    full.ForEach(x =>
                    {
                        if (!rattachbins.NullOrEmpty())
                        {
                            var xattachbins = from y in attachbins
                                              join r in rattachbins on y.Id equals r.AttachmentId
                                              join a in full on r.AlarmId equals a.Id
                                              where a.Id == x.Id
                                              select y;
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.AddRangeComplement(xattachbins);
                        }

                        if (!rattachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join a in full on r.AlarmId equals a.Id
                                              where a.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUris.AddRangeComplement(xattachuris);
                        }

                        if (!rattendees.NullOrEmpty())
                        {
                            var xattendees = from y in attendees
                                             join r in rattendees on y.Id equals r.AttendeeId
                                             join f in full on r.AlarmId equals f.Id
                                             where f.Id == x.Id
                                             select y;
                            if (!xattendees.NullOrEmpty()) x.Attendees.AddRangeComplement(xattendees);
                        }
                    });
                }

                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public EMAIL_ALARM Find(string key)
        {
            try
            {
                var dry = db.Select<EMAIL_ALARM>(q => q.Id == key).FirstOrDefault();
                return dry != null ? this.Hydrate(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                var dry = db.Select<EMAIL_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                var dry = db.Select<EMAIL_ALARM>(skip, take);
                return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void Save(EMAIL_ALARM entity)
        {
            try
            {
                //Save dry event entity i.a. without related details
                db.Save(entity);

                //1. retrieve entity details
                var attendees = entity.Attendees;
                var attachbins = entity.AttachmentBinaries;
                var attachuris = entity.AttachmentUris;

                //2. save details
                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees);
                    var rattendees = attendees.Select(x => new REL_EALARMS_ATTENDEES { AlarmId = entity.Id, AttendeeId = x.Id });
                    var orattendees = db.Select<REL_EALARMS_ATTENDEES>(q => q.Id == entity.Id && Sql.In(q.AttendeeId, attendees.Select(x => x.Id).ToArray()));
                    db.SynchronizeAll(rattendees, orattendees);
                }

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins);
                    var rattachbins = attachbins.Select(x => new REL_EALARMS_ATTACHBINS { AlarmId = entity.Id, AttachmentId = x.Id });
                    var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                    db.SynchronizeAll(rattachbins, orattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris);
                    var rattachuris = attachuris.Select(x => new REL_EALARMS_ATTACHURIS { AlarmId = entity.Id, AttachmentId = x.Id });
                    var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                    db.SynchronizeAll(rattachuris, orattachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void Erase(string key)
        {
            try
            {
                db.Delete<EMAIL_ALARM>(q => q.Id.ToUpper() == key.ToUpper());
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            try
            {
                db.SaveAll(entities);

                //1. retrieve details of events
                var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
                var attachbins = entities.Where(x => x.AttachmentBinaries.Count() > 0).SelectMany(x => x.AttachmentBinaries);
                var attachuris = entities.Where(x => x.AttachmentUris.Count() > 0).SelectMany(x => x.AttachmentUris);
                var keys = entities.Select(x => x.Id);

                //2. save details of events
                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                        .SelectMany(a => a.Attendees.Select(x => new REL_EALARMS_ATTENDEES
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            AlarmId = a.Id,
                            AttendeeId = x.Id
                        }));
                    var orattendees = db.Select<REL_EALARMS_ATTENDEES>(q => Sql.In(q.Id, keys));
                    db.SynchronizeAll(rattendees, orattendees);
                }

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty())
                        .SelectMany(a => a.AttachmentBinaries.Select(x => new REL_EALARMS_ATTACHBINS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            AlarmId = a.Id,
                            AttachmentId = x.Id
                        }));
                    var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => Sql.In(q.Id, keys));
                    db.SynchronizeAll(rattachbins, orattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty())
                        .SelectMany(a => a.AttachmentUris.Select(x => new REL_EALARMS_ATTACHURIS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            AlarmId = a.Id,
                            AttachmentId = x.Id
                        }));
                    var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => Sql.In(q.Id, keys));
                    db.SynchronizeAll(rattachuris, orattachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys)
        {
            try
            {
                db.Delete<EMAIL_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            catch (Exception) { throw; }
        }

        public void EraseAll()
        {
            try
            {
                db.DeleteAll<EMAIL_ALARM>();
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
            catch (Exception) { throw; }
        }

        public void Patch(EMAIL_ALARM source, Expression<Func<EMAIL_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<EMAIL_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Trigger,
                x.Duration,
                x.Repeat,
                x.Description,
                x.Summary
            };

            //3.Get list of all related event details (relation)
            Expression<Func<EMAIL_ALARM, object>> relations = x => new
            {
                x.Attendees,
                x.AttachmentBinaries,
                x.AttachmentUris
            };

            //4. Get list of selected relations
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Patch relations
            if (!srelation.NullOrEmpty())
            {
                Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                Expression<Func<EMAIL_ALARM, object>> attachbinsexpr = y => y.AttachmentBinaries;
                Expression<Func<EMAIL_ALARM, object>> attachurisexpr = y => y.AttachmentUris;

                using (var db = this.factory.OpenDbConnection())
                {
                    var okeys = (keys != null)
                ? db.SelectParam<EMAIL_ALARM, string>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                : db.SelectParam<EMAIL_ALARM>(q => q.Id).ToArray();

                    try
                    {
                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees;
                            if (!attendees.NullOrEmpty())
                            {
                                db.SaveAll(attendees);
                                var rattendees = okeys.SelectMany(x => attendees.Select(y => new REL_EALARMS_ATTENDEES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttendeeId = y.Id
                                }));
                                var orattendees = db.Select<REL_EALARMS_ATTENDEES>(q => Sql.In(q.AlarmId, okeys));
                                db.SynchronizeAll(rattendees, orattendees);
                            }
                        }

                        if (selection.Contains(attachbinsexpr.GetMemberName()))
                        {
                            var attachbins = source.AttachmentBinaries;
                            if (!attachbins.NullOrEmpty())
                            {
                                db.SaveAll(attachbins);
                                var rattachbins = okeys.SelectMany(x => attachbins.Select(y => new REL_EALARMS_ATTACHBINS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, okeys));
                                db.SynchronizeAll(rattachbins, orattachbins);
                            }
                        }

                        if (selection.Contains(attachurisexpr.GetMemberName()))
                        {
                            var attachuris = source.AttachmentBinaries;
                            if (!attachuris.NullOrEmpty())
                            {
                                db.SaveAll(attachuris);
                                var rattachuris = okeys.SelectMany(x => attachuris.Select(y => new REL_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, okeys));
                                db.SynchronizeAll(rattachuris, orattachuris);
                            }
                        }

                        //6. Get list of selected primitives
                        var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

                        //7. Update matching event primitives
                        if (!sprimitives.NullOrEmpty())
                        {
                            var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                            var patchexpr = patchstr.CompileToExpressionFunc<EMAIL_ALARM, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(EMAIL_ALARM).Assembly.Location, typeof(IContainsKey<string>).Assembly.Location });

                            if (!okeys.NullOrEmpty()) db.UpdateOnly<EMAIL_ALARM, object>(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                            else db.UpdateOnly<EMAIL_ALARM, object>(source, patchexpr);
                        }
                    }
                    catch (InvalidOperationException) { throw; }
                    catch (ApplicationException) { throw; }
                    catch (Exception) { throw; }
                }
            }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return db.Count<EMAIL_ALARM>(q => q.Id == key) != 0;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var dkeys = keys.Distinct().ToArray();
                if (mode == ExpectationMode.pessimistic || mode == ExpectationMode.unknown)
                    return db.Count<EMAIL_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
                else return db.Count<EMAIL_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
            }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public EMAIL_ALARM Dehydrate(EMAIL_ALARM full)
        {
            try
            {
                var dry = full;
                if (!dry.Attendees.NullOrEmpty()) dry.Attendees.Clear();
                if (!dry.AttachmentBinaries.NullOrEmpty()) dry.AttachmentBinaries.Clear();
                if (!dry.AttachmentUris.NullOrEmpty()) dry.AttachmentUris.Clear();
                return dry;
            }
            catch (ArgumentNullException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> DehydrateAll(IEnumerable<EMAIL_ALARM> full)
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

        public void Dispose()
        {
            if (this.conn != null) this.conn.Dispose();
        }
    }
}