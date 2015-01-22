using reexjungle.foundation.essentials.concretes;
using reexjungle.foundation.essentials.contracts;
using reexjungle.infrastructure.contracts;
using reexjungle.technical.data.concretes.extensions.redis;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using ServiceStack.Common;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class AudioAlarmRedisRepository : IAudioAlarmRedisRepository
    {
        private IRedisClientsManager manager;
        private IKeyGenerator<string> keygen;
        private IRedisClient client = null;

        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
                this.client = manager.GetClient();
            }
        }

        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        public AudioAlarmRedisRepository()
        {
        }

        public AudioAlarmRedisRepository(IRedisClientsManager manager)
        {
            this.RedisClientsManager = manager;
        }

        public AudioAlarmRedisRepository(IRedisClient client)
        {
            if (client == null) throw new ArgumentNullException("IRedisClient");
            this.client = client;
        }

        public AUDIO_ALARM Find(string key)
        {
            try
            {
                return this.redis.As<AUDIO_ALARM>().GetById(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<AUDIO_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<AUDIO_ALARM>().GetByIds(keys);
                else return this.redis.As<AUDIO_ALARM>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<AUDIO_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<AUDIO_ALARM>().GetAll();
                else
                {
                    var alarms = this.redis.As<AUDIO_ALARM>().GetAll();
                    return !alarms.NullOrEmpty()
                        ? alarms.Skip(skip.Value).Take(take.Value)
                        : alarms;
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return this.redis.As<AUDIO_ALARM>().ContainsKey(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var matches = this.redis.As<AUDIO_ALARM>().GetAllKeys().Intersect(keys);
                if (matches.NullOrEmpty()) return false;
                return mode == ExpectationMode.pessimistic
                    ? matches.Count() == keys.Count()
                    : !matches.NullOrEmpty();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Save(AUDIO_ALARM entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    #region save normalized and non-normalized attributes

                    var orattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == entity.Id);
                    var orattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == entity.Id);

                    if (entity.AttachmentBinary != null)
                    {
                        transaction.QueueCommand(x => x.Store(entity.AttachmentBinary));
                        var rattachbin = new REL_AALARMS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = entity.AttachmentBinary.Id
                        };

                        this.redis.MergeAll(rattachbin.ToSingleton(), orattachbins, transaction);
                    }
                    else this.redis.RemoveAll(orattachbins, transaction);

                    if (entity.AttachmentUri != null)
                    {
                        transaction.QueueCommand(x => x.Store(entity.AttachmentUri));
                        var rattachuri = new REL_AALARMS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = entity.AttachmentUri.Id
                        };

                        this.redis.MergeAll(rattachuri.ToSingleton(), orattachuris, transaction);
                    }
                    else this.redis.RemoveAll(orattachuris, transaction);

                    #endregion save normalized and non-normalized attributes

                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });
        }

        public void Patch(AUDIO_ALARM source, Expression<Func<AUDIO_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames();

            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Duration,
                x.Trigger,
                x.Repeat
            };

            Expression<Func<AUDIO_ALARM, object>> relations = x => new
            {
                x.AttachmentUri,
                x.AttachmentBinary
            };

            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);
            var srelations = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var aclient = this.redis.As<AUDIO_ALARM>();
                    var okeys = aclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    #region save (insert or update) normalized attributes

                    if (!srelations.NullOrEmpty())
                    {
                        Expression<Func<EMAIL_ALARM, object>> attachbinsexpr = y => y.AttachmentBinaries;
                        Expression<Func<EMAIL_ALARM, object>> attachurisexpr = y => y.AttachmentUris;

                        var orattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                        var orattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                        if (srelations.Contains(attachbinsexpr.GetMemberName()))
                        {
                            if (source.AttachmentBinary != null)
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().Store(source.AttachmentBinary));
                                var rattachbins = keys.Select(x => new REL_AALARMS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = source.AttachmentBinary.Id
                                });

                                this.redis.MergeAll(rattachbins, orattachbins, transaction);
                            }
                            else this.redis.RemoveAll(orattachbins, transaction);
                        }

                        if (srelations.Contains(attachurisexpr.GetMemberName()))
                        {
                            if (source.AttachmentUri != null)
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().Store(source.AttachmentUri));
                                var rattachuris = keys.Select(x => new REL_AALARMS_ATTACHURIS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = source.AttachmentUri.Id
                                });

                                this.redis.MergeAll(rattachuris, orattachuris, transaction);
                            }
                            else this.redis.RemoveAll(orattachuris, transaction);
                        }
                    }

                    #endregion save (insert or update) normalized attributes

                    #region save (insert or update) non-normalized attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<AUDIO_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<AUDIO_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<AUDIO_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<AUDIO_ALARM, object>> triggerexpr = x => x.Trigger;

                        var entities = aclient.GetByIds(keys).ToList();
                        entities.ForEach(x =>
                        {
                            if (sprimitives.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (sprimitives.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (sprimitives.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (sprimitives.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                        });

                        transaction.QueueCommand(x => x.StoreAll(entities));
                    }

                    #endregion save (insert or update) non-normalized attributes
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });
        }

        public void Erase(string key)
        {
            try
            {
                if (this.redis.As<AUDIO_ALARM>().ContainsKey(key))
                {
                    var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteById(key));
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            try
            {
                var keys = this.redis.As<AUDIO_ALARM>().GetAllKeys().ToArray();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                this.manager.ExecTrans(transaction =>
                {
                    if (!entities.NullOrEmpty()) transaction.QueueCommand(x => x.StoreAll(entities.Distinct()));
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty())
                {
                    var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => keys.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => keys.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteByIds(keys));
                    });
                }
                else
                {
                    this.manager.ExecTrans(transaction =>
                    {
                        transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteAll());
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public AUDIO_ALARM Hydrate(AUDIO_ALARM dry)
        {
            try
            {
                var full = this.redis.As<AUDIO_ALARM>().GetValue(dry.Id);
                if (full != null)
                {
                    var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == full.Id);
                    var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == full.Id);

                    if (!rattachbins.NullOrEmpty())
                    {
                        full.AttachmentBinary = this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()).First();
                    }
                    if (!rattachuris.NullOrEmpty())
                    {
                        full.AttachmentUri = this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()).First();
                    }
                }
                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<AUDIO_ALARM> HydrateAll(IEnumerable<AUDIO_ALARM> dry)
        {
            try
            {
                var full = dry.ToList();
                var eclient = this.redis.As<AUDIO_ALARM>();
                var keys = dry.Select(x => x.Id).Distinct().ToList();
                if (eclient.GetAllKeys().Intersect(keys).Count() == keys.Count()) //all keys are found
                {
                    full = eclient.GetValues(keys);

                    #region 1. retrieve relationships

                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId));
                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId));

                    #endregion 1. retrieve relationships

                    #region 2. retrieve secondary entities

                    var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()) : null;
                    var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()) : null;

                    #endregion 2. retrieve secondary entities

                    #region 3. Use Linq to stitch secondary entities to primary entities

                    full.ForEach(x =>
                    {
                        if (!attachbins.NullOrEmpty())
                        {
                            var xattachbins = from y in attachbins
                                              join r in rattachbins on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinary = xattachbins.First();
                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUri = xattachuris.First();
                        }
                    });

                    #endregion 3. Use Linq to stitch secondary entities to primary entities
                }

                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public AUDIO_ALARM Dehydrate(AUDIO_ALARM full)
        {
            try
            {
                if (full.AttachmentBinary != null) full.AttachmentBinary = null;
                if (full.AttachmentUri != null) full.AttachmentUri = null;
                return full;
            }
            catch (ArgumentNullException) { throw; }
        }

        public IEnumerable<AUDIO_ALARM> DehydrateAll(IEnumerable<AUDIO_ALARM> full)
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
    }

    public class DisplayAlarmRedisRepository : IDisplayAlarmRedisRepository
    {
        private IRedisClientsManager manager;
        private IKeyGenerator<string> keygen;
        private IRedisClient client = null;

        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
                this.client = manager.GetClient();
            }
        }

        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        public DisplayAlarmRedisRepository()
        {
        }

        public DisplayAlarmRedisRepository(IRedisClientsManager manager)
        {
            this.RedisClientsManager = manager;
        }

        public DisplayAlarmRedisRepository(IRedisClient client)
        {
            if (client == null) throw new ArgumentNullException("IRedisClient");
            this.client = client;
        }

        public DISPLAY_ALARM Find(string key)
        {
            try
            {
                return this.redis.As<DISPLAY_ALARM>().GetById(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<DISPLAY_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<DISPLAY_ALARM>().GetByIds(keys);
                else return this.redis.As<DISPLAY_ALARM>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<DISPLAY_ALARM>().GetAll();
                else
                {
                    var alarms = this.redis.As<DISPLAY_ALARM>().GetAll();
                    var selected = !alarms.NullOrEmpty()
                        ? alarms.Skip(skip.Value).Take(take.Value)
                        : alarms;
                    return selected ?? new List<DISPLAY_ALARM>();
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return this.redis.As<DISPLAY_ALARM>().ContainsKey(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var matches = this.redis.As<DISPLAY_ALARM>().GetAllKeys().Intersect(keys);
                if (matches.NullOrEmpty()) return false;
                return mode == ExpectationMode.pessimistic
                    ? matches.Count() == keys.Count()
                    : !matches.NullOrEmpty();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Save(DISPLAY_ALARM entity)
        {
            try
            {
                var keys = this.redis.As<DISPLAY_ALARM>().GetAllKeys().ToArray();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                this.manager.ExecTrans(transaction =>
                {
                    if (entity != null) transaction.QueueCommand(x => x.Store(entity));
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Patch(DISPLAY_ALARM source, Expression<Func<DISPLAY_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames();

            Expression<Func<DISPLAY_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Duration,
                x.Trigger,
                x.Repeat,
                x.Description
            };

            //4. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            try
            {
                this.manager.ExecTrans(transaction =>
                {
                    var daclient = this.redis.As<DISPLAY_ALARM>();

                    var okeys = daclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<DISPLAY_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<DISPLAY_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<DISPLAY_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<DISPLAY_ALARM, object>> triggerexpr = x => x.Trigger;
                        Expression<Func<DISPLAY_ALARM, object>> descexpr = x => x.Description;

                        var entities = !keys.NullOrEmpty() ? daclient.GetByIds(keys).ToList() : daclient.GetAll().ToList();
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (selection.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (selection.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                            if (selection.Contains(descexpr.GetMemberName())) x.Description = source.Description;
                        });

                        transaction.QueueCommand(x => x.StoreAll(entities));
                    }

                    #endregion update-only non-relational attributes
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Erase(string key)
        {
            try
            {
                var daclient = this.redis.As<DISPLAY_ALARM>();
                if (daclient.ContainsKey(key)) daclient.DeleteById(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            try
            {
                var keys = this.redis.As<DISPLAY_ALARM>().GetAllKeys().ToArray();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                this.manager.ExecTrans(transaction =>
                {
                    if (!entities.NullOrEmpty()) transaction.QueueCommand(x => x.StoreAll(entities.Distinct()));
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                this.manager.ExecTrans(transaction =>
                {
                    if (!keys.NullOrEmpty()) transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteByIds(keys));
                    else transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteAll());
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }
    }

    public class EmailAlarmRedisRepository : IEmailAlarmRedisRepository
    {
        private IRedisClientsManager manager;
        private IKeyGenerator<string> keygen;
        private IRedisClient client = null;

        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
                this.client = manager.GetClient();
            }
        }

        public IKeyGenerator<string> KeyGenerator
        {
            get { return this.keygen; }
            set
            {
                if (value == null) throw new ArgumentNullException("KeyGenerator");
                this.keygen = value;
            }
        }

        public EmailAlarmRedisRepository()
        {
        }

        public EmailAlarmRedisRepository(IRedisClientsManager manager)
        {
            this.RedisClientsManager = manager;
        }

        public EmailAlarmRedisRepository(IRedisClient client)
        {
            if (client == null) throw new ArgumentNullException("IRedisClient");
            this.client = client;
        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            try
            {
                var full = this.redis.As<EMAIL_ALARM>().GetValue(dry.Id);
                if (full != null)
                {
                    var rattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == full.Id);
                    var rattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == full.Id);
                    var rattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == full.Id);

                    if (!rattachbins.NullOrEmpty())
                    {
                        full.AttachmentBinaries.MergeRange(this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()));
                    }
                    if (!rattachuris.NullOrEmpty())
                    {
                        full.AttachmentUris.MergeRange(this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()));
                    }
                    if (!rattendees.NullOrEmpty())
                    {
                        full.Attendees.MergeRange(this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()));
                    }
                }
                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> HydrateAll(IEnumerable<EMAIL_ALARM> dry)
        {
            try
            {
                var full = dry.ToList();
                var eclient = this.redis.As<EMAIL_ALARM>();
                var keys = dry.Select(x => x.Id).Distinct().ToList();
                if (eclient.GetAllKeys().Intersect(keys).Count() == keys.Count()) //all keys are found
                {
                    full = eclient.GetValues(keys);

                    #region 1. retrieve relationships

                    var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId));
                    var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId));
                    var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId));

                    #endregion 1. retrieve relationships

                    #region 2. retrieve secondary entities

                    var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()) : null;
                    var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()) : null;
                    var attendees = (!rattendees.Empty()) ? this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()) : null;

                    #endregion 2. retrieve secondary entities

                    #region 3. Use Linq to stitch secondary entities to primary entities

                    full.ForEach(x =>
                    {
                        if (!attendees.NullOrEmpty())
                        {
                            var xattendees = from y in attendees
                                             join r in rattendees on y.Id equals r.AttendeeId
                                             join e in full on r.EventId equals e.Id
                                             where e.Id == x.Id
                                             select y;
                            if (!xattendees.NullOrEmpty()) x.Attendees.AddRange(xattendees);
                        }

                        if (!attachbins.NullOrEmpty())
                        {
                            var xattachbins = from y in attachbins
                                              join r in rattachbins on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachbins.NullOrEmpty()) x.AttachmentBinaries.MergeRange(xattachbins);
                        }

                        if (!attachuris.NullOrEmpty())
                        {
                            var xattachuris = from y in attachuris
                                              join r in rattachuris on y.Id equals r.AttachmentId
                                              join e in full on r.EventId equals e.Id
                                              where e.Id == x.Id
                                              select y;
                            if (!xattachuris.NullOrEmpty()) x.AttachmentUris.MergeRange(xattachuris);
                        }
                    });

                    #endregion 3. Use Linq to stitch secondary entities to primary entities
                }

                return full ?? dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public EMAIL_ALARM Find(string key)
        {
            try
            {
                var dry = this.redis.As<EMAIL_ALARM>().GetById(key);
                return dry != null ? this.Hydrate(dry) : dry;
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null)
                {
                    var dry = this.redis.As<EMAIL_ALARM>().GetByIds(keys);
                    return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
                }
                else
                {
                    var dry = this.redis.As<EMAIL_ALARM>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
                    return !dry.NullOrEmpty() ? this.HydrateAll(dry) : dry;
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null, int? take = null)
        {
            try
            {
                if (skip == null && take == null) return this.redis.As<EMAIL_ALARM>().GetAll();
                else
                {
                    var alarms = this.redis.As<EMAIL_ALARM>().GetAll();
                    var selected = !alarms.NullOrEmpty()
                        ? alarms.Skip(skip.Value).Take(take.Value)
                        : alarms;
                    return !selected.NullOrEmpty() ? this.HydrateAll(selected) : new List<EMAIL_ALARM>();
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            try
            {
                return this.redis.As<EMAIL_ALARM>().ContainsKey(key);
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            try
            {
                var matches = this.redis.As<EMAIL_ALARM>().GetAllKeys().Intersect(keys);
                if (matches.NullOrEmpty()) return false;
                return mode == ExpectationMode.pessimistic
                    ? matches.Count() == keys.Count()
                    : !matches.NullOrEmpty();
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Save(EMAIL_ALARM entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    #region retrieve attributes of entity

                    var attendees = entity.Attendees;
                    var attachbins = entity.AttachmentBinaries;
                    var attachuris = entity.AttachmentUris;

                    #endregion retrieve attributes of entity

                    #region save normalized and non-normalized attributes

                    var orattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == entity.Id);
                    var orattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == entity.Id);
                    var orattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == entity.Id);

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                        var rattendees = attendees.Select(x => new REL_EALARMS_ATTENDEES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttendeeId = x.Id
                        });

                        this.redis.MergeAll(rattendees, orattendees, transaction);
                    }
                    else this.redis.RemoveAll(orattendees, transaction);

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                        var rattachbins = attachbins.Select(x => new REL_EALARMS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        this.redis.MergeAll(rattachbins, orattachbins, transaction);
                    }
                    else this.redis.RemoveAll(orattachbins, transaction);

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                        var rattachuris = attachuris.Select(x => new REL_EALARMS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        this.redis.MergeAll(rattachuris, orattachuris, transaction);
                    }
                    else this.redis.RemoveAll(orattachuris, transaction);

                    #endregion save normalized and non-normalized attributes

                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });
        }

        public void Patch(EMAIL_ALARM source, Expression<Func<EMAIL_ALARM, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames();

            Expression<Func<EMAIL_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Repeat,
                x.Trigger,
                x.Description,
                x.Duration
            };

            Expression<Func<EMAIL_ALARM, object>> relations = x => new
            {
                x.AttachmentBinaries,
                x.AttachmentUris,
                x.Attendees
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var eaclient = this.redis.As<EMAIL_ALARM>();
                    var okeys = eaclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    #region save (insert or update) relational attributes

                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                        Expression<Func<EMAIL_ALARM, object>> attachbinsexpr = y => y.AttachmentBinaries;
                        Expression<Func<EMAIL_ALARM, object>> attachurisexpr = y => y.AttachmentUris;

                        var orattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == source.Id);
                        var orattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                        var orattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees;
                            if (!attendees.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                                var rattendees = keys.SelectMany(x => attendees.Select(y => new REL_EALARMS_ATTENDEES
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttendeeId = y.Id
                                }));

                                this.redis.MergeAll(rattendees, orattendees, transaction);
                            }
                            else this.redis.RemoveAll(orattendees, transaction);
                        }

                        if (selection.Contains(attachbinsexpr.GetMemberName()))
                        {
                            var attachbins = source.AttachmentBinaries;
                            if (!attachbins.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                                var rattachbins = keys.SelectMany(x => attachbins.Select(y => new REL_EALARMS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));

                                this.redis.MergeAll(rattachbins, orattachbins, transaction);
                            }
                            else this.redis.RemoveAll(orattachbins, transaction);
                        }

                        if (selection.Contains(attachurisexpr.GetMemberName()))
                        {
                            var attachuris = source.AttachmentUris;
                            if (!attachuris.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                                var rattachuris = keys.SelectMany(x => attachuris.Select(y => new REL_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                this.redis.MergeAll(rattachuris, orattachuris, transaction);
                            }
                            else this.redis.RemoveAll(orattachuris, transaction);
                        }
                    }

                    #endregion save (insert or update) relational attributes

                    #region save (insert or update) non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<EMAIL_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<EMAIL_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<EMAIL_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<EMAIL_ALARM, object>> triggerexpr = x => x.Trigger;
                        Expression<Func<EMAIL_ALARM, object>> descexpr = x => x.Description;
                        Expression<Func<EMAIL_ALARM, object>> summexpr = x => x.Summary;

                        var entities = !keys.NullOrEmpty() ? eaclient.GetByIds(keys).ToList() : eaclient.GetAll().ToList();
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (selection.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (selection.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                            if (selection.Contains(descexpr.GetMemberName())) x.Description = source.Description;
                            if (selection.Contains(summexpr.GetMemberName())) x.Summary = source.Summary;
                        });

                        transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));
                    }

                    #endregion save (insert or update) non-relational attributes
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });
        }

        public void Erase(string key)
        {
            try
            {
                if (this.redis.As<EMAIL_ALARM>().ContainsKey(key))
                {
                    var rattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll();
                    var rattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>()
                            .DeleteByIds(rattendees.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteById(key));
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            #region 1. retrieve attributes of entities

            var attendees = entities.Where(x => !x.Attendees.NullOrEmpty()).SelectMany(x => x.Attendees);
            var attachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty()).SelectMany(x => x.AttachmentBinaries);
            var attachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty()).SelectMany(x => x.AttachmentUris);

            #endregion 1. retrieve attributes of entities

            #region 2. save aggregate attribbutes of entities

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = entities.Select(x => x.Id).ToArray();

                    var orattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.AlarmId));
                    var orattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                    var orattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                        var rattendees = entities.Where(x => !x.Attendees.NullOrEmpty())
                            .SelectMany(e => e.Attendees.Select(x => new REL_EALARMS_ATTENDEES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttendeeId = x.Id
                                }));

                        this.redis.MergeAll(rattendees, orattendees, transaction);
                    }
                    else this.redis.RemoveAll(orattendees, transaction);

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                        var rattachbins = entities.Where(x => !x.AttachmentBinaries.NullOrEmpty())
                            .SelectMany(e => e.AttachmentBinaries.Select(x => new REL_EALARMS_ATTACHBINS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));

                        this.redis.MergeAll(rattachbins, orattachbins, transaction);
                    }
                    else this.redis.RemoveAll(orattachbins, transaction);

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                        var rattachuris = entities.Where(x => !x.AttachmentUris.NullOrEmpty())
                            .SelectMany(e => e.AttachmentUris.Select(x => new REL_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        this.redis.MergeAll(rattachuris, orattachuris, transaction);
                    }
                    else this.redis.RemoveAll(orattachuris, transaction);

                    transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) { throw; }
            });

            #endregion 2. save aggregate attribbutes of entities
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            try
            {
                if (!keys.NullOrEmpty())
                {
                    var rattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll();
                    var rattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll();
                    var rattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {
                        if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>()
                            .DeleteByIds(rattachbins.Where(x => keys.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>()
                            .DeleteByIds(rattachuris.Where(x => keys.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase))));

                        if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>()
                            .DeleteByIds(rattendees.Where(x => keys.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteByIds(keys));
                    });
                }
                else
                {
                    this.manager.ExecTrans(transaction =>
                    {
                        transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteAll());
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public EMAIL_ALARM Dehydrate(EMAIL_ALARM full)
        {
            try
            {
                if (!full.Attendees.NullOrEmpty()) full.Attendees.Clear();
                if (!full.AttachmentBinaries.NullOrEmpty()) full.AttachmentBinaries.Clear();
                if (!full.AttachmentUris.NullOrEmpty()) full.AttachmentUris.Clear();
                return full;
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
    }
}