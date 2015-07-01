using reexjungle.technical.data.concretes.extensions.ormlite;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using reexjungle.xmisc.infrastructure.concretes.io;
using reexjungle.xmisc.infrastructure.contracts;
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
    public class AudioAlarmOrmLiteRepository : IAudioAlarmRepository, IOrmLiteRepository, IDisposable
    {
        private IDbConnection conn;
        private readonly IDbConnectionFactory factory;
        private readonly IKeyGenerator<Guid> keygenerator;

        private IDbConnection db
        {
            get { return (conn) ?? (conn = factory.OpenDbConnection()); }
        }

        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Null factory</exception>
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return factory; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioAlarmOrmLiteRepository"/> class.
        /// </summary>
        public AudioAlarmOrmLiteRepository(IKeyGenerator<Guid> keygenerator, IDbConnectionFactory factory)
        {
            if (keygenerator == null) throw new ArgumentNullException("keygenerator");
            if (factory == null) throw new ArgumentNullException("factory");

            this.keygenerator = keygenerator;
            this.factory = factory;
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public AUDIO_ALARM Find(Guid key)
        {
            var dry = db.Select<AUDIO_ALARM>(q => q.Id == key);
            return !dry.NullOrEmpty() ? Hydrate(dry.First()) : dry.First();
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
        public IEnumerable<AUDIO_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            var dry = db.Select<AUDIO_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<AUDIO_ALARM> Get(int? skip = null, int? take = null)
        {
            var dry = db.Select<AUDIO_ALARM>(skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(AUDIO_ALARM entity)
        {
            db.Save(entity);

            var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => q.Id == entity.Id);
            var orattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => q.Id == entity.Id);

            if (entity.AttachmentBinary != null)
            {
                db.Save(entity.AttachmentBinary);
                var rattachbin = new REL_AALARMS_ATTACHBINS
                {
                    Id = keygenerator.GetNext(),
                    AlarmId = entity.Id,
                    AttachmentId = entity.AttachmentBinary.Id
                };

                db.MergeAll(rattachbin.ToSingleton(), orattachbins);
            }
            else db.RemoveAll(orattachbins);

            if (entity.AttachmentUri != null)
            {
                db.Save(entity.AttachmentUri);
                var rattachuri = new REL_AALARMS_ATTACHBINS
                {
                    Id = keygenerator.GetNext(),
                    AlarmId = entity.Id,
                    AttachmentId = entity.AttachmentUri.Id
                };

                db.MergeAll(rattachuri.ToSingleton(), orattachbins);
            }
            else db.RemoveAll(orattachuris);
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(Guid key)
        {
            db.Delete<AUDIO_ALARM>(q => q.Id == key);
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

            db.SaveAll(entities.Distinct());
            var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.Id, keys));
            var orattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.Id, keys));

            if (!attachbins.NullOrEmpty())
            {
                db.SaveAll(attachbins.Distinct());
                var rattachbins = entities.Where(x => x.AttachmentBinary != null)
                    .Select(x => new REL_AALARMS_ATTACHBINS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = x.Id,
                        AttachmentId = x.AttachmentBinary.Id
                    });
                db.MergeAll(rattachbins, orattachbins);
            }
            else db.RemoveAll(orattachbins);

            if (!attachuris.NullOrEmpty())
            {
                db.SaveAll(attachuris.Distinct());
                var rattachuris = entities.Where(x => x.AttachmentUri != null)
                    .Select(x => new REL_AALARMS_ATTACHURIS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = x.Id,
                        AttachmentId = x.AttachmentUri.Id
                    });
                db.MergeAll(rattachuris, orattachuris);
            }
            else db.RemoveAll(orattachuris);
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(AUDIO_ALARM source, Expression<Func<AUDIO_ALARM, object>> fields, IEnumerable<Guid> keys = null)
        {
            var okeys = (keys != null)
                ? db.SelectParam<AUDIO_ALARM>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                : db.SelectParam<AUDIO_ALARM>(q => q.Id).ToArray();

            //1. Get fields slected for patching
            var selection = fields.GetMemberNames();

            //2.Get list of all non-related event details (primitives)
            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Trigger,
                x.Duration,
                x.Repeat,
            };

            Expression<Func<AUDIO_ALARM, object>> relations = x => new
            {
                x.AttachmentBinary,
                x.AttachmentUri
            };

            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);
            var srelations = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //4. Patch relations
            if (!srelations.NullOrEmpty())
            {
                Expression<Func<AUDIO_ALARM, object>> attachbinexpr = x => x.AttachmentBinary;
                Expression<Func<AUDIO_ALARM, object>> attachuriexpr = x => x.AttachmentUri;

                if (srelations.Contains(attachbinexpr.GetMemberName()))
                {
                    var orattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, okeys));
                    if (source.AttachmentBinary != null)
                    {
                        db.Save(source.AttachmentBinary);
                        var rattachbins = okeys.Select(x => new REL_AALARMS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = x,
                            AttachmentId = source.AttachmentBinary.Id
                        });
                        db.MergeAll(rattachbins, orattachbins);
                    }
                    else db.RemoveAll(orattachbins);
                }
                if (srelations.Contains(attachuriexpr.GetMemberName()))
                {
                    var orattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, okeys));
                    if (source.AttachmentUri != null)
                    {
                        db.Save(source.AttachmentUri);
                        var rattachuris = okeys.Select(x => new REL_AALARMS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = x,
                            AttachmentId = source.AttachmentUri.Id
                        });
                        db.MergeAll(rattachuris, orattachuris);
                    }
                    else db.RemoveAll(orattachuris);
                }
            }

            if (!sprimitives.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<AUDIO_ALARM, object>(CodeDomLanguage.csharp, new string[] { "System.dll", "System.Core.dll", typeof(AUDIO_ALARM).Assembly.Location, typeof(IContainsKey<Guid>).Assembly.Location });

                if (!okeys.NullOrEmpty()) db.UpdateOnly<AUDIO_ALARM, object>(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                else db.UpdateOnly<AUDIO_ALARM, object>(source, patchexpr);
            }
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            var keylist = keys as IList<Guid> ?? keys.ToList();
            if (!keylist.NullOrEmpty()) db.Delete<AUDIO_ALARM>(q => Sql.In(q.Id, keylist));
            else db.DeleteAll<AUDIO_ALARM>();
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
            return db.Count<AUDIO_ALARM>(q => q.Id == key) != 0;
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
            if (mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown)
                return db.Count<AUDIO_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
            return db.Count<AUDIO_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (conn != null) conn.Dispose();
        }

        public AUDIO_ALARM Hydrate(AUDIO_ALARM dry)
        {
            var full = dry;
            var found = db.Select<AUDIO_ALARM>(q => q.Id == dry.Id);
            if (!found.NullOrEmpty())
            {
                var attachbins = db.Select<ATTACH_BINARY, AUDIO_ALARM, REL_AALARMS_ATTACHBINS>(
                    r => r.AttachmentId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id);
                if (!attachbins.NullOrEmpty()) full.AttachmentBinary = attachbins.First();

                var attachuris = db.Select<ATTACH_URI, AUDIO_ALARM, REL_AALARMS_ATTACHURIS>(
                    r => r.AttachmentId,
                    r => r.AlarmId,
                    a => a.Id == dry.Id);
                if (!attachuris.NullOrEmpty()) full.AttachmentUri = attachuris.First();
            }

            return full ?? dry;
        }

        public IEnumerable<AUDIO_ALARM> HydrateAll(IEnumerable<AUDIO_ALARM> dry)
        {
            var keys = dry.Select(q => q.Id).ToArray();
            var full = db.Select<AUDIO_ALARM>(q => Sql.In(q.Id, keys));

            //1. retrieve relationships
            if (!full.NullOrEmpty())
            {
                var rattachbins = db.Select<REL_AALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, keys));
                var rattachuris = db.Select<REL_AALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, keys));

                //2. retrieve secondary entities
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
                        if (!xattachbins.NullOrEmpty()) x.AttachmentBinary = xattachbins.First();
                    }

                    if (!rattachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join a in full on r.AlarmId equals a.Id
                                          where a.Id == x.Id
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.AttachmentUri = xattachuris.First();
                    }
                });
            }

            return full ?? dry;
        }

        public AUDIO_ALARM Dehydrate(AUDIO_ALARM full)
        {
            full.AttachmentBinary = null;
            full.AttachmentUri = null;
            return full;
        }

        public IEnumerable<AUDIO_ALARM> DehydrateAll(IEnumerable<AUDIO_ALARM> full)
        {
            var pquery = full.AsParallel();
            pquery.ForAll(x => Dehydrate(x));
            return pquery.AsEnumerable();
        }
    }

    /// <summary>
    /// ORMLite repository for Display Alarams
    /// </summary>
    public class DisplayAlarmOrmLiteRepository : IDisplayAlarmRepository, IOrmLiteRepository, IDisposable
    {
        private readonly IDbConnectionFactory factory;
        private IDbConnection conn;

        private IDbConnection db
        {
            get { return (conn) ?? (conn = factory.OpenDbConnection()); }
        }

        /// <summary>
        /// Gets or sets the connection factory of ORMLite datasources
        /// </summary>
        /// <exception cref="System.ArgumentNullException">DbConnectionFactory</exception>
        public IDbConnectionFactory DbConnectionFactory
        {
            get { return factory; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAlarmOrmLiteRepository"/> class.
        /// </summary>
        public DisplayAlarmOrmLiteRepository(IDbConnectionFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            this.factory = factory;
        }

        /// <summary>
        /// Finds an entity in the repository based on a unique identifier
        /// </summary>
        /// <param name="key">Type of unique identifier for retrieval of entity from repository</param>
        /// <returns>
        /// The found entity from the repository
        /// </returns>
        public DISPLAY_ALARM Find(Guid key)
        {
            return db.Select<DISPLAY_ALARM>(q => q.Id == key).FirstOrDefault();
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
        public IEnumerable<DISPLAY_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            return db.Select<DISPLAY_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
        }

        /// <summary>
        /// Gets all entities from the repository
        /// </summary>
        /// <param name="skip">The the number of rows to skip</param>
        /// <param name="take">The nummber of result rows to retrieve</param>
        /// <returns></returns>
        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null, int? take = null)
        {
            return db.Select<DISPLAY_ALARM>(skip, take);
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one in the repository
        /// </summary>
        /// <param name="entity">The entity to save</param>
        public void Save(DISPLAY_ALARM entity)
        {
            db.Save(entity);
        }

        /// <summary>
        /// Inserts new entities or updates existing ones in the repository
        /// </summary>
        /// <param name="entities">The entities to save</param>
        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            db.SaveAll(entities);
        }

        /// <summary>
        /// Patches fields of an entity in the repository
        /// </summary>
        /// <param name="source">The source containing patch details</param>
        /// <param name="fields">Specfies which fields are used for the patching. The fields are specified in an anonymous variable</param>
        /// <param name="keys">Filters the entities to patch by keys. No filter implies all entities are patched</param>
        public void Patch(DISPLAY_ALARM source, Expression<Func<DISPLAY_ALARM, object>> fields, IEnumerable<Guid> keys = null)
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

            var okeys = (keys != null)
                ? db.SelectParam<DISPLAY_ALARM>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                : db.SelectParam<DISPLAY_ALARM>(q => q.Id).ToArray();

            if (!sprimitives.NullOrEmpty())
            {
                //4. Update matching event primitives
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<DISPLAY_ALARM, object>(
                    CodeDomLanguage.csharp,
                    "System.dll", "System.Core.dll",
                    typeof(DISPLAY_ALARM).Assembly.Location,
                    typeof(IContainsKey<Guid>).Assembly.Location);
                if (!okeys.NullOrEmpty()) db.UpdateOnly(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                else db.UpdateOnly(source, patchexpr);
            }
        }

        /// <summary>
        /// Erases an entity from the repository based on a unique identifier
        /// </summary>
        /// <param name="key">The unique identifier of the entity</param>
        public void Erase(Guid key)
        {
            db.Delete<DISPLAY_ALARM>(q => q.Id == key);
        }

        /// <summary>
        /// Erases entities from the repository based on unique identifiers
        /// </summary>
        /// <param name="keys">The unique identifier of the entity</param>
        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty()) db.Delete<DISPLAY_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
            else db.DeleteAll<DISPLAY_ALARM>();
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
            return db.Count<DISPLAY_ALARM>(q => q.Id == key) != 0;
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
            if (mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown)
                return db.Count<DISPLAY_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
            return db.Count<DISPLAY_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (conn != null) conn.Dispose();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class EmailAlarmOrmLiteRepository : IEmailAlarmRepository, IOrmLiteRepository, IDisposable
    {
        private IDbConnection conn;

        private IDbConnection db
        {
            get { return (conn) ?? (conn = factory.OpenDbConnection()); }
        }

        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IDbConnectionFactory factory;

        public IDbConnectionFactory DbConnectionFactory
        {
            get { return factory; }
        }

        public EmailAlarmOrmLiteRepository(IKeyGenerator<Guid> keygenerator, IDbConnectionFactory factory)
        {
            if (keygenerator == null) throw new ArgumentNullException("keygenerator");
            if (factory == null) throw new ArgumentNullException("factory");

            this.keygenerator = keygenerator;
            this.factory = factory;
        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            var matches = db.Select<EMAIL_ALARM>(q => q.Id == dry.Id);

            if (matches.NullOrEmpty()) return dry;

            var attendees = db.Select<ATTENDEE, EMAIL_ALARM, REL_EALARMS_ATTENDEES>(
                r => r.AttendeeId,
                r => r.AlarmId,
                a => a.Id == dry.Id);
            if (!attendees.NullOrEmpty()) dry.Attendees.MergeRange(attendees);

            var attachbins = db.Select<ATTACH_BINARY, EMAIL_ALARM, REL_EALARMS_ATTACHBINS>(
                r => r.AttachmentId,
                r => r.AlarmId,
                a => a.Id == dry.Id);
            if (!attachbins.NullOrEmpty()) dry.AttachmentBinaries.MergeRange(attachbins);

            var attachuris = db.Select<ATTACH_URI, EMAIL_ALARM, REL_EALARMS_ATTACHURIS>(
                r => r.AttachmentId,
                r => r.AlarmId,
                a => a.Id == dry.Id);
            if (!attachuris.NullOrEmpty()) dry.AttachmentUris.MergeRange(attachuris);

            return dry;
        }

        public IEnumerable<EMAIL_ALARM> HydrateAll(IEnumerable<EMAIL_ALARM> dry)
        {
            var keys = dry.Select(q => q.Id).ToArray();
            var full = db.Select<EMAIL_ALARM>(q => Sql.In(q.Id, keys));

            //1. retrieve relationships
            if (full.NullOrEmpty()) return dry;

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
                    if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.MergeRange(xattachbins);
                }

                if (!rattachuris.NullOrEmpty())
                {
                    var xattachuris = from y in attachuris
                                      join r in rattachuris on y.Id equals r.AttachmentId
                                      join a in full on r.AlarmId equals a.Id
                                      where a.Id == x.Id
                                      select y;
                    if (!xattachuris.NullOrEmpty()) x.AttachmentUris.MergeRange(xattachuris);
                }

                if (!rattendees.NullOrEmpty())
                {
                    var xattendees = from y in attendees
                                     join r in rattendees on y.Id equals r.AttendeeId
                                     join f in full on r.AlarmId equals f.Id
                                     where f.Id == x.Id
                                     select y;
                    if (!xattendees.NullOrEmpty()) x.Attendees.MergeRange(xattendees);
                }
            });

            return full;
        }

        public EMAIL_ALARM Find(Guid key)
        {
            var dry = db.Select<EMAIL_ALARM>(q => q.Id == key).FirstOrDefault();
            return dry != null ? Hydrate(dry) : dry;
        }

        public IEnumerable<EMAIL_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            var dry = db.Select<EMAIL_ALARM>(q => Sql.In(q.Id, keys.ToArray()), skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null, int? take = null)
        {
            var dry = db.Select<EMAIL_ALARM>(skip, take);
            return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
        }

        public void Save(EMAIL_ALARM entity)
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
                db.MergeAll(rattendees, orattendees);
            }

            if (!attachbins.NullOrEmpty())
            {
                db.SaveAll(attachbins);
                var rattachbins = attachbins.Select(x => new REL_EALARMS_ATTACHBINS { AlarmId = entity.Id, AttachmentId = x.Id });
                var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachbins.Select(x => x.Id).ToArray()));
                db.MergeAll(rattachbins, orattachbins);
            }

            if (!attachuris.NullOrEmpty())
            {
                db.SaveAll(attachuris);
                var rattachuris = attachuris.Select(x => new REL_EALARMS_ATTACHURIS { AlarmId = entity.Id, AttachmentId = x.Id });
                var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => q.Id == entity.Id && Sql.In(q.AttachmentId, attachuris.Select(x => x.Id).ToArray()));
                db.MergeAll(rattachuris, orattachuris);
            }
        }

        public void Erase(Guid key)
        {
            db.Delete<EMAIL_ALARM>(q => q.Id == key);
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            try
            {
                db.SaveAll(entities);

                //1. retrieve details of events
                var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
                var attachbins = entities.Where(x => x.AttachmentBinaries.Any()).SelectMany(x => x.AttachmentBinaries);
                var attachuris = entities.Where(x => x.AttachmentUris.Any()).SelectMany(x => x.AttachmentUris);
                var keys = entities.Select(x => x.Id);

                //2. save details of events
                if (!attendees.NullOrEmpty())
                {
                    db.SaveAll(attendees.Distinct());
                    var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                        .SelectMany(a => a.Attendees.Select(x => new REL_EALARMS_ATTENDEES
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = a.Id,
                            AttendeeId = x.Id
                        }));
                    var orattendees = db.Select<REL_EALARMS_ATTENDEES>(q => Sql.In(q.Id, keys));
                    db.MergeAll(rattendees, orattendees);
                }

                if (!attachbins.NullOrEmpty())
                {
                    db.SaveAll(attachbins.Distinct());
                    var rattachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty())
                        .SelectMany(a => a.AttachmentBinaries.Select(x => new REL_EALARMS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = a.Id,
                            AttachmentId = x.Id
                        }));
                    var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => Sql.In(q.Id, keys));
                    db.MergeAll(rattachbins, orattachbins);
                }

                if (!attachuris.NullOrEmpty())
                {
                    db.SaveAll(attachuris.Distinct());
                    var rattachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty())
                        .SelectMany(a => a.AttachmentUris.Select(x => new REL_EALARMS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = a.Id,
                            AttachmentId = x.Id
                        }));
                    var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => Sql.In(q.Id, keys));
                    db.MergeAll(rattachuris, orattachuris);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (ApplicationException) { throw; }
        }

        public void EraseAll(IEnumerable<Guid> keys)
        {
            db.Delete<EMAIL_ALARM>(q => Sql.In(q.Id, keys.ToArray()));
        }

        public void EraseAll()
        {
            db.DeleteAll<EMAIL_ALARM>();
        }

        public void Patch(EMAIL_ALARM source, Expression<Func<EMAIL_ALARM, object>> fields, IEnumerable<Guid> keys = null)
        {
            var okeys = (keys != null)
                ? db.SelectParam<EMAIL_ALARM>(q => q.Id, p => Sql.In(p.Id, keys.ToArray())).ToArray()
                : db.SelectParam<EMAIL_ALARM>(q => q.Id).ToArray();

            var selection = fields.GetMemberNames();

            //1. obtain primitives i.e. non-normalized properties
            Expression<Func<EMAIL_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Trigger,
                x.Duration,
                x.Repeat,
                x.Description,
                x.Summary
            };

            //2. obtain relations i.e. normalized properties
            Expression<Func<EMAIL_ALARM, object>> relations = x => new
            {
                x.Attendees,
                x.AttachmentBinaries,
                x.AttachmentUris
            };

            //3. choose selected properties
            var srelations = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //4. Patch relations
            if (!srelations.NullOrEmpty())
            {
                Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                Expression<Func<EMAIL_ALARM, object>> attachbinsexpr = y => y.AttachmentBinaries;
                Expression<Func<EMAIL_ALARM, object>> attachurisexpr = y => y.AttachmentUris;

                if (srelations.Contains(attendsexpr.GetMemberName()))
                {
                    var orattendees = db.Select<REL_EALARMS_ATTENDEES>(q => Sql.In(q.AlarmId, okeys));
                    if (!source.Attendees.NullOrEmpty())
                    {
                        db.SaveAll(source.Attendees.Distinct());
                        var rattendees = okeys.SelectMany(x => source.Attendees.Select(y => new REL_EALARMS_ATTENDEES
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = x,
                            AttendeeId = y.Id
                        }));
                        db.MergeAll(rattendees, orattendees);
                    }
                    else db.RemoveAll(orattendees);
                }

                if (srelations.Contains(attachbinsexpr.GetMemberName()))
                {
                    var orattachbins = db.Select<REL_EALARMS_ATTACHBINS>(q => Sql.In(q.AlarmId, okeys));
                    if (!source.AttachmentBinaries.NullOrEmpty())
                    {
                        db.SaveAll(source.AttachmentBinaries.Distinct());
                        var rattachbins = okeys.SelectMany(x => source.AttachmentBinaries.Select(y => new REL_EALARMS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = x,
                            AttachmentId = y.Id
                        }));
                        db.MergeAll(rattachbins, orattachbins);
                    }
                    else db.RemoveAll(orattachbins);
                }

                if (srelations.Contains(attachurisexpr.GetMemberName()))
                {
                    var orattachuris = db.Select<REL_EALARMS_ATTACHURIS>(q => Sql.In(q.AlarmId, okeys));
                    if (!source.AttachmentUris.NullOrEmpty())
                    {
                        db.SaveAll(source.AttachmentUris.Distinct());
                        var rattachuris = okeys.SelectMany(x => source.AttachmentUris.Select(y => new REL_EALARMS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = x,
                            AttachmentId = y.Id
                        }));
                        db.MergeAll(rattachuris, orattachuris);
                    }
                    else db.RemoveAll(orattachuris);
                }
            }

            //5. Patch primitives
            if (!sprimitives.NullOrEmpty())
            {
                var patchstr = string.Format("f => new {{ {0} }}", string.Join(", ", sprimitives.Select(x => string.Format("f.{0}", x))));
                var patchexpr = patchstr.CompileToExpressionFunc<EMAIL_ALARM, object>(
                    CodeDomLanguage.csharp,
                    "System.dll", "System.Core.dll",
                    typeof(EMAIL_ALARM).Assembly.Location,
                    typeof(IContainsKey<Guid>).Assembly.Location);

                if (!okeys.NullOrEmpty()) db.UpdateOnly(source, patchexpr, q => Sql.In(q.Id, okeys.ToArray()));
                else db.UpdateOnly(source, patchexpr);
            }
        }

        public bool ContainsKey(Guid key)
        {
            return db.Count<EMAIL_ALARM>(q => q.Id == key) != 0;
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var dkeys = keys.Distinct().ToArray();
            if (mode == ExpectationMode.Pessimistic || mode == ExpectationMode.Unknown)
                return db.Count<EMAIL_ALARM>(q => Sql.In(q.Id, dkeys)) == dkeys.Count();
            return db.Count<EMAIL_ALARM>(q => Sql.In(q.Id, dkeys)) != 0;
        }

        public EMAIL_ALARM Dehydrate(EMAIL_ALARM full)
        {
            var dry = full;
            if (!dry.Attendees.NullOrEmpty()) dry.Attendees.Clear();
            if (!dry.AttachmentBinaries.NullOrEmpty()) dry.AttachmentBinaries.Clear();
            if (!dry.AttachmentUris.NullOrEmpty()) dry.AttachmentUris.Clear();
            return dry;
        }

        public IEnumerable<EMAIL_ALARM> DehydrateAll(IEnumerable<EMAIL_ALARM> full)
        {
            var pquery = full.AsParallel();
            pquery.ForAll(x => Dehydrate(x));
            return pquery.AsEnumerable();
        }

        public void Dispose()
        {
            if (conn != null) conn.Dispose();
        }
    }
}