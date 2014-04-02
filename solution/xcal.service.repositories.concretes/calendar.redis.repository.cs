using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.crosscut.essentials.contracts;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class CalendarRedisRepository : ICalendarRedisRepository
    {
        private IRedisClientsManager manager;
        private IEventRepository eventrepository;
        private int? page_size = null;
        private IKeyGenerator<string> keygen;

        private IRedisClient client = null;
        private IRedisClient redis
        {
            get
            {
                return (client != null) ? client : this.manager.GetClient();
            }
        }

        public int? PageSize
        {
            get { return this.page_size; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                this.page_size = value;
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
            this.manager = manager;
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
            try
            {
                var cclient = this.redis.As<VCALENDAR>();
                if (cclient.ContainsKey(dry.Id))
                {
                    full = cclient.GetValue(dry.Id);
                    var events = this.EventRepository.Find(dry.Id.ToSingleton());
                    if (!events.NullOrEmpty()) full.Components.AddRangeComplement(events);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }

            return full ?? dry;
        }

        public IEnumerable<VCALENDAR> Hydrate(IEnumerable<VCALENDAR> dry)
        {
            IEnumerable<VCALENDAR> full = null;
            try
            {
                var cclient = this.redis.As<VCALENDAR>();
                var keys = dry.Select(x => x.Id).Distinct().ToList();
                if (cclient.GetAllKeys().Intersect(keys).Count() == keys.Count())
                {
                    full = cclient.GetValues(keys);
                    full.Select(x =>
                    {
                        var events = this.EventRepository.Find(x.Id.ToSingleton());
                        if (!events.NullOrEmpty()) x.Components.AddRangeComplement(events);
                        return x;
                    });
                }

            }
            catch (ArgumentNullException) { throw; }
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
            VCALENDAR dry = this.redis.As<VCALENDAR>().GetValue(key);
            return (dry != null) ? this.Hydrate(dry) : dry;
        }

        public IEnumerable<VCALENDAR> Find(IEnumerable<string> keys, int? page = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            var cclient = this.redis.As<VCALENDAR>();
            var dkeys = keys.Distinct().ToList();
            if (cclient.GetAllKeys().Intersect(dkeys).Count() == dkeys.Count())
            {
                dry = (page != null) ?
                    cclient.GetValues(dkeys).Skip(page.Value).Take(page_size.Value)
                    : cclient.GetValues(dkeys);
            }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
        }

        public IEnumerable<VCALENDAR> Get(int? page = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                var cclient = this.redis.As<VCALENDAR>();
                dry = (page != null) ?
                    cclient.GetAll().Skip(page.Value).Take(page_size.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
        }

        public void Save(VCALENDAR entity)
        {
            //save calendar
            using (var transaction = this.redis.CreateTransaction())
            {
                try
                {
                    var cclient = this.redis.As<VCALENDAR>();
                    transaction.QueueCommand( x => cclient.Store(entity));

                    //save events
                    var events = entity.Components.OfType<VEVENT>();
                    if (!events.NullOrEmpty())
                    {
                        var eventids = events.Select(x => x.Id).ToArray();
                        this.EventRepository.SaveAll(events);
                        var rels = events.Select(x => new REL_CALENDARS_EVENTS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            CalendarId = entity.Id,
                            EventId = x.Id
                        });

                        var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                        var orels = rclient.GetAll().Where(x => x.CalendarId == entity.Id && !eventids.Where(y => y == x.EventId).NullOrEmpty());
                        transaction.QueueCommand(x => rclient.StoreAll(!orels.NullOrEmpty() ? rels.Except(orels) : rels));
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                } 
            }
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
                    var ids = (predicate != null) ? cclient.GetAll().Where(predicate.Compile()).Select(x => x.Id) : cclient.GetAllKeys();


                    if (!srelation.NullOrEmpty())
                    {
                        Expression<Func<VCALENDAR, object>> compexr = y => y.Components;
                        if (selection.Contains(compexr.GetMemberName()))
                        {
                            var events = source.Components.OfType<VEVENT>();
                            if (!events.NullOrEmpty())
                            {
                                var eventids = events.Select(x => x.Id).ToArray();
                                this.EventRepository.SaveAll(events);
                                
                                var rels = ids.SelectMany(x => events.Select(y => new REL_CALENDARS_EVENTS
                                {
                                    Id = KeyGenerator.GetNextKey(),
                                    CalendarId = x,
                                    EventId = y.Id
                                }));

                                var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                                var orels = rclient.GetAll()
                                    .Where(x => !ids.Where(y => y == x.CalendarId).NullOrEmpty() 
                                        && !eventids.Where(y => y == x.EventId).NullOrEmpty());

                                transaction.QueueCommand(x => rclient.StoreAll(!orels.NullOrEmpty() ? rels.Except(orels) : rels));
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

                        ids.ToList().ForEach(x =>
                        {
                            var cal = cclient.GetValue(x);
                            if (selection.Contains(versionexpr.GetMemberName())) cal.Version = source.Version;
                            if (selection.Contains(scaleexpr.GetMemberName())) cal.Calscale = source.Calscale;
                            if (selection.Contains(methodexpr.GetMemberName())) cal.Method = source.Method;
                            transaction.QueueCommand(trans => cclient.Store(cal));
                        });
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
                    cclient.StoreAll(entities);

                    //save events
                    var events = entities.SelectMany(x => x.Components.OfType<VEVENT>());
                    if (!events.NullOrEmpty()) this.EventRepository.SaveAll(events);

                    entities.ToList().ForEach(x =>
                    {
                        var ids = x.Components.OfType<VEVENT>().Select(y => y.Id).ToArray();

                        var rels = events.Select(y => new REL_CALENDARS_EVENTS
                        {
                            Id = KeyGenerator.GetNextKey(),
                            CalendarId = x.Id,
                            EventId = y.Id
                        });

                        var rclient = this.redis.As<REL_CALENDARS_EVENTS>();
                        var orels = rclient.GetAll().Where(y => y.CalendarId == x.Id && !ids.Where(z => z == y.EventId).NullOrEmpty());
                        rclient.StoreAll(!orels.NullOrEmpty() ? rels.Except(orels) : rels);

                    });

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
            if (keys != null) cclient.DeleteAll();
            else
            {
                keys.ToList().ForEach(x =>
                {
                    if (cclient.ContainsKey(x))
                    {
                        cclient.DeleteRelatedEntities<REL_CALENDARS_EVENTS>(x);
                        cclient.DeleteById(x);
                    }
                });
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
    }
}
