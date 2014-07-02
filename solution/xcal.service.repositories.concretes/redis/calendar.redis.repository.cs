using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Common;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.io.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.xcal.service.repositories.concretes.relations;
using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.technical.data.concretes.extensions.redis;

namespace reexmonkey.xcal.service.repositories.concretes.redis
{
    public class CalendarRedisRepository : ICalendarRedisRepository
    {
        private IRedisClientsManager manager;
        private IEventRepository eventrepository;
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
                this.client = this.manager.GetClient();
            }
        }

        public IEventRepository EventRepository
        {
            get { return this.eventrepository; }
            set
            {
                if (value == null) throw new ArgumentNullException("EventRepository");
                this.eventrepository = value;
            }
        }

        public CalendarRedisRepository()  { }

        public CalendarRedisRepository(IRedisClientsManager manager, IEventRepository eventrepository)
        {
            this.EventRepository = eventrepository;
            this.RedisClientsManager = manager;
        }

        public CalendarRedisRepository(IRedisClient client, IEventRepository eventrepository)
        {
            if (client == null) throw new ArgumentNullException("IRedisClient");
            this.client = client;
            this.EventRepository = eventrepository;
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            var full = dry;
            if (full != null)
            {
                var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll()
                    .Where(x => x.CalendarId.Equals(full.Id, StringComparison.OrdinalIgnoreCase));
                if (!revents.NullOrEmpty())
                {
                    var keys = revents.Select(x => x.EventId).ToList();
                    var events = this.EventRepository.FindAll(keys);
                    full.Events.AddRangeComplement(events);
                }
            }
            return full ?? dry;
        }

        public IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry)
        {
            var full = dry.ToList();
            if (!full.NullOrEmpty())
            {
                var keys = dry.Select(x => x.Id).ToArray();
                var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => keys.Contains(x.CalendarId));
                if(!revents.NullOrEmpty())
                {
                   var events = this.EventRepository.FindAll(revents.Select(x => x.EventId));
                   full.ForEach(x =>
                   {
                       var xevents = from y in events
                                     join r in revents on y.Id equals r.EventId
                                     join c in full on r.CalendarId equals c.Id
                                     where c.Id == x.Id
                                     select y;
                       if (!xevents.NullOrEmpty()) x.Events.AddRangeComplement(xevents);
                   });
                }
            }
            return full ?? dry;
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

        public VCALENDAR Find(string key)
        {
            var calendar = this.redis.As<VCALENDAR>().GetById(key);
            return (calendar != null) ? this.Hydrate(calendar) : calendar;
        }

        public IEnumerable<VCALENDAR> FindAll(IEnumerable<string> keys, int? skip = null, int? take = null)
        {
            if (skip == null && take == null)
            {
                var calendars = this.redis.As<VCALENDAR>().GetByIds(keys);
                return (!calendars.NullOrEmpty()) ? this.HydrateAll(calendars): new List<VCALENDAR>();
            }
            else
            {
                var calendars = this.redis.As<VCALENDAR>().GetByIds(keys).Skip(skip.Value + 1).Take(take.Value);
                return (!calendars.NullOrEmpty()) ? this.HydrateAll(calendars) : new List<VCALENDAR>();
            }

        }

        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            if (skip == null && take == null) return this.redis.As<VCALENDAR>().GetAll();
            else
            {
                var keys = this.redis.As<VCALENDAR>().GetAllKeys().Where(k => !k.StartsWith("ids"));
                var selected = !keys.NullOrEmpty()
                    ? keys.Skip(skip.Value).Take(take.Value)
                    : keys;
                var calendars = this.redis.As<VCALENDAR>().GetValues(selected.ToList());
                return (!calendars.NullOrEmpty()) ? this.HydrateAll(calendars) : new List<VCALENDAR>();
            }
        }

        public void Save(VCALENDAR entity)
        {
            try
            {
                var keys = this.redis.As<VCALENDAR>().GetAllKeys();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys.ToArray());
                this.manager.ExecTrans(transaction =>
                {

                    if (!entity.Events.NullOrEmpty())
                    {
                        var events = entity.Events;
                        this.EventRepository.SaveAll(events);
                        var revents = events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            CalendarId = entity.Id,
                            EventId = x.Id
                        });

                        var orevents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => x.CalendarId == entity.Id);
                        this.redis.SynchronizeAll<REL_CALENDARS_EVENTS>(revents, orevents, transaction);
                    }
                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));
                });
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, IEnumerable<string> keys = null)
        {
            #region construct anonymous fields using expression lambdas
            
            var selection = fields.GetMemberNames();

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
                x.IanaComponents,
                x.XComponents            
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase);

            #endregion

            var cclient = this.redis.As<VCALENDAR>();
            var okeys = cclient.GetAllKeys().ToArray();
            if (!okeys.NullOrEmpty()) this.redis.Watch(okeys);

            try
            {

                #region save (insert or update) relational attributes

                if (!srelation.NullOrEmpty())
                {
                    Expression<Func<VCALENDAR, object>> eventsexr = y => y.Events;
                    if (selection.Contains(eventsexr.GetMemberName()))
                    {
                        var events = source.Events;
                        this.manager.ExecTrans(transaction =>
                        {
                            if (!events.NullOrEmpty())
                            {
                                this.EventRepository.SaveAll(events);
                                var revents = keys.SelectMany(x => events.Select(y => new REL_CALENDARS_EVENTS
                                {
                                    Id = this.KeyGenerator.GetNextKey(),
                                    CalendarId = x,
                                    EventId = y.Id
                                }));

                                var orevents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => keys.Contains(x.CalendarId));
                                this.redis.SynchronizeAll<REL_CALENDARS_EVENTS>(revents, orevents, transaction);
                            }
                        });
                    }
                }

                #endregion

                #region update-only non-relational attributes

                if (!sprimitives.NullOrEmpty())
                {
                    Expression<Func<VCALENDAR, object>> prodexpr = x => x.ProdId;
                    Expression<Func<VCALENDAR, object>> versionexpr = x => x.Version;
                    Expression<Func<VCALENDAR, object>> scaleexpr = x => x.Calscale;
                    Expression<Func<VCALENDAR, object>> methodexpr = x => x.Method;

                    var entities = cclient.GetByIds(keys).ToList();
                    this.manager.ExecTrans(transaction =>
                    {
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(prodexpr.GetMemberName())) x.ProdId = source.ProdId;
                            if (selection.Contains(versionexpr.GetMemberName())) x.Version = source.Version;
                            if (selection.Contains(scaleexpr.GetMemberName())) x.Calscale = source.Calscale;
                            if (selection.Contains(methodexpr.GetMemberName())) x.Method = source.Method;
                        });
                        transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));
                    });
                }

                #endregion

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
                var ukey = UrnId.Create<VCALENDAR>(key).ToLower();
                if (this.redis.As<VCALENDAR>().ContainsKey(ukey))
                {
                    var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll();
                    var rtodos = this.redis.As<REL_CALENDARS_TODOS>().GetAll();
                    var rfreebusies = this.redis.As<REL_CALENDARS_FREEBUSIES>().GetAll();
                    var rtimezones = this.redis.As<REL_CALENDARS_TIMEZONES>().GetAll();
                    var rjournals = this.redis.As<REL_CALENDARS_JOURNALS>().GetAll();
                    var rianacs = this.redis.As<REL_CALENDARS_IANAC>().GetAll();
                    var rxcomponents = this.redis.As<REL_CALENDARS_XC>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {

                        if (!revents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>()
                            .DeleteByIds(revents.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rtodos.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>()
                            .DeleteByIds(rtodos.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rfreebusies.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>()
                            .DeleteByIds(rfreebusies.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rtimezones.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>()
                            .DeleteByIds(rtimezones.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rjournals.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>()
                            .DeleteByIds(rjournals.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rianacs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_IANAC>()
                            .DeleteByIds(rianacs.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        if (!rxcomponents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_XC>()
                            .DeleteByIds(rxcomponents.Where(x => x.CalendarId.Equals(key, StringComparison.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteById(key));
                    });

                }

            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            try
            {
                var keys = this.redis.As<VCALENDAR>().GetAllKeys();
                if (!keys.NullOrEmpty()) this.redis.Watch(keys.ToArray());

                //save events of calendar
                var events = entities.SelectMany(x => x.Events);
                this.manager.ExecTrans(transaction =>
                {
                    if (!events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(events);
                        var revents = entities.SelectMany(x => x.Events.Select(y => new REL_CALENDARS_EVENTS
                        {
                            Id = this.KeyGenerator.GetNextKey(),
                            CalendarId = x.Id,
                            EventId = y.Id
                        }));

                        var okeys = entities.Select(x => x.Id);
                        var orevents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => okeys.Contains(x.CalendarId));
                        this.redis.SynchronizeAll<REL_CALENDARS_EVENTS>(revents, orevents, transaction);
                    }

                    transaction.QueueCommand(x => x.StoreAll(this.DehydrateAll(entities)));

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
                    var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll();
                    var rtodos = this.redis.As<REL_CALENDARS_TODOS>().GetAll();
                    var rfreebusies = this.redis.As<REL_CALENDARS_FREEBUSIES>().GetAll();
                    var rtimezones = this.redis.As<REL_CALENDARS_TIMEZONES>().GetAll();
                    var rjournals = this.redis.As<REL_CALENDARS_JOURNALS>().GetAll();
                    var rianacs = this.redis.As<REL_CALENDARS_IANAC>().GetAll();
                    var rxcomponents = this.redis.As<REL_CALENDARS_XC>().GetAll();

                    this.manager.ExecTrans(transaction =>
                    {

                        if (!revents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>()
                            .DeleteByIds(revents.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!revents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>()
                            .DeleteByIds(rtodos.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!rfreebusies.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>()
                            .DeleteByIds(rfreebusies.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!rtimezones.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>()
                            .DeleteByIds(rtimezones.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!rjournals.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>()
                            .DeleteByIds(rjournals.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!rianacs.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_IANAC>()
                            .DeleteByIds(rianacs.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        if (!rxcomponents.NullOrEmpty()) transaction.QueueCommand(t => t.As<REL_CALENDARS_XC>()
                            .DeleteByIds(rxcomponents.Where(x => keys.Contains(x.CalendarId, StringComparer.OrdinalIgnoreCase))));

                        transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteByIds(keys));
                    });
                }
                else
                {
                    this.manager.ExecTrans(transaction =>
                    {
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_EVENTS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_TODOS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_FREEBUSIES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_TIMEZONES>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_JOURNALS>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_IANAC>().DeleteAll());
                        transaction.QueueCommand(t => t.As<REL_CALENDARS_XC>().DeleteAll());
                        transaction.QueueCommand(t => t.As<VCALENDAR>().DeleteAll());
                    });
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (RedisResponseException) { throw; }
            catch (RedisException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<VCALENDAR>().ContainsKey(UrnId.Create<VCALENDAR>(key).ToLower());
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var all = this.redis.As<VCALENDAR>().GetAllKeys();
            var matches = all.Intersect(keys.Select(x => UrnId.Create<VCALENDAR>(x)).ToList(), StringComparer.OrdinalIgnoreCase);
            if (matches.NullOrEmpty()) return false;
            return mode == ExpectationMode.pessimistic
                ? matches.Count() == keys.Count()
                : !matches.NullOrEmpty();
        }

        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            var dry = full;
            try
            {
                if(!dry.Events.NullOrEmpty()) dry.Events.Clear();
                if (!dry.ToDos.NullOrEmpty()) dry.ToDos.Clear();
                if (!dry.FreeBusies.NullOrEmpty()) dry.FreeBusies.Clear();
                if (!dry.Journals.NullOrEmpty()) dry.Journals.Clear();
                if (!dry.TimeZones.NullOrEmpty()) dry.TimeZones.Clear();
                if (!dry.IanaComponents.NullOrEmpty()) dry.IanaComponents.Clear();
                if (!dry.XComponents.NullOrEmpty()) dry.XComponents.Clear();
            }
            catch (ArgumentNullException) { throw; }
            return dry;
        }

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

        public IEnumerable<string> GetKeys(int? skip = null, int? take = null)
        {
            var keys = this.redis.As<VCALENDAR>().GetAllKeys();
            if (skip == null && take == null) 
               return !keys.NullOrEmpty()
                   ? keys.Select(x => UrnId.GetStringId(x))
                   : new List<string>();
            else
                return (!keys.NullOrEmpty())
                    ? keys.Select(x => UrnId.GetStringId(x)).Skip(skip.Value + 1).Take(take.Value) 
                    : new List<string>();
        }
    }
}
