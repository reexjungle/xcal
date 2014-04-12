using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class AudioAlarmRedisRepository: IAudioAlarmRedisRepository
    {
        private IRedisClientsManager manager;
        private int? take = null;
        private IKeyGenerator<string> keygen;

        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public int? Take
        {
            get { return this.take; }
            set
            {
                if (value == null) throw new ArgumentNullException("Take");
                this.take = value;
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
            return this.redis.As<AUDIO_ALARM>().GetValue(key);
        }

        public IEnumerable<AUDIO_ALARM> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<AUDIO_ALARM> dry = null;
            var cclient = this.redis.As<AUDIO_ALARM>();
            var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (cclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (skip != null) ?
                    cclient.GetValues(dkeys).Skip(skip.Value).Take(take.Value)
                    : cclient.GetValues(dkeys);
            }
            return dry;
        }

        public IEnumerable<AUDIO_ALARM> Get(int? skip = null)
        {
            IEnumerable<AUDIO_ALARM> dry = null;
            try
            {
                var cclient = this.redis.As<AUDIO_ALARM>();
                dry = (skip != null) ?
                    cclient.GetAll().Skip(skip.Value).Take(take.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return dry;
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
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    var cclient = this.redis.As<AUDIO_ALARM>();
                    transaction.QueueCommand(x => cclient.Store(entity));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Patch(AUDIO_ALARM source, Expression<Func<AUDIO_ALARM, object>> fields, Expression<Func<AUDIO_ALARM, bool>> predicate = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames();

            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Repeat,
                x.Trigger,
                x.Duration,
                x.Attachment
            };

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    #region save (insert or update) relational attributes

                    var cclient = this.redis.As<AUDIO_ALARM>();
                    var keys = (predicate != null)
                        ? cclient.GetAll().Where(predicate.Compile()).Select(x => x.Id).ToList()
                        : cclient.GetAllKeys();

                    #endregion

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<AUDIO_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<AUDIO_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<AUDIO_ALARM, object>> triggerexpr = x => x.Trigger;
                        Expression<Func<AUDIO_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<AUDIO_ALARM, object>> attachexpr = x => x.Attachment;


                        var entities = cclient.GetValues(keys);
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (selection.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (selection.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                            if (selection.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(attachexpr.GetMemberName())) x.Attachment = source.Attachment;
                        });

                        transaction.QueueCommand(trans => cclient.StoreAll(entities));
                    }

                    #endregion

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Erase(string key)
        {
            var cclient = this.redis.As<AUDIO_ALARM>();
            if (cclient.ContainsKey(key))cclient.DeleteById(key);
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    //save calendar
                    var cclient = this.redis.As<AUDIO_ALARM>();
                    var keys = entities.Select(c => c.Id);
                    transaction.QueueCommand(x => cclient.StoreAll(entities));
                    transaction.Commit();
                }
                catch (ArgumentNullException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var cclient = this.redis.As<AUDIO_ALARM>();

            if (keys == null)cclient.DeleteAll();
            else
            {
                var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                cclient.DeleteByIds(dkeys);
            }
        }
    }

    public class DisplayAlarmRedisRepository: IDisplayAlarmRedisRepository
    {
        private IRedisClientsManager manager;
        private int? take = null;
        private IKeyGenerator<string> keygen;

        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public int? Take
        {
            get { return this.take; }
            set
            {
                if (value == null) throw new ArgumentNullException("Take");
                this.take = value;
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
            return this.redis.As<DISPLAY_ALARM>().GetValue(key);
        }

        public IEnumerable<DISPLAY_ALARM> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<DISPLAY_ALARM> dry = null;
            var cclient = this.redis.As<DISPLAY_ALARM>();
            var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (cclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (skip != null) ?
                    cclient.GetValues(dkeys).Skip(skip.Value).Take(take.Value)
                    : cclient.GetValues(dkeys);
            }
            return dry;
        }

        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null)
        {
            IEnumerable<DISPLAY_ALARM> dry = null;
            try
            {
                var cclient = this.redis.As<DISPLAY_ALARM>();
                dry = (skip != null) ?
                    cclient.GetAll().Skip(skip.Value).Take(take.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return dry;
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
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    var cclient = this.redis.As<DISPLAY_ALARM>();
                    transaction.QueueCommand(x => cclient.Store(entity));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Patch(DISPLAY_ALARM source, Expression<Func<DISPLAY_ALARM, object>> fields, Expression<Func<DISPLAY_ALARM, bool>> predicate = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields.GetMemberNames();

            Expression<Func<DISPLAY_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Repeat,
                x.Trigger,
                x.Duration,
                x.Description
            };

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    #region save (insert or update) relational attributes

                    var cclient = this.redis.As<DISPLAY_ALARM>();
                    var keys = (predicate != null)
                        ? cclient.GetAll().Where(predicate.Compile()).Select(x => x.Id).ToList()
                        : cclient.GetAllKeys();

                    #endregion

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<DISPLAY_ALARM, object>> actionexpr = x => x.Action;
                        Expression<Func<DISPLAY_ALARM, object>> repeatexpr = x => x.Repeat;
                        Expression<Func<DISPLAY_ALARM, object>> triggerexpr = x => x.Trigger;
                        Expression<Func<DISPLAY_ALARM, object>> durationexpr = x => x.Duration;
                        Expression<Func<DISPLAY_ALARM, object>> descexpr = x => x.Description;


                        var entities = cclient.GetValues(keys);
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                            if (selection.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                            if (selection.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                            if (selection.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                            if (selection.Contains(descexpr.GetMemberName())) x.Description = source.Description;
                        });

                        transaction.QueueCommand(trans => cclient.StoreAll(entities));
                    }

                    #endregion

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Erase(string key)
        {
            var cclient = this.redis.As<DISPLAY_ALARM>();
            if (cclient.ContainsKey(key)) cclient.DeleteById(key);
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    //save calendar
                    var cclient = this.redis.As<DISPLAY_ALARM>();
                    var keys = entities.Select(c => c.Id);
                    transaction.QueueCommand(x => cclient.StoreAll(entities));
                    transaction.Commit();
                }
                catch (ArgumentNullException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            var cclient = this.redis.As<DISPLAY_ALARM>();

            if (keys == null) cclient.DeleteAll();
            else
            {
                var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                cclient.DeleteByIds(dkeys);
            }
        }

    }

    public class EmailAlarmRedisRepository: IEmailAlarmRedisRepository
    {

        private IRedisClientsManager manager;
        private int? take = null;
        private IKeyGenerator<string> keygen;

        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public int? Take
        {
            get { return this.take; }
            set
            {
                if (value == null) throw new ArgumentNullException("Take");
                this.take = value;
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
                var rattendees = this.redis.As<RELS_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == full.Id);
                var rattachbins = this.redis.As<RELS_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == full.Id);
                var rattachuris = this.redis.As<RELS_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == full.Id);

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
            return this.redis.As<EMAIL_ALARM>().GetValue(key);
        }

        public IEnumerable<EMAIL_ALARM> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<EMAIL_ALARM> dry = null;
            var cclient = this.redis.As<EMAIL_ALARM>();
            var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (cclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (skip != null) ?
                    cclient.GetValues(dkeys).Skip(skip.Value).Take(take.Value)
                    : cclient.GetValues(dkeys);
            }
            return dry;
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null)
        {
            IEnumerable<EMAIL_ALARM> dry = null;
            try
            {
                var cclient = this.redis.As<EMAIL_ALARM>();
                dry = (skip != null) ?
                    cclient.GetAll().Skip(skip.Value).Take(take.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return dry;
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
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {

                    #region retrieve attributes of entity

                    var attendees = entity.Attendees.OfType<ATTENDEE>();
                    var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                    var attachuris = entity.Attachments.OfType<ATTACH_URI>();

                    #endregion

                    #region save attributes and  relations

                    transaction.QueueCommand(x => this.redis.As<EMAIL_ALARM>().Store(entity));

                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                        var rattendees = attendees.Select(x => new RELS_EALARMS_ATTENDEES
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttendeeId = x.Id
                        });

                        var rclient = this.redis.As<RELS_EALARMS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattendees.NullOrEmpty()
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                        var rattachbins = attachbins.Select(x => new RELS_EALARMS_ATTACHBINS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<RELS_EALARMS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachbins.NullOrEmpty()
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                        var rattachuris = attachuris.Select(x => new RELS_EALARMS_ATTACHURIS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            AlarmId = entity.Id,
                            AttachmentId = x.Id
                        });

                        var rclient = this.redis.As<RELS_EALARMS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => x.AlarmId == entity.Id);
                        transaction.QueueCommand(x => rclient.StoreAll(
                            !orattachuris.NullOrEmpty()
                            ? rattachuris.Except(orattachuris)
                            : rattachuris));
                    }

                    #endregion

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Patch(EMAIL_ALARM source, Expression<Func<EMAIL_ALARM, object>> fields, Expression<Func<EMAIL_ALARM, bool>> predicate = null)
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

            if (!srelation.NullOrEmpty())
            {
                Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                Expression<Func<EMAIL_ALARM, object>> attachsexpr = y => y.Attachments;

                string[] keys = null;
                var eclient = this.redis.As<EMAIL_ALARM>();
                    
                keys = (predicate != null)
                        ? eclient.GetAll().Where(predicate.Compile()).Select(x => x.Id).ToArray()
                        : eclient.GetAllKeys().ToArray();



                #region save relational aggregate attributes of entities

                using (var transaction = this.redis.CreateTransaction())
                {

                    try
                    {

                        if (selection.Contains(attendsexpr.GetMemberName()))
                        {
                            var attendees = source.Attendees.OfType<ATTENDEE>();
                            if (!attendees.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                                var rattendees = keys.SelectMany(x => attendees.Select(y => new RELS_EALARMS_ATTENDEES
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttendeeId = y.Id
                                }));

                                var rclient = this.redis.As<RELS_EALARMS_ATTENDEES>();
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
                                var rattachbins = keys.SelectMany(x => attachbins.Select(y => new RELS_EALARMS_ATTACHBINS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                
                                var rclient = this.redis.As<RELS_EALARMS_ATTACHBINS>();
                                var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x => rclient.StoreAll(!orattachbins.NullOrEmpty()
                                    ? rattachbins.Except(orattachbins)
                                    : rattachbins));
                            }

                            var attachuris = source.Attachments.OfType<ATTACH_URI>();
                            if (!attachuris.NullOrEmpty())
                            {
                                transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                                var rattachuris = keys.SelectMany(x => attachuris.Select(y => new RELS_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = x,
                                    AttachmentId = y.Id
                                }));
                                var rclient = this.redis.As<RELS_EALARMS_ATTACHURIS>();
                                var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                                transaction.QueueCommand(x => rclient.StoreAll(!orattachuris.NullOrEmpty()
                                    ? rattachuris.Except(orattachuris)
                                    : rattachuris));
                            }
                        }

                        transaction.Commit();

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }

                }

                #endregion

                

            } 

            #endregion
        }

        public void Erase(string key)
        {
            var eclient = this.redis.As<EMAIL_ALARM>();
            if (eclient.ContainsKey(key))
            {
                eclient.DeleteRelatedEntities<RELS_EALARMS_ATTACHBINS>(key);
                eclient.DeleteRelatedEntities<RELS_EALARMS_ATTACHURIS>(key);
                eclient.DeleteRelatedEntities<RELS_EALARMS_ATTENDEES>(key);
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
                    transaction.QueueCommand(x => this.redis.As<EMAIL_ALARM>().StoreAll(entities));
                    var keys = entities.Select(x => x.Id).ToArray();
                    if (!attendees.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTENDEE>().StoreAll(attendees));
                        var rattendees = entities.Where(x => !x.Attendees.OfType<ATTENDEE>().NullOrEmpty())
                            .SelectMany(e => e.Attendees.OfType<ATTENDEE>()
                                .Select(x => new RELS_EALARMS_ATTENDEES
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttendeeId = x.Id
                                }));

                        var rclient = this.redis.As<RELS_EALARMS_ATTENDEES>();
                        var orattendees = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => rclient.StoreAll((!orattendees.NullOrEmpty())
                            ? rattendees.Except(orattendees)
                            : rattendees));
                    }

                    if (!attachbins.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                        var rattachbins = entities.Where(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_BINARY>()
                                .Select(x => new RELS_EALARMS_ATTACHBINS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<RELS_EALARMS_ATTACHBINS>();
                        var orattachbins = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => rclient.StoreAll((!orattachbins.NullOrEmpty())
                            ? rattachbins.Except(orattachbins)
                            : rattachbins));
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        transaction.QueueCommand(x => this.redis.As<ATTACH_URI>().StoreAll(attachuris));
                        var rattachuris = entities.Where(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty())
                            .SelectMany(e => e.Attachments.OfType<ATTACH_URI>()
                                .Select(x => new RELS_EALARMS_ATTACHURIS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    AlarmId = e.Id,
                                    AttachmentId = x.Id
                                }));
                        var rclient = this.redis.As<RELS_EALARMS_ATTACHURIS>();
                        var orattachuris = rclient.GetAll().Where(x => keys.Contains(x.AlarmId));
                        transaction.QueueCommand(x => rclient.StoreAll((!orattachuris.NullOrEmpty())
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
                    eclient.DeleteRelatedEntities<RELS_EALARMS_ATTACHURIS>(k);
                    eclient.DeleteRelatedEntities<RELS_EALARMS_ATTACHBINS>(k);
                    eclient.DeleteRelatedEntities<RELS_EALARMS_ATTENDEES>(k);

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

    }
}
