using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.concretes.relations;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;
using reexjungle.xmisc.technical.data.concretes.nosql;
using ServiceStack.Common;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace reexjungle.xcal.service.repositories.concretes.redis
{
    /// <summary>
    ///
    /// </summary>
    public class CalendarRedisRepository : ICalendarRepository, IRedisRepository
    {
        private readonly IRedisClientsManager manager;
        private readonly IEventRepository eventrepository;
        private readonly IKeyGenerator<Guid> keygenerator;
        private IRedisClient client;

        private IRedisClient redis => client ?? (client = manager.GetClient());

        /// <summary>
        ///
        /// </summary>
        public IRedisClientsManager RedisClientsManager => manager;

        /// <summary>
        ///
        /// </summary>
        /// <param name="keygenerator"></param>
        /// <param name="eventrepository"></param>
        /// <param name="manager"></param>
        public CalendarRedisRepository(IKeyGenerator<Guid> keygenerator, IEventRepository eventrepository, IRedisClientsManager manager)
        {
            if (keygenerator == null) throw new ArgumentNullException(nameof(keygenerator));
            this.keygenerator = keygenerator;

            if (eventrepository == null) throw new ArgumentNullException(nameof(eventrepository));
            this.eventrepository = eventrepository;

            if (manager == null) throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            if (dry != null)
            {
                var revents = redis.As<REL_CALENDARS_EVENTS>().GetAll()
                    .Where(x => x.CalendarId == dry.Id);
                if (!revents.NullOrEmpty())
                {
                    var keys = revents.Select(x => x.EventId).ToList();
                    var events = eventrepository.FindAll(keys);
                    dry.Events.MergeRange(events);
                }
            }
            return dry;
        }

        public IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry)
        {
            var full = dry.ToList();
            if (!full.NullOrEmpty())
            {
                var keys = dry.Select(x => x.Id).ToArray();
                var revents = redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => keys.Contains(x.CalendarId));
                if (!revents.NullOrEmpty())
                {
                    var events = eventrepository.FindAll(revents.Select(x => x.EventId));
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

        public VCALENDAR Find(Guid key)
        {
            var calendar = redis.As<VCALENDAR>().GetById(key);
            return (calendar != null) ? Hydrate(calendar) : calendar;
        }

        public IEnumerable<VCALENDAR> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            if (skip == null && take == null)
            {
                var calendars = redis.As<VCALENDAR>().GetByIds(keys).ToList();
                return (!calendars.NullOrEmpty()) ? HydrateAll(calendars) : new List<VCALENDAR>();
            }
            else
            {
                var calendars = redis.As<VCALENDAR>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value).ToList();
                return (!calendars.NullOrEmpty()) ? HydrateAll(calendars) : new List<VCALENDAR>();
            }
        }

        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            if (skip == null && take == null) return redis.As<VCALENDAR>().GetAll();
            else
            {
                var calendars = redis.As<VCALENDAR>().GetAll();
                var selected = !calendars.NullOrEmpty()
                    ? calendars.Skip(skip.Value).Take(take.Value).ToList()
                    : null;
                return (!selected.NullOrEmpty()) ? HydrateAll(selected) : new List<VCALENDAR>();
            }
        }

        public void Save(VCALENDAR entity)
        {
            var keys = redis.As<VCALENDAR>().GetAllKeys();
            if (!keys.NullOrEmpty()) redis.Watch(keys.ToArray());
            manager.ExecTrans(transaction =>
            {
                var orevents = redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => x.CalendarId == entity.Id);

                if (!entity.Events.NullOrEmpty())
                {
                    eventrepository.SaveAll(entity.Events.Distinct());
                    var revents = entity.Events.Select(x => new REL_CALENDARS_EVENTS
                    {
                        Id = keygenerator.GetNext(),
                        CalendarId = entity.Id,
                        EventId = x.Id
                    });

                    redis.MergeAll(revents, orevents, transaction);
                }
                else redis.RemoveAll(orevents, transaction);

                transaction.QueueCommand(x => x.Store(Dehydrate(entity)));
            });
        }

        public void Patch(VCALENDAR source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            #region construct anonymous fields using expression lambdas

            var selection = fields as IList<string> ?? fields.ToList();

            Expression<Func<VCALENDAR, object>> primitives = x => new
            {
                x.ProdId,
                x.Method,
                x.Calscale,
                x.Version
            };

            Expression<Func<VCALENDAR, object>> relations = x => new
            {
                x.Events,
                x.ToDos,
                x.FreeBusies,
                x.Journals,
                x.TimeZones,
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection);

            #endregion construct anonymous fields using expression lambdas

            var cclient = redis.As<VCALENDAR>();
            var okeys = cclient.GetAllKeys().ToArray();
            if (!okeys.NullOrEmpty()) redis.Watch(okeys);

            #region save (insert or update) relational attributes

            if (!srelation.NullOrEmpty())
            {
                Expression<Func<VCALENDAR, object>> eventsexr = y => y.Events;
                if (selection.Contains(eventsexr.GetMemberName()))
                {
                    var events = source.Events;
                    manager.ExecTrans(transaction =>
                    {
                        var orevents = redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => keys.Contains(x.CalendarId));
                        if (!events.NullOrEmpty())
                        {
                            eventrepository.SaveAll(events.Distinct());
                            var revents = keys.SelectMany(x => events.Select(y => new REL_CALENDARS_EVENTS
                            {
                                Id = keygenerator.GetNext(),
                                CalendarId = x,
                                EventId = y.Id
                            }));

                            redis.MergeAll(revents, orevents, transaction);
                        }
                        else redis.RemoveAll(orevents, transaction);
                    });
                }
            }

            #endregion save (insert or update) relational attributes

            #region update-only non-relational attributes

            if (!sprimitives.NullOrEmpty())
            {
                Expression<Func<VCALENDAR, object>> prodexpr = x => x.ProdId;
                Expression<Func<VCALENDAR, object>> versionexpr = x => x.Version;
                Expression<Func<VCALENDAR, object>> scaleexpr = x => x.Calscale;
                Expression<Func<VCALENDAR, object>> methodexpr = x => x.Method;

                var entities = cclient.GetByIds(keys).ToList();
                manager.ExecTrans(transaction =>
                {
                    entities.ForEach(x =>
                    {
                        if (selection.Contains(prodexpr.GetMemberName())) x.ProdId = source.ProdId;
                        if (selection.Contains(versionexpr.GetMemberName())) x.Version = source.Version;
                        if (selection.Contains(scaleexpr.GetMemberName())) x.Calscale = source.Calscale;
                        if (selection.Contains(methodexpr.GetMemberName())) x.Method = source.Method;
                    });
                    transaction.QueueCommand(x => x.StoreAll(DehydrateAll(entities)));
                });
            }

            #endregion update-only non-relational attributes
        }

        public void Erase(Guid key)
        {
            var ukey = UrnId.Create<VCALENDAR>(key).ToLower();
            if (redis.As<VCALENDAR>().ContainsKey(ukey))
            {
                var revents = redis.As<REL_CALENDARS_EVENTS>().GetAll();
                var rtodos = redis.As<REL_CALENDARS_TODOS>().GetAll();
                var rfreebusies = redis.As<REL_CALENDARS_FREEBUSIES>().GetAll();
                var rtimezones = redis.As<REL_CALENDARS_TIMEZONES>().GetAll();
                var rjournals = redis.As<REL_CALENDARS_JOURNALS>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!revents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>()
                        .DeleteByIds(revents.Where(x => x.CalendarId == key)));

                    if (!rtodos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>()
                        .DeleteByIds(rtodos.Where(x => x.CalendarId == key)));

                    if (!rfreebusies.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>()
                        .DeleteByIds(rfreebusies.Where(x => x.CalendarId == key)));

                    if (!rtimezones.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>()
                        .DeleteByIds(rtimezones.Where(x => x.CalendarId == key)));

                    if (!rjournals.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>()
                        .DeleteByIds(rjournals.Where(x => x.CalendarId == key)));

                    transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteById(key));
                });
            }
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            var keys = redis.As<VCALENDAR>().GetAllKeys();
            if (!keys.NullOrEmpty()) redis.Watch(keys.ToArray());

            //save events of calendar
            var calendars = entities as IList<VCALENDAR> ?? entities.ToList();
            var events = calendars.SelectMany(x => x.Events).ToList();
            manager.ExecTrans(transaction =>
            {
                var okeys = calendars.Select(x => x.Id);
                var orevents = redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => okeys.Contains(x.CalendarId));

                if (!events.NullOrEmpty())
                {
                    eventrepository.SaveAll(events.Distinct());
                    var revents = calendars.SelectMany(x => x.Events.Select(y => new REL_CALENDARS_EVENTS
                    {
                        Id = keygenerator.GetNext(),
                        CalendarId = x.Id,
                        EventId = y.Id
                    }));

                    redis.MergeAll(revents, orevents, transaction);
                }
                else redis.RemoveAll(orevents, transaction);

                transaction.QueueCommand(x => x.StoreAll(DehydrateAll(calendars)));
            });
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            if (!keys.NullOrEmpty())
            {
                var revents = redis.As<REL_CALENDARS_EVENTS>().GetAll();
                var rtodos = redis.As<REL_CALENDARS_TODOS>().GetAll();
                var rfreebusies = redis.As<REL_CALENDARS_FREEBUSIES>().GetAll();
                var rtimezones = redis.As<REL_CALENDARS_TIMEZONES>().GetAll();
                var rjournals = redis.As<REL_CALENDARS_JOURNALS>().GetAll();

                manager.ExecTrans(transaction =>
                {
                    if (!revents.NullOrEmpty() &&
                        !revents.Where(x => keys.Contains(x.CalendarId)).NullOrEmpty())
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>().DeleteByIds(revents));

                    if (!rtodos.NullOrEmpty() &&
                        !rtodos.Where(x => keys.Contains(x.CalendarId)).NullOrEmpty())
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>().DeleteByIds(revents));

                    if (!rfreebusies.NullOrEmpty() &&
                        !rfreebusies.Where(x => keys.Contains(x.CalendarId)).NullOrEmpty())
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>().DeleteByIds(revents));

                    if (!rtimezones.NullOrEmpty() &&
                        !rtimezones.Where(x => keys.Contains(x.CalendarId)).NullOrEmpty())
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>().DeleteByIds(revents));

                    if (!rjournals.NullOrEmpty() &&
                        !rjournals.Where(x => keys.Contains(x.CalendarId)).NullOrEmpty())
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>().DeleteByIds(revents));

                    transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteByIds(keys));
                });
            }
            else
            {
                manager.ExecTrans(transaction =>
                {
                    transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>().DeleteAll());
                    transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>().DeleteAll());
                    transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteAll());
                });
            }
        }

        public bool ContainsKey(Guid key)
        {
            return redis.As<VCALENDAR>().ContainsKey(UrnId.Create<VCALENDAR>(key).ToLower());
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            var all = redis.As<VCALENDAR>().GetAllKeys();
            var matches = all.Intersect(keys.Select(x => UrnId.Create<VCALENDAR>(x)).ToList());
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.Pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            var dry = full;
            if (!dry.Events.NullOrEmpty()) dry.Events.Clear();
            if (!dry.ToDos.NullOrEmpty()) dry.ToDos.Clear();
            if (!dry.FreeBusies.NullOrEmpty()) dry.FreeBusies.Clear();
            if (!dry.Journals.NullOrEmpty()) dry.Journals.Clear();
            if (!dry.TimeZones.NullOrEmpty()) dry.TimeZones.Clear();
            return dry;
        }

        public IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full)
        {
            var pquery = full.AsParallel();
            pquery.ForAll(x => Dehydrate(x));
            return pquery.AsEnumerable();
        }

        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            var keys = redis.As<VCALENDAR>().GetAllKeys();
            if (skip == null && take == null)
                return !keys.NullOrEmpty()
                    ? keys.Select(UrnId.GetGuidId)
                    : new List<Guid>();

            return (!keys.NullOrEmpty())
                ? keys.Select(UrnId.GetGuidId).Skip(skip.Value + 1).Take(take.Value)
                : new List<Guid>();
        }
    }
}