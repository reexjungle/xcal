using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Common;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.infrastructure.operations.contracts;


namespace reexmonkey.xcal.service.repositories.concretes
{
    public class AudioAlarmRedisRepository: IAudioAlarmRedisRepository
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
               
        public AudioAlarmRedisRepository() { }

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
            return this.redis.As<AUDIO_ALARM>().GetById(key);
        }

        public IEnumerable<AUDIO_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            var allkeys = this.redis.As<AUDIO_ALARM>().GetAllKeys();
            if (skip == null && take == null)
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<AUDIO_ALARM>().GetByIds(filtered);
            }
            else
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<AUDIO_ALARM>().GetByIds(filtered);
            }
        }

        public IEnumerable<AUDIO_ALARM> Get(int? skip = null, int? take = null)
        {
            if (skip == null && take == null) return this.redis.As<AUDIO_ALARM>().GetAll();
            else
            {
                var allkeys = this.redis.As<AUDIO_ALARM>().GetAllKeys();
                var selected = !allkeys.NullOrEmpty()
                    ? allkeys.Skip(skip.Value).Take(take.Value)
                    : allkeys;
                return this.redis.As<AUDIO_ALARM>().GetValues(selected.ToList());
            }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<AUDIO_ALARM>().ContainsKey(key);
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<AUDIO_ALARM>().GetAllKeys().Intersect(keys);
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(AUDIO_ALARM entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = this.redis.As<AUDIO_ALARM>().GetAllKeys().ToArray();
                    if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                    #region save attributes and  relations

                    if (entity.Attachment != null && entity.Attachment is ATTACH_BINARY)
                    {
                        var attachbin = entity.Attachment as ATTACH_BINARY;
                        transaction.QueueCommand(x => x.Store(attachbin));
                        var rattachbin = new REL_AALARMS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = attachbin.Id
                        };
                        var rclient = this.redis.As<REL_AALARMS_ATTACHBINS>();
                        var orattachbin = rclient.GetAll().Where(x => x.AlarmId.Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachbin.NullOrEmpty()
                            ? rattachbin.ToSingleton().Except(orattachbin)
                            : rattachbin.ToSingleton()));
                    }

                    if (entity.Attachment != null && entity.Attachment is ATTACH_URI)
                    {
                        var attachuri = entity.Attachment as ATTACH_URI;
                        transaction.QueueCommand(x => x.Store(attachuri));
                        var rattachuri = new REL_AALARMS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = attachuri.Id
                        };
                        var rclient = this.redis.As<REL_AALARMS_ATTACHURIS>();
                        var orattachuri = rclient.GetAll().Where(x => x.AlarmId.Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachuri.NullOrEmpty()
                            ? rattachuri.ToSingleton().Except(orattachuri)
                            : rattachuri.ToSingleton()));
                    }
                    
                    #endregion   
             
                    transaction.QueueCommand(x => this.redis.As<AUDIO_ALARM>().Store(this.Dehydrate(entity)));
                
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
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
                x.Attachment
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var aclient = this.redis.As<AUDIO_ALARM>();
                    var okeys = aclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    #region save (insert or update) relational attributes

                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<AUDIO_ALARM, object>> attachsexpr = y => y.Attachment;
                        if (selection.Contains(attachsexpr.GetMemberName()))
                        {
                            var attachbin = source.Attachment as ATTACH_BINARY;
                            if (attachbin != null)
                            {
                                transaction.QueueCommand(x => x.Store(attachbin));
                                var rattachbins = keys.Select(x => new REL_AALARMS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = attachbin.Id
                                });

                                var orattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x =>
                                    x.StoreAll(!orattachbins.NullOrEmpty()
                                    ? rattachbins.Except(orattachbins)
                                    : rattachbins));
                            }

                            var attachuri = source.Attachment as ATTACH_URI;
                            if (attachuri != null)
                            {
                                transaction.QueueCommand(x => x.Store(attachuri));
                                var rattachuris= keys.Select(x => new REL_AALARMS_ATTACHURIS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = attachbin.Id
                                });

                                var orattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x =>
                                    x.StoreAll(!orattachuris.NullOrEmpty()
                                    ? rattachuris.Except(orattachuris)
                                    : rattachuris));
                            }
                        }
                    }

                    #endregion

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<AUDIO_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<AUDIO_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<AUDIO_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<AUDIO_ALARM, object>> triggerexpr = x => x.Trigger;

                        var entities = aclient.GetByIds(keys).ToList();
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (selection.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (selection.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                        });

                        transaction.QueueCommand(x => x.StoreAll(this.Dehydrate(entities)));
                    }

                    #endregion

                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            }); 
        }

        public void Erase(string key)
        {
            var aaclient = this.redis.As<AUDIO_ALARM>();
            if (aaclient.ContainsKey(key))
            {

                var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                if (!rattachbins.NullOrEmpty()) this.redis.As<REL_AALARMS_ATTACHBINS>()
                    .DeleteByIds(rattachbins.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll();
                if (!rattachuris.NullOrEmpty()) this.redis.As<REL_AALARMS_ATTACHURIS>()
                    .DeleteByIds(rattachuris.Where(x => x.AlarmId.Equals(key, StringComparison.OrdinalIgnoreCase)));

                aaclient.DeleteById(key);
            }
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var cclient = this.redis.As<AUDIO_ALARM>();

                    //watch keys
                    var okeys = cclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    //save calendar
                    var keys = entities.Select(c => c.Id);
                    transaction.QueueCommand(x => x.StoreAll(entities));

                    //save attachments
                    var attachbins = entities.Where(x => x.Attachment != null && x.Attachment is ATTACH_BINARY)
                        .Select(x => x.Attachment as ATTACH_BINARY);
                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins));
                        var rattachbins = entities.Where(x => x.Attachment != null && x.Attachment is ATTACH_BINARY)
                            .Select(x => new REL_AALARMS_ATTACHBINS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                AlarmId = x.Id,
                                AttachmentId = (x.Attachment as ATTACH_BINARY).Id
                            });

                        var orattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll()
                            .Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => x.StoreAll(!orattachbins.NullOrEmpty()
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    var attachuris = entities.Where(x => x.Attachment != null && x.Attachment is ATTACH_URI)
                        .Select(x => x.Attachment as ATTACH_URI);
                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins));
                        var rattachuris = entities.Where(x => x.Attachment != null && x.Attachment is ATTACH_URI)
                            .Select(x => new REL_AALARMS_ATTACHURIS
                            {
                                Id = this.KeyGenerator.GetNextKey(),
                                AlarmId = x.Id,
                                AttachmentId = (x.Attachment as ATTACH_URI).Id
                            });

                        var orattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll()
                            .Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => x.StoreAll(!orattachuris.NullOrEmpty()
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    transaction.QueueCommand(x => x.StoreAll(this.Dehydrate(entities)));
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var aaclient = this.redis.As<AUDIO_ALARM>();
            var allkeys = aaclient.GetAllKeys().Select(x => UrnId.GetStringId(x));
            if (!allkeys.NullOrEmpty())
            {
                var found = allkeys.Intersect(keys);
                if(!found.NullOrEmpty())
                {
                    var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                    if (!rattachbins.NullOrEmpty()) this.redis.As<REL_AALARMS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => found.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase)));

                    var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll();
                    if (!rattachuris.NullOrEmpty()) this.redis.As<REL_AALARMS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => found.Contains(x.AlarmId, StringComparer.OrdinalIgnoreCase)));

                    aaclient.DeleteByIds(found);
                }

            }
            else aaclient.DeleteAll();
        }

        public AUDIO_ALARM Hydrate(AUDIO_ALARM dry)
        {
            var full = dry;
            try
            {
                if (full != null)
                {
                    var rattachbins = this.redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                    if (!rattachbins.NullOrEmpty()) full.Attachment = this.redis.As<ATTACH_BINARY>().GetById(rattachbins.FirstOrDefault().AttachmentId);

                    var rattachuris = this.redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                    if (!rattachuris.NullOrEmpty()) full.Attachment = this.redis.As<ATTACH_URI>().GetById(rattachuris.FirstOrDefault().AttachmentId);
                }
            }
            catch (ArgumentNullException) { throw; }

            return full ?? dry;
        }

        public IEnumerable<AUDIO_ALARM> Hydrate(IEnumerable<AUDIO_ALARM> dry)
        {
            var full = dry.ToList();
            var keys = full.Select(x => x.Id).Distinct().ToList();

            #region 1. retrieve relationships

            var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();

            #endregion

            #region 2. retrieve secondary entities

            var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetByIds(rattachbins.Select(x => x.AttachmentId)) : null;
            var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetByIds(rattachuris.Select(x => x.AttachmentId)) : null;

            #endregion

            #region 3. Use Linq to stitch secondary entities to primary entities

            full.ForEach(x =>
            {

                if (!attachbins.NullOrEmpty())
                {
                    var xattachbins = from y in attachbins
                                      join r in rattachbins on y.Id equals r.AttachmentId
                                      join e in full on r.EventId equals e.Id
                                      where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                      select y;
                    if (!xattachbins.NullOrEmpty()) x.Attachment = xattachbins.FirstOrDefault();

                }

                if (!attachuris.NullOrEmpty())
                {
                    var xattachuris = from y in attachuris
                                      join r in rattachuris on y.Id equals r.AttachmentId
                                      join e in full on r.EventId equals e.Id
                                      where e.Id.Equals(x.Id, StringComparison.OrdinalIgnoreCase)
                                      select y;
                    if (!xattachuris.NullOrEmpty()) x.Attachment = xattachuris.FirstOrDefault();
                }

            });

            #endregion

            return full ?? dry;
        }

        public AUDIO_ALARM Dehydrate(AUDIO_ALARM full)
        {
            try
            {
                full.Attachment = null;
            }
            catch (ArgumentNullException) { throw; }
            return full;        
        }

        public IEnumerable<AUDIO_ALARM> Dehydrate(IEnumerable<AUDIO_ALARM> full)
        {
            try
            {
                return full.Select(x => { return this.Dehydrate(x); });

            }
            catch (ArgumentNullException) { throw; }
        }
    }

    public class DisplayAlarmRedisRepository: IDisplayAlarmRedisRepository
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

        public DisplayAlarmRedisRepository() { }

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
            return this.redis.As<DISPLAY_ALARM>().GetById(key);
        }

        public IEnumerable<DISPLAY_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            var allkeys = this.redis.As<DISPLAY_ALARM>().GetAllKeys();
            if (skip == null && take == null)
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<DISPLAY_ALARM>().GetByIds(filtered);
            }
            else
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<DISPLAY_ALARM>().GetByIds(filtered);
            }
        }

        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null, int? take = null)
        {
            if (skip == null && take == null) return this.redis.As<DISPLAY_ALARM>().GetAll();
            else
            {
                var allkeys = this.redis.As<DISPLAY_ALARM>().GetAllKeys();
                var selected = !allkeys.NullOrEmpty()
                    ? allkeys.Skip(skip.Value).Take(take.Value)
                    : allkeys;
                return this.redis.As<DISPLAY_ALARM>().GetValues(selected.ToList());
            }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<DISPLAY_ALARM>().ContainsKey(key);
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<DISPLAY_ALARM>().GetAllKeys().Intersect(keys);
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(DISPLAY_ALARM entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = this.redis.As<DISPLAY_ALARM>().GetAllKeys().ToArray();
                    if (!keys.NullOrEmpty()) this.redis.Watch(keys);

                    transaction.QueueCommand(x => this.redis.As<DISPLAY_ALARM>().Store(entity));

                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });
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

            #endregion

            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var aclient = this.redis.As<DISPLAY_ALARM>();
                    var okeys = aclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<DISPLAY_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<DISPLAY_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<DISPLAY_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<DISPLAY_ALARM, object>> triggerexpr = x => x.Trigger;
                        Expression<Func<DISPLAY_ALARM, object>> descexpr = x => x.Description;

                        var entities = aclient.GetByIds(keys).ToList();
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

                    #endregion

                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            }); 
        }

        public void Erase(string key)
        {
            var daclient = this.redis.As<DISPLAY_ALARM>();
            if (daclient.ContainsKey(key)) daclient.DeleteById(key);
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var daclient = this.redis.As<DISPLAY_ALARM>();

                    //watch keys
                    var okeys = daclient.GetAllKeys().ToArray();
                    if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

                    var keys = entities.Select(c => c.Id);
                    transaction.QueueCommand(x => x.StoreAll(entities));
                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
            });
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var daclient = this.redis.As<DISPLAY_ALARM>();
            var allkeys = daclient.GetAllKeys().Select(x => UrnId.GetStringId(x));
            if (!allkeys.NullOrEmpty())
            {
                var found = allkeys.Intersect(keys);
                if (!found.NullOrEmpty())daclient.DeleteByIds(found);
            }
            else daclient.DeleteAll();
        }

    }

    public class EmailAlarmRedisRepository: IEmailAlarmRedisRepository
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

        public EmailAlarmRedisRepository() { }

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
            EMAIL_ALARM full = null;
            var cclient = this.redis.As<EMAIL_ALARM>();
            full = cclient.GetValue(dry.Id);
            if (full != null)
            {
                var rattendees = this.redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == full.Id);
                var rattachbins = this.redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == full.Id);
                var rattachuris = this.redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == full.Id);

                if (!rattachbins.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()));
                }
                if (!rattachuris.NullOrEmpty())
                {
                    full.Attachments.AddRangeComplement(this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()));
                }
                if (!rattendees.NullOrEmpty())
                {
                    full.Attendees.AddRangeComplement(this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()));
                }

            }
            return full ?? dry;
        }

        public IEnumerable<EMAIL_ALARM> Hydrate(IEnumerable<EMAIL_ALARM> dry)
        {
            List<EMAIL_ALARM> full = null;
            var eclient = this.redis.As<EMAIL_ALARM>();
            var keys = dry.Select(x => x.Id).Distinct().ToList();
            if (eclient.GetAllKeys().Intersect(keys).Count() == keys.Count()) //all keys are found
            {
                full = eclient.GetValues(keys);

                #region 1. retrieve relationships

                var rattachbins = this.redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rattachuris = this.redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId));
                var rattendees = this.redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId));
                #endregion

                #region 2. retrieve secondary entities

                var attachbins = (!rattachbins.Empty()) ? this.redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId).ToList()) : null;
                var attachuris = (!rattachuris.Empty()) ? this.redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId).ToList()) : null;
                var attendees = (!rattendees.Empty()) ? this.redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId).ToList()) : null;

                #endregion

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
                        if (!xattachbins.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachbins);

                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.Attachments.AddRangeComplement(xattachuris);
                    }

                });
                #endregion
            }

            return full ?? dry;
        }

        public EMAIL_ALARM Find(string key)
        {
            return this.redis.As<EMAIL_ALARM>().GetById(key);
        }

        public IEnumerable<EMAIL_ALARM> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            var allkeys = this.redis.As<EMAIL_ALARM>().GetAllKeys();
            if (skip == null && take == null)
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<EMAIL_ALARM>().GetByIds(filtered);
            }
            else
            {
                var filtered = !keys.NullOrEmpty()
                    ? allkeys.Intersect(keys.Select(x => UrnId.GetStringId(x)))
                    : new List<string>();
                return this.redis.As<EMAIL_ALARM>().GetByIds(filtered);
            }
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null, int? take = null)
        {
            if (skip == null && take == null) return this.redis.As<EMAIL_ALARM>().GetAll();
            else
            {
                var allkeys = this.redis.As<DISPLAY_ALARM>().GetAllKeys();
                var selected = !allkeys.NullOrEmpty()
                    ? allkeys.Skip(skip.Value).Take(take.Value)
                    : allkeys;
                return this.redis.As<EMAIL_ALARM>().GetValues(selected.ToList());
            }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<EMAIL_ALARM>().ContainsKey(key);
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<EMAIL_ALARM>().GetAllKeys().Intersect(keys);
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(EMAIL_ALARM entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {

                    #region retrieve attributes of entity

                    var attendees = entity.Attendees.OfType<ATTENDEE>();
                    var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                    var attachuris = entity.Attachments.OfType<ATTACH_URI>();

                    #endregion

                    #region save attributes and  relations

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees));
                        var rattendees = attendees.Select(x => new REL_EALARMS_ATTENDEES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttendeeId = x.Id
                        });

                        var rclient = this.redis.As<REL_EALARMS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => x.StoreAll(
                            !orattendees.NullOrEmpty()
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins));
                        var rattachbins = attachbins.Select(x => new REL_EALARMS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<REL_EALARMS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => x.StoreAll(
                            !orattachbins.NullOrEmpty()
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris));
                        var rattachuris = attachuris.Select(x => new REL_EALARMS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<REL_EALARMS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => x.StoreAll(
                            !orattachuris.NullOrEmpty()
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    #endregion


                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));

                }
                catch (ArgumentNullException) { throw; }
                catch (RedisResponseException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (RedisException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
                catch (InvalidOperationException)
                {
                    try { transaction.Rollback(); }
                    catch (RedisResponseException) { throw; }
                    catch (RedisException) { throw; }
                    catch (Exception) { throw; }
                }
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
                x.Attachments,
                x.Attendees
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            #region save (insert or update) relational attributes

            var eaclient = this.redis.As<EMAIL_ALARM>();
            var okeys = eaclient.GetAllKeys().ToArray();
            if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

            if (!srelation.NullOrEmpty())
            {
                Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                Expression<Func<EMAIL_ALARM, object>> attachsexpr = y => y.Attachments;

                #region save relational aggregate attributes of entities

                this.manager.ExecTrans(transaction =>
                {

                    try
                    {

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees.OfType<ATTENDEE>();
                            if (!attendees.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                                var rattendees = keys.SelectMany(x => attendees.Select(y => new REL_EALARMS_ATTENDEES
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttendeeId = y.Id
                                }));

                                var rclient = this.redis.As<REL_EALARMS_ATTENDEES>();
                                var orattendees = rclient.GetAll().Where(x => x.AlarmId == source.Id);
                                transaction.QueueCommand(x => rclient.StoreAll(
                                    !orattendees.NullOrEmpty()
                                    ? rattendees.Except(orattendees)
                                    : rattendees));
                            }
                        }

                        if (selection.Contains(attachsexpr.GetMemberName()))
                        {
                            var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                            if (!attachbins.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                                var rattachbins = keys.SelectMany(x => attachbins.Select(y => new REL_EALARMS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));

                                var rclient = this.redis.As<REL_EALARMS_ATTACHBINS>();
                                var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x => rclient.StoreAll(!orattachbins.NullOrEmpty()
                                    ? rattachbins.Except(orattachbins)
                                    : rattachbins));
                            }

                            var attachuris = source.Attachments.OfType<ATTACH_URI>();
                            if (!attachuris.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                                var rattachuris = keys.SelectMany(x => attachuris.Select(y => new REL_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                var rclient = this.redis.As<REL_EALARMS_ATTACHURIS>();
                                var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x => rclient.StoreAll(!orattachuris.NullOrEmpty()
                                    ? rattachuris.Except(orattachuris)
                                    : rattachuris));
                            }
                        }
                    }
                    catch (ArgumentNullException) { throw; }
                    catch (RedisResponseException)
                    {
                        try { transaction.Rollback(); }
                        catch (RedisResponseException) { throw; }
                        catch (RedisException) { throw; }
                        catch (Exception) { throw; }
                    }
                    catch (RedisException)
                    {
                        try { transaction.Rollback(); }
                        catch (RedisResponseException) { throw; }
                        catch (RedisException) { throw; }
                        catch (Exception) { throw; }
                    }
                    catch (InvalidOperationException)
                    {
                        try { transaction.Rollback(); }
                        catch (RedisResponseException) { throw; }
                        catch (RedisException) { throw; }
                        catch (Exception) { throw; }
                    }

                });

                #endregion



            }

            #endregion
        }

        public void Erase(string key)
        {
            var eclient = this.redis.As<EMAIL_ALARM>();
            if (eclient.ContainsKey(key))
            {
                eclient.DeleteRelatedEntities<REL_EALARMS_ATTACHBINS>(key);
                eclient.DeleteRelatedEntities<REL_EALARMS_ATTACHURIS>(key);
                eclient.DeleteRelatedEntities<REL_EALARMS_ATTENDEES>(key);
                eclient.DeleteById(key);
            }
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            #region 1. retrieve attributes of entities

            var attendees = entities.Where(x => !x.Attendees.NullOrEmpty() && !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                .SelectMany(x => x.Attendees.OfType<ATTENDEE>());
            var attachbins = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
            var attachuris = entities.Where(x => !x.Attachments.NullOrEmpty() && !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                .SelectMany(x => x.Attachments.OfType<ATTACH_URI>());

            #endregion

            #region 2. save aggregate attribbutes of entities

            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    transaction.QueueCommand(x => x.StoreAll(entities));
                    var keys = entities.Select(x => x.Id).ToArray();
                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attendees));
                        var rattendees = entities.Where(x => !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                            .SelectMany(e => e.Attendees.OfType<ATTENDEE>()
                                .Select(x => new REL_EALARMS_ATTENDEES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttendeeId = x.Id
                                }));

                        var rclient = this.redis.As<REL_EALARMS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => x.StoreAll((!orattendees.NullOrEmpty())
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachbins));
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>()
                                .Select(x => new REL_EALARMS_ATTACHBINS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EALARMS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => x.StoreAll((!orattachbins.NullOrEmpty())
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => x.StoreAll(attachuris));
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>()
                                .Select(x => new REL_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<REL_EALARMS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => x.StoreAll((!orattachuris.NullOrEmpty())
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    try { transaction.Rollback(); }
                    catch (Exception) { throw; }
                }
            }

            #endregion        
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var eclient = this.redis.As<EMAIL_ALARM>();
            Action<IRedisTypedClient<EMAIL_ALARM>, string> cascade_on_delete = (e, k) =>
            {
                if (e.ContainsKey(k))
                {
                    eclient.DeleteRelatedEntities<REL_EALARMS_ATTACHURIS>(k);
                    eclient.DeleteRelatedEntities<REL_EALARMS_ATTACHBINS>(k);
                    eclient.DeleteRelatedEntities<REL_EALARMS_ATTENDEES>(k);

                }
            };

            if (keys == null)
            {
                eclient.GetAllKeys().ForEach(x => cascade_on_delete(eclient, x));
                eclient.DeleteAll();
            }
            else
            {
                var dkeys = keys.Distinct().ToList();
                dkeys.ForEach(x => cascade_on_delete(eclient, x));
                eclient.DeleteByIds(dkeys);
            }
        }

        public EMAIL_ALARM Dehydrate(EMAIL_ALARM full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Dehydrate(IEnumerable<EMAIL_ALARM> full)
        {
            throw new NotImplementedException();
        }
    }
}
