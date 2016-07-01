using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.nosql;
using ServiceStack.Common.Extensions;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    public class AudioAlarmRedisRepository : IAudioAlarmRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private readonly IKeyGenerator<Guid> keygenerator;
        private IRedisClient client;

        private IRedisClient redis => client ?? (client = manager.GetClient());

        public IRedisClientsManager RedisClientsManager => manager;

        public AudioAlarmRedisRepository(IKeyGenerator<Guid> keygenerator, IRedisClientsManager manager)
        {
            if (keygenerator == null) throw new ArgumentNullException(nameof(keygenerator));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            this.keygenerator = keygenerator;
            this.manager = manager;
        }

        public AUDIO_ALARM Find(Guid key)
        {
            return redis.As<AUDIO_ALARM>().GetById(key);
        }

        public IEnumerable<AUDIO_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            return skip == null || take == null
                ? redis.As<AUDIO_ALARM>().GetByIds(keys)
                : redis.As<AUDIO_ALARM>().GetByIds(keys).Skip(skip.Value).Take(take.Value);
        }

        public IEnumerable<AUDIO_ALARM> Get(int? skip = null, int? take = null)
        {
            return skip == null || take == null
                ? redis.As<AUDIO_ALARM>().GetAll()
                : redis.As<AUDIO_ALARM>().GetAll().Skip(skip.Value).Take(take.Value);
        }

        public bool ContainsKey(Guid key)
        {
            return redis.As<AUDIO_ALARM>().ContainsKey(key.ToString());
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var matches = redis.As<AUDIO_ALARM>().GetAllKeys().Intersect(keys.Select(x => x.ToString())).ToList();
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.Pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(AUDIO_ALARM entity)
        {
            if (entity.Attachment == null) return;

            manager.ExecTrans(transaction =>
            {
                var orattachbins = redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == entity.Id);
                var orattachuris = redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == entity.Id);
                var attachbin = entity.Attachment as ATTACH_BINARY;
                if (attachbin != null)
                {
                    transaction.QueueCommand(x => x.Store(attachbin));
                    var rattachbin = new REL_AALARMS_ATTACHBINS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = entity.Id,
                        AttachmentId = attachbin.Id
                    };

                    redis.MergeAll(rattachbin.ToSingleton(), orattachbins, transaction);
                }
                else redis.RemoveAll(orattachbins, transaction);

                var attachuri = entity.Attachment as ATTACH_URI;
                if (attachuri != null)
                {
                    transaction.QueueCommand(x => x.Store(attachuri));
                    var rattachuri = new REL_AALARMS_ATTACHURIS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = entity.Id,
                        AttachmentId = attachuri.Id
                    };

                    redis.MergeAll(rattachuri.ToSingleton(), orattachuris, transaction);
                }
                else redis.RemoveAll(orattachuris, transaction);


                transaction.QueueCommand(x => x.Store(Dehydrate(entity)));
            });
        }

        public void Patch(AUDIO_ALARM source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields as IList<string> ?? fields.ToList();

            Expression<Func<AUDIO_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Duration,
                x.Trigger,
                x.Repeat
            };

            Expression<Func<AUDIO_ALARM, object>> relations = x => new
            {
                x.Attachment,
            };

            var sprimitives = primitives.GetMemberNames().Intersect(selection);
            var srelations = relations.GetMemberNames().Intersect(selection);

            #endregion construct anonymous fields using expression lambdas

            manager.ExecTrans(transaction =>
            {
                var aclient = redis.As<AUDIO_ALARM>();
                var okeys = aclient.GetAllKeys().ToArray();
                if (!okeys.NullOrEmpty()) redis.Watch(okeys);

                #region save (insert or update) normalized attributes

                if (!srelations.NullOrEmpty())
                {
                    Expression<Func<EMAIL_ALARM, object>> attachsexpr = y => y.Attachments;

                    var orattachbins = redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                    var orattachuris = redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                    if (srelations.Contains(attachsexpr.GetMemberName()))
                    {
                        var attachbin = source.Attachment as ATTACH_BINARY;
                        if (attachbin != null)
                        {
                            transaction.QueueCommand(x => redis.As<ATTACH_BINARY>().Store(attachbin));
                            var rattachbins = keys.Select(x => new REL_AALARMS_ATTACHBINS
                            {
                                Id = keygenerator.GetNext(),
                                AlarmId = x,
                                AttachmentId = attachbin.Id
                            });

                            redis.MergeAll(rattachbins, orattachbins, transaction);
                        }
                        else redis.RemoveAll(orattachbins, transaction);

                        var attachuri = source.Attachment as ATTACH_URI;
                        if (attachuri != null)
                        {
                            transaction.QueueCommand(x => redis.As<ATTACH_URI>().Store(attachuri));
                            var rattachuris = keys.Select(x => new REL_AALARMS_ATTACHURIS
                            {
                                Id = keygenerator.GetNext(),
                                AlarmId = x,
                                AttachmentId = attachuri.Id
                            });

                            redis.MergeAll(rattachuris, orattachuris, transaction);
                        }
                        else redis.RemoveAll(orattachuris, transaction);
                    }
                }

                #endregion save (insert or update) normalized attributes

                #region save (insert or update) non-normalized attributes

                var lsprimitives = sprimitives as IList<string> ?? sprimitives.ToList();
                if (!lsprimitives.NullOrEmpty())
                {
                    Expression<Func<AUDIO_ALARM, object>> actionexpr = x => x.Action;
                    Expression<Func<AUDIO_ALARM, object>> repeatexpr = x => x.Repeat;
                    Expression<Func<AUDIO_ALARM, object>> durationexpr = x => x.Duration;
                    Expression<Func<AUDIO_ALARM, object>> triggerexpr = x => x.Trigger;

                    var entities = aclient.GetByIds(keys).ToList();
                    entities.ForEach(x =>
                    {
                        if (lsprimitives.Contains(actionexpr.GetMemberName())) x.Action = source.Action;
                        if (lsprimitives.Contains(repeatexpr.GetMemberName())) x.Repeat = source.Repeat;
                        if (lsprimitives.Contains(durationexpr.GetMemberName())) x.Duration = source.Duration;
                        if (lsprimitives.Contains(triggerexpr.GetMemberName())) x.Trigger = source.Trigger;
                    });

                    transaction.QueueCommand(x => x.StoreAll(entities));
                }

                #endregion save (insert or update) non-normalized attributes
            });
        }

        public void Erase(Guid key)
        {
            if (redis.As<AUDIO_ALARM>().ContainsKey(key.ToString()))
            {
                var rattachbins = redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_AALARMS_ATTACHURIS>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => x.AlarmId == key)));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => x.AlarmId == key)));

                    transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteById(key));
                });
            }
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
        {
            var keys = redis.As<AUDIO_ALARM>().GetAllKeys().ToArray();
            if (!keys.NullOrEmpty()) redis.Watch(keys);

            manager.ExecTrans(transaction =>
            {
                if (!entities.NullOrEmpty()) transaction.QueueCommand(x => x.StoreAll(entities.Distinct()));
            });
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty())
            {
                var rattachbins = redis.As<REL_AALARMS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_AALARMS_ATTACHURIS>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => keys.Contains(x.AlarmId))));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => keys.Contains(x.AlarmId))));

                    transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteByIds(keys));
                });
            }
            else
            {
                manager.ExecTrans(transaction =>
                {
                    transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHBINS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_AALARMS_ATTACHURIS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteAll());
                });
            }
        }

        public AUDIO_ALARM Hydrate(AUDIO_ALARM alarm)
        {
            var match = redis.As<AUDIO_ALARM>().GetValue(alarm.Id.ToString());
            if (match != null)
            {
                var rattachbins = redis.As<REL_AALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == match.Id).ToList();
                var rattachuris = redis.As<REL_AALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == match.Id).ToList();

                if (rattachbins.Any())
                {
                    match.Attachment = redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId.ToString()).ToList()).First();
                }
                if (rattachuris.Any())
                {
                    match.Attachment = redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId.ToString()).ToList()).First();
                }
            }
            return match ?? alarm;
        }

        public IEnumerable<AUDIO_ALARM> HydrateAll(IEnumerable<AUDIO_ALARM> alarms)
        {
            var eclient = redis.As<AUDIO_ALARM>();
            var audioAlarms = alarms as IList<AUDIO_ALARM> ?? alarms.ToList();
            var keys = audioAlarms.Select(x => x.Id).Distinct().ToList();

            if (eclient.GetAllKeys().Intersect(keys.Select(x => x.ToString())).Count() != keys.Count()) return audioAlarms;

            alarms = eclient.GetValues(keys.Select(x => x.ToString()).ToList());

            #region 1. retrieve relationships

            var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();
            var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId)).ToList();

            #endregion 1. retrieve relationships

            #region 2. retrieve secondary entities

            var attachbins = (!rattachbins.Empty()) ? redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId.ToString()).ToList()) : null;
            var attachuris = (!rattachuris.Empty()) ? redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId.ToString()).ToList()) : null;

            #endregion 2. retrieve secondary entities

            #region 3. Use Linq to stitch secondary entities to primary entities

            audioAlarms.ForEach(x =>
            {
                if (!attachbins.NullOrEmpty())
                {
                    var xattachbins = from y in attachbins
                                      join r in rattachbins on y.Id equals r.AttachmentId
                                      join e in audioAlarms on r.EventId equals e.Id
                                      where e.Id == x.Id
                                      select y;
                    if (!xattachbins.NullOrEmpty()) x.Attachment = xattachbins.First();
                }

                if (!attachuris.NullOrEmpty())
                {
                    var xattachuris = from y in attachuris
                                      join r in rattachuris on y.Id equals r.AttachmentId
                                      join e in audioAlarms on r.EventId equals e.Id
                                      where e.Id == x.Id
                                      select y;
                    if (!xattachuris.NullOrEmpty()) x.Attachment = xattachuris.First();
                }
            });

            #endregion 3. Use Linq to stitch secondary entities to primary entities

            return alarms;
        }

        public AUDIO_ALARM Dehydrate(AUDIO_ALARM alarm)
        {
            alarm.Attachment = null;
            return alarm;
        }

        public IEnumerable<AUDIO_ALARM> DehydrateAll(IEnumerable<AUDIO_ALARM> alarms)
        {
            var pquery = alarms.AsParallel();
            pquery.ForAll(x => Dehydrate(x));
            return pquery.AsEnumerable();
        }
    }

    public class DisplayAlarmRedisRepository : IDisplayAlarmRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private IRedisClient client = null;

        private IRedisClient redis => client ?? (client = manager.GetClient());

        public IRedisClientsManager RedisClientsManager => manager;

        public DisplayAlarmRedisRepository(IRedisClientsManager manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
        }

        public DISPLAY_ALARM Find(Guid key)
        {
            return redis.As<DISPLAY_ALARM>().GetById(key);
        }

        public IEnumerable<DISPLAY_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            if (skip == null || take == null) return redis.As<DISPLAY_ALARM>().GetByIds(keys);
            return redis.As<DISPLAY_ALARM>().GetByIds(keys).Skip(skip.Value).Take(take.Value);
        }

        public IEnumerable<DISPLAY_ALARM> Get(int? skip = null, int? take = null)
        {
            if (skip == null || take == null)
                return redis.As<DISPLAY_ALARM>().GetAll();

            var alarms = redis.As<DISPLAY_ALARM>().GetAll();
            var selected = !alarms.NullOrEmpty()
                ? alarms.Skip(skip.Value).Take(take.Value)
                : alarms;
            return selected ?? new List<DISPLAY_ALARM>();
        }

        public bool ContainsKey(Guid key)
        {
            return redis.As<DISPLAY_ALARM>().ContainsKey(key.ToString());
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var matches = redis.As<DISPLAY_ALARM>().GetAllKeys().Intersect(keys.Select(x => x.ToString())).ToList();
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.Pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(DISPLAY_ALARM entity)
        {
            var keys = redis.As<DISPLAY_ALARM>().GetAllKeys().ToArray();
            if (!keys.NullOrEmpty()) redis.Watch(keys);

            manager.ExecTrans(transaction =>
            {
                if (entity != null) transaction.QueueCommand(x => x.Store(entity));
            });
        }

        public void Patch(DISPLAY_ALARM source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields as IList<string> ?? fields.ToList();

            Expression<Func<DISPLAY_ALARM, object>> primitives = x => new
            {
                x.Action,
                x.Duration,
                x.Trigger,
                x.Repeat,
                x.Description
            };

            //4. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection);

            #endregion construct anonymous fields using expression lambdas

            manager.ExecTrans(transaction =>
            {
                var daclient = redis.As<DISPLAY_ALARM>();

                var okeys = daclient.GetAllKeys().ToArray();
                if (!okeys.NullOrEmpty()) redis.Watch(okeys);

                #region update-only non-relational attributes

                if (!sprimitives.NullOrEmpty())
                {
                    Expression<Func<DISPLAY_ALARM, object>> actionexpr = x => x.Action;
                    Expression<Func<DISPLAY_ALARM, object>> repeatexpr = x => x.Repeat;
                    Expression<Func<DISPLAY_ALARM, object>> durationexpr = x => x.Duration;
                    Expression<Func<DISPLAY_ALARM, object>> triggerexpr = x => x.Trigger;
                    Expression<Func<DISPLAY_ALARM, object>> descexpr = x => x.Description;

                    var lkeys = keys as IList<Guid> ?? keys.ToList();
                    var entities = !lkeys.NullOrEmpty() ? daclient.GetByIds(lkeys).ToList() : daclient.GetAll().ToList();
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

        public void Erase(Guid key)
        {
            var daclient = redis.As<DISPLAY_ALARM>();
            if (daclient.ContainsKey(key.ToString())) daclient.DeleteById(key);
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
        {
            var keys = redis.As<DISPLAY_ALARM>().GetAllKeys().ToArray();
            if (!keys.NullOrEmpty()) redis.Watch(keys);

            manager.ExecTrans(transaction =>
            {
                if (!entities.NullOrEmpty()) transaction.QueueCommand(x => x.StoreAll(entities.Distinct()));
            });
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            manager.ExecTrans(transaction =>
            {
                if (!keys.NullOrEmpty()) transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteByIds(keys));
                else transaction.QueueCommand(t => t.As<AUDIO_ALARM>().DeleteAll());
            });
        }
    }

    public class EmailAlarmRedisRepository : IEmailAlarmRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private readonly IKeyGenerator<Guid> keygenerator;
        private IRedisClient client = null;

        private IRedisClient redis => client ?? (client = manager.GetClient());

        public IRedisClientsManager RedisClientsManager => manager;

        public EmailAlarmRedisRepository(IKeyGenerator<Guid> keygenerator, IRedisClientsManager manager)
        {
            if (keygenerator == null) throw new ArgumentNullException(nameof(keygenerator));
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            this.keygenerator = keygenerator;
            this.manager = manager;
        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM alarm)
        {
            var full = redis.As<EMAIL_ALARM>().GetValue(alarm.Id.ToString());
            if (full != null)
            {
                var rattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == full.Id).ToList();
                var rattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == full.Id).ToList();
                var rattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == full.Id).ToList();

                if (!rattachbins.NullOrEmpty())
                {
                    full.Attachments.MergeRange(redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId.ToString()).ToList()));
                }
                if (!rattachuris.NullOrEmpty())
                {
                    full.Attachments.MergeRange(redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId.ToString()).ToList()));
                }
                if (!rattendees.NullOrEmpty())
                {
                    full.Attendees.MergeRange(redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId.ToString()).ToList()));
                }
            }
            return full ?? alarm;
        }

        public IEnumerable<EMAIL_ALARM> HydrateAll(IEnumerable<EMAIL_ALARM> alarms)
        {
            var full = alarms.ToList();
            var eclient = redis.As<EMAIL_ALARM>();
            var keys = alarms.Select(x => x.Id.ToString()).Distinct().ToList();
            if (eclient.GetAllKeys().Intersect(keys).Count() == keys.Count()) //all keys are found
            {
                full = eclient.GetValues(keys);

                #region 1. retrieve relationships

                var rattachbins = redis.As<REL_EVENTS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.EventId.ToString())).ToList();
                var rattachuris = redis.As<REL_EVENTS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.EventId.ToString())).ToList();
                var rattendees = redis.As<REL_EVENTS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.EventId.ToString())).ToList();

                #endregion 1. retrieve relationships

                #region 2. retrieve secondary entities

                var attachbins = (!rattachbins.Empty()) ? redis.As<ATTACH_BINARY>().GetValues(rattachbins.Select(x => x.AttachmentId.ToString()).ToList()) : null;
                var attachuris = (!rattachuris.Empty()) ? redis.As<ATTACH_URI>().GetValues(rattachuris.Select(x => x.AttachmentId.ToString()).ToList()) : null;
                var attendees = (!rattendees.Empty()) ? redis.As<ATTENDEE>().GetValues(rattendees.Select(x => x.AttendeeId.ToString()).ToList()) : null;

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
                        if (!xattachbins.NullOrEmpty()) x.Attachments.MergeRange(xattachbins);
                    }

                    if (!attachuris.NullOrEmpty())
                    {
                        var xattachuris = from y in attachuris
                                          join r in rattachuris on y.Id equals r.AttachmentId
                                          join e in full on r.EventId equals e.Id
                                          where e.Id == x.Id
                                          select y;
                        if (!xattachuris.NullOrEmpty()) x.Attachments.MergeRange(xattachuris);
                    }
                });

                #endregion 3. Use Linq to stitch secondary entities to primary entities
            }

            return full;
        }

        public EMAIL_ALARM Find(Guid key)
        {
            var alarm = redis.As<EMAIL_ALARM>().GetById(key);
            return alarm != null ? Hydrate(alarm) : alarm;
        }

        public IEnumerable<EMAIL_ALARM> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            if (skip == null || take == null)
            {
                var dry = redis.As<EMAIL_ALARM>().GetByIds(keys);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
            }
            else
            {
                var dry = redis.As<EMAIL_ALARM>().GetByIds(keys).Skip(skip.Value).Take(take.Value);
                return !dry.NullOrEmpty() ? HydrateAll(dry) : dry;
            }
        }

        public IEnumerable<EMAIL_ALARM> Get(int? skip = null, int? take = null)
        {
            if (skip == null || take == null)
                return redis.As<EMAIL_ALARM>().GetAll();

            var alarms = redis.As<EMAIL_ALARM>().GetAll();
            var selected = !alarms.NullOrEmpty()
                ? alarms.Skip(skip.Value).Take(take.Value).ToList()
                : alarms;
            return !selected.NullOrEmpty() ? HydrateAll(selected) : new List<EMAIL_ALARM>();
        }

        public bool ContainsKey(Guid key)
        {
            return redis.As<EMAIL_ALARM>().ContainsKey(key.ToString());
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var lkeys = keys as IList<Guid> ?? keys.ToList();
            var matches = redis.As<EMAIL_ALARM>().GetAllKeys().Intersect(lkeys.Select(x => x.ToString())).ToList();
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.Pessimistic
                ? matches.Count() == lkeys.Count()
                : !matches.NullOrEmpty();
        }

        public void Save(EMAIL_ALARM entity)
        {
            manager.ExecTrans(transaction =>
            {
                #region retrieve attributes of entity

                var attendees = entity.Attendees;
                var attachbins = entity.Attachments.OfType<ATTACH_BINARY>();
                var attachuris = entity.Attachments.OfType<ATTACH_URI>();

                #endregion retrieve attributes of entity

                #region save normalized and non-normalized attributes

                var orattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == entity.Id);
                var orattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => x.AlarmId == entity.Id);
                var orattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => x.AlarmId == entity.Id);

                if (!attendees.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                    var rattendees = attendees.Select(x => new REL_EALARMS_ATTENDEES
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = entity.Id,
                        AttendeeId = x.Id
                    });

                    redis.MergeAll(rattendees, orattendees, transaction);
                }
                else redis.RemoveAll(orattendees, transaction);

                if (!attachbins.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                    var rattachbins = attachbins.Select(x => new REL_EALARMS_ATTACHBINS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = entity.Id,
                        AttachmentId = x.Id
                    });

                    redis.MergeAll(rattachbins, orattachbins, transaction);
                }
                else redis.RemoveAll(orattachbins, transaction);

                if (!attachuris.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                    var rattachuris = attachuris.Select(x => new REL_EALARMS_ATTACHURIS
                    {
                        Id = keygenerator.GetNext(),
                        AlarmId = entity.Id,
                        AttachmentId = x.Id
                    });

                    redis.MergeAll(rattachuris, orattachuris, transaction);
                }
                else redis.RemoveAll(orattachuris, transaction);

                #endregion save normalized and non-normalized attributes

                transaction.QueueCommand(x => x.Store(Dehydrate(entity)));
            });
        }

        public void Patch(EMAIL_ALARM source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields as IList<string> ?? fields.ToList();

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
            var srelation = relations.GetMemberNames().Intersect(selection).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion construct anonymous fields using expression lambdas

            manager.ExecTrans(transaction =>
            {
                var eaclient = redis.As<EMAIL_ALARM>();
                var okeys = eaclient.GetAllKeys().ToArray();
                if (!okeys.NullOrEmpty()) redis.Watch(okeys);

                #region save (insert or update) relational attributes

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<EMAIL_ALARM, object>> attendsexpr = y => y.Attendees;
                    Expression<Func<EMAIL_ALARM, object>> attachsexpr = y => y.Attachments;

                    var orattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => x.AlarmId == source.Id);
                    var orattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                    var orattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                    if (selection.Contains(attendsexpr.GetMemberName()))
                    {
                        var attendees = source.Attendees;
                        if (!attendees.NullOrEmpty())
                        {
                            transaction.QueueCommand(x => redis.As<ATTENDEE>().StoreAll(attendees));
                            var rattendees = keys.SelectMany(x => attendees.Select(y => new REL_EALARMS_ATTENDEES
                            {
                                Id = keygenerator.GetNext(),
                                AlarmId = x,
                                AttendeeId = y.Id
                            }));

                            redis.MergeAll(rattendees, orattendees, transaction);
                        }
                        else redis.RemoveAll(orattendees, transaction);
                    }

                    if (selection.Contains(attachsexpr.GetMemberName()))
                    {
                        var attachbins = source.Attachments.OfType<ATTACH_BINARY>();
                        if (attachbins.Any())
                        {
                            transaction.QueueCommand(x => redis.As<ATTACH_BINARY>().StoreAll(attachbins));
                            var rattachbins = keys.SelectMany(x => attachbins.Select(y => new REL_EALARMS_ATTACHBINS
                            {
                                Id = keygenerator.GetNext(),
                                AlarmId = x,
                                AttachmentId = y.Id
                            }));

                            redis.MergeAll(rattachbins, orattachbins, transaction);
                        }
                        else redis.RemoveAll(orattachbins, transaction);

                        var attachuris = source.Attachments.OfType<ATTACH_URI>();
                        if (attachuris.Any())
                        {
                            transaction.QueueCommand(x => redis.As<ATTACH_URI>().StoreAll(attachuris));
                            var rattachuris = keys.SelectMany(x => attachuris.Select(y => new REL_EALARMS_ATTACHURIS
                            {
                                Id = keygenerator.GetNext(),
                                AlarmId = x,
                                AttachmentId = y.Id
                            }));
                            redis.MergeAll(rattachuris, orattachuris, transaction);
                        }
                        else redis.RemoveAll(orattachuris, transaction);
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

                    transaction.QueueCommand(x => x.StoreAll(DehydrateAll(entities)));
                }

                #endregion save (insert or update) non-relational attributes
            });
        }

        public void Erase(Guid key)
        {
            if (redis.As<EMAIL_ALARM>().ContainsKey(key.ToString()))
            {
                var rattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll();
                var rattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => x.AlarmId == key)));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => x.AlarmId == key)));

                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => x.AlarmId == key)));

                    transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteById(key));
                });
            }
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
        {
            #region 1. retrieve attributes of entities

            var lentities = entities as IList<EMAIL_ALARM> ?? entities.ToList();
            var attendees = lentities.SelectMany(x => x.Attendees);
            var attachbins = lentities.SelectMany(x => x.Attachments.OfType<ATTACH_BINARY>());
            var attachuris = lentities.SelectMany(x => x.Attachments.OfType<ATTACH_URI>());

            #endregion 1. retrieve attributes of entities

            #region 2. save aggregate attribbutes of entities

            manager.ExecTrans(transaction =>
            {
                var keys = lentities.Select(x => x.Id).ToArray();

                var orattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll().Where(x => keys.Contains(x.AlarmId));
                var orattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll().Where(x => keys.Contains(x.AlarmId));
                var orattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll().Where(x => keys.Contains(x.AlarmId));

                if (!attendees.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attendees.Distinct()));
                    var rattendees = lentities.Where(x => !x.Attendees.NullOrEmpty())
                        .SelectMany(e => e.Attendees.Select(x => new REL_EALARMS_ATTENDEES
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = e.Id,
                            AttendeeId = x.Id
                        }));

                    redis.MergeAll(rattendees, orattendees, transaction);
                }
                else redis.RemoveAll(orattendees, transaction);

                if (!attachbins.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachbins.Distinct()));
                    var rattachbins = lentities.SelectMany(e => e.Attachments.Select(x => new REL_EALARMS_ATTACHBINS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = e.Id,
                            AttachmentId = x.Id
                        }));

                    redis.MergeAll(rattachbins, orattachbins, transaction);
                }
                else redis.RemoveAll(orattachbins, transaction);

                if (!attachuris.NullOrEmpty())
                {
                    transaction.QueueCommand(x => x.StoreAll(attachuris.Distinct()));
                    var rattachuris = lentities.SelectMany(e => attachuris.Select(x => new REL_EALARMS_ATTACHURIS
                        {
                            Id = keygenerator.GetNext(),
                            AlarmId = e.Id,
                            AttachmentId = x.Id
                        }));
                    redis.MergeAll(rattachuris, orattachuris, transaction);
                }
                else redis.RemoveAll(orattachuris, transaction);

                transaction.QueueCommand(x => x.StoreAll(DehydrateAll(lentities)));
            });

            #endregion 2. save aggregate attribbutes of entities
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty())
            {
                var rattachbins = redis.As<REL_EALARMS_ATTACHBINS>().GetAll();
                var rattachuris = redis.As<REL_EALARMS_ATTACHURIS>().GetAll();
                var rattendees = redis.As<REL_EALARMS_ATTENDEES>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!rattachbins.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>()
                        .DeleteByIds(rattachbins.Where(x => keys.Contains(x.AlarmId))));

                    if (!rattachuris.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>()
                        .DeleteByIds(rattachuris.Where(x => keys.Contains(x.AlarmId))));

                    if (!rattendees.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>()
                        .DeleteByIds(rattendees.Where(x => keys.Contains(x.AlarmId))));

                    transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteByIds(keys));
                });
            }
            else
            {
                manager.ExecTrans(transaction =>
                {
                    transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHBINS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_EALARMS_ATTACHURIS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_EALARMS_ATTENDEES>().DeleteAll());
                    transaction.QueueCommand(t => t.As<EMAIL_ALARM>().DeleteAll());
                });
            }
        }

        public EMAIL_ALARM Dehydrate(EMAIL_ALARM alarm)
        {
            alarm.Attendees.Clear();
            alarm.Attachments.Clear();
            return alarm;
        }

        public IEnumerable<EMAIL_ALARM> DehydrateAll(IEnumerable<EMAIL_ALARM> alarms)
        {
            return alarms.Select(Dehydrate);
        }
    }
}