using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;
using reexmonkey.infrastructure.operations.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class CalendarRedisRepository : ICalendarRedisRepository
    {
        private IRedisClientsManager manager;
        private IEventRepository eventrepository;
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

        public CalendarRedisRepository() { }

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
            VCALENDAR full = null;
            full = this.redis.As<VCALENDAR>().GetById(dry.Id);
            if (full != null)
            {
                var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => x.CalendarId == full.Id);
                if (!revents.NullOrEmpty())
                {
                    var events = this.EventRepository.Find(revents.Select(x => x.EventId).ToList());
                    full.Components.AddRangeComplement(this.EventRepository.Hydrate(events));
                }
            }
            return full ?? dry;
        }

        public IEnumerable<VCALENDAR> Hydrate(IEnumerable<VCALENDAR> dry)
        {
            IEnumerable<VCALENDAR> full = null;
            var cclient = this.redis.As<VCALENDAR>();
            var keys = dry.Select(x => x.Id).Distinct().ToList();
            
            full = cclient.GetByIds(keys);
            if (!full.NullOrEmpty())
            {
                var revents = this.redis.As<REL_CALENDARS_EVENTS>().GetAll().Where(x => keys.Contains(x.CalendarId));
                if(!revents.NullOrEmpty())
                {
                   var events = this.EventRepository.Hydrate(this.EventRepository.Find(revents.Select(x => x.EventId))).ToList();
                   full.Select(x =>
                   {
                       var xevents = from y in events
                                     join r in revents on y.Id equals r.EventId
                                     join c in full on r.CalendarId equals c.Id
                                     where c.Id == x.Id
                                     select y;
                       if (!xevents.NullOrEmpty()) x.Components.AddRangeComplement(xevents);
                       return x;
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
            return this.redis.As<VCALENDAR>().GetValue(key);
        }

        public IEnumerable<VCALENDAR> Find(IEnumerable<string> keys, int? skip = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            var cclient = this.redis.As<VCALENDAR>();
            var dkeys = keys.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (cclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (skip != null) ?
                    cclient.GetValues(dkeys).Skip(skip.Value).Take(take.Value)
                    : cclient.GetValues(dkeys);
            }
            return dry;
        }

        public IEnumerable<VCALENDAR> Get(int? skip = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                var cclient = this.redis.As<VCALENDAR>();
                dry = (skip != null) ?
                    cclient.GetAll().Skip(skip.Value).Take(take.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return dry;
        }

        public void Save(VCALENDAR entity)
        {
            this.manager.ExecTrans(transaction =>
            {
                try
                {
                    var keys = this.redis.As<VCALENDAR>().GetAllKeys().ToArray();
                    if (!keys.NullOrEmpty()) this.redis.Watch(keys);
                    var events = entity.Components.OfType<VEVENT>();
                    if (!events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(events);
                        var revents = events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            CalendarId = entity.Id,
                            EventId = x.Id
                        });

                        var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                        var orevents = rclient.GetAll().Where(x => x.CalendarId == entity.Id);
                        transaction.QueueCommand(x => x.StoreAll(!orevents.NullOrEmpty()
                            ? revents.Except(orevents)
                            : revents));
                    }

                    transaction.QueueCommand(x => x.Store(this.Dehydrate(entity)));

                }
                catch (RedisResponseException) { throw; }
                catch (RedisException) { throw; }
                catch (InvalidOperationException) {    throw; }
            });
        }

        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, Expression<Func<VCALENDAR, bool>> predicate = null)
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
                x.Components
            };

            //4. Get list of selected relationals
            var srelation = relations.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            //5. Get list of selected primitives
            var sprimitives = primitives.GetMemberNames().Intersect(selection, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase);

            #endregion

            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    #region save (insert or update) relational attributes

                    var cclient = this.redis.As<VCALENDAR>();
                    var keys = (predicate != null) 
                        ? cclient.GetAll().Where(predicate.Compile()).Select(x => x.Id).ToList() 
                        : cclient.GetAllKeys();


                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<VCALENDAR, object>> compexr = y => y.Components;
                        if (selection.Contains(compexr.GetMemberName()))
                        {
                            var events = source.Components.OfType<VEVENT>();
                            if (!events.NullOrEmpty())
                            {
                                var eventkeys = events.Select(x => x.Id).ToArray();
                                this.EventRepository.SaveAll(events);
                                
                                var revents = keys.SelectMany(x => events.Select(y => new REL_CALENDARS_EVENTS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    CalendarId = x,
                                    EventId = y.Id
                                }));

                                var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                                var orevents = rclient.GetAll().Where(x => keys.Contains(x.CalendarId));

                                transaction.QueueCommand(x => 
                                    x.StoreAll(!orevents.NullOrEmpty() 
                                    ? revents.Except(orevents) 
                                    : revents));
                            }
                        }
                    }

                    #endregion

                    #region update-only non-relational attributes

                    if (!sprimitives.NullOrEmpty())
                    {
                        Expression<Func<VCALENDAR, object>> versionexpr = x => x.Version;
                        Expression<Func<VCALENDAR, object>> scaleexpr = x => x.Calscale;
                        Expression<Func<VCALENDAR, object>> methodexpr = x => x.Method;

                        var entities = cclient.GetValues(keys);
                        entities.ForEach(x =>
                        {
                            if (selection.Contains(versionexpr.GetMemberName())) x.Version = source.Version;
                            if (selection.Contains(scaleexpr.GetMemberName())) x.Calscale = source.Calscale;
                            if (selection.Contains(methodexpr.GetMemberName())) x.Method = source.Method;
                        });

                        transaction.QueueCommand(x => x.StoreAll(entities));
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
            var cclient = this.redis.As<VCALENDAR>();
            if (cclient.ContainsKey(key))
            {
                cclient.DeleteRelatedEntities<REL_CALENDARS_EVENTS>(key);
                cclient.DeleteById(key);
            }
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    //save calendar
                    var cclient = this.redis.As<VCALENDAR>();
                    var keys = entities.Select(c => c.Id);
                    transaction.QueueCommand(x => x.StoreAll(entities));

                    //save events
                    var events = entities.SelectMany(x => x.Components.OfType<VEVENT>());
                    if (!events.NullOrEmpty())
                    {
                        this.EventRepository.SaveAll(events);
                        var revents = entities.Where(x => !x.Components.OfType<VEVENT>().NullOrEmpty())
                            .SelectMany(c => c.Components.OfType<VEVENT>()
                                .Select(x => new REL_CALENDARS_EVENTS 
                                { 
                                    Id = this.KeyGenerator.GetNextKey(), 
                                    CalendarId = c.Id, 
                                    EventId = x.Id 
                                }));

                        var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                        var orevents  = rclient.GetAll().Where(x => keys.Contains(x.CalendarId));
                        transaction.QueueCommand(x => x.StoreAll(!orevents.NullOrEmpty()? revents.Except(orevents): revents));
                    }
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
            var cclient = this.redis.As<VCALENDAR>();
            Action<IRedisTypedClient<VCALENDAR>, string> cascade_on_delete = (e, k) =>
            {
                if (e.ContainsKey(k))
                {
                    cclient.DeleteRelatedEntities<REL_CALENDARS_EVENTS>(k);
                }
            };

            if (keys == null)
            {
                cclient.GetAllKeys().ForEach(x => cascade_on_delete(cclient, x));
                cclient.DeleteAll();
            }
            else
            {
                var dkeys = keys.Distinct().ToList();
                dkeys.ForEach(x => cascade_on_delete(cclient, x));
                cclient.DeleteByIds(dkeys);
            }
        }

        public bool ContainsKey(string key)
        {
            return this.redis.As<VCALENDAR>().ContainsKey(key);
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            var matches = this.redis.As<VCALENDAR>().GetAllKeys().Intersect(keys);
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
                if(!dry.Components.NullOrEmpty()) dry.Components.Clear();
            }
            catch (ArgumentNullException) { throw; }
            return dry;
        }

        public IEnumerable<VCALENDAR> Dehydrate(IEnumerable<VCALENDAR> full)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                dry = full.Select(x => 
                {
                    if (!x.Components.NullOrEmpty()) x.Components.Clear();
                    return x;
                });
            }
            catch (ArgumentNullException) { throw; }
            return dry ?? full;
        }


        public IEnumerable<string> GetKeys(int? skip = null)
        {
            if (skip == null) return this.redis.As<VCALENDAR>().GetAllKeys();
            else
            {
                var keys = this.redis.As<VCALENDAR>().GetAllKeys();
                return (!keys.NullOrEmpty()) ? keys.Skip(skip.Value).Take(take.Value) : keys;
            }
        }
    }
}
