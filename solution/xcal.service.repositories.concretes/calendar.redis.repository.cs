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

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            VCALENDAR full = null;
            try
            {
                var cclient = this.redis.GetTypedClient<VCALENDAR>();
                if(cclient.ContainsKey(dry.Id))
                {
                    full = cclient.GetValue(dry.Id);
                    var events = this.EventRepository.Find(dry.Id.ToSingleton());
                    if (!events.NullOrEmpty()) full.Components.AddRangeComplement(events);
                }
            }
            catch (ArgumentNullException)  { throw; }
            catch (Exception)  { throw; }

            return full??dry;
        }

        public IEnumerable<VCALENDAR> Hydrate(IEnumerable<VCALENDAR> dry)
        {
            IEnumerable<VCALENDAR> full = null;
            try
            {
                var cclient = this.redis.GetTypedClient<VCALENDAR>();
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
            catch (Exception) { throw; }
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
            VCALENDAR dry = null;
            try
            {
                dry = this.redis.GetTypedClient<VCALENDAR>().GetValue(key);
            }
            catch (Exception) {  throw; }
            return (dry != null)? this.Hydrate(dry) : dry;
        }

        public IEnumerable<VCALENDAR> Find(IEnumerable<string> keys, int? page = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                var cclient = this.redis.GetTypedClient<VCALENDAR>();
                var dkeys = keys.Distinct().ToList();
                if (cclient.GetAllKeys().Intersect(keys).Count() == keys.Count())
                {
                    dry = (page != null)? 
                        cclient.GetValues(dkeys).Skip(page.Value).Take(page_size.Value)
                        : cclient.GetValues(dkeys);
                }
               
            }
            catch (Exception) { throw; }
            return (!dry.NullOrEmpty() ) ? this.Hydrate(dry) : dry;
        }

        public IEnumerable<VCALENDAR> Get(int? page = null)
        {
            IEnumerable<VCALENDAR> dry = null;
            try
            {
                var cclient = this.redis.GetTypedClient<VCALENDAR>();
                dry = (page != null) ?
                    cclient.GetAll().Skip(page.Value).Take(page_size.Value)
                    : cclient.GetAll();
            }
            catch (Exception) { throw; }
            return (!dry.NullOrEmpty()) ? this.Hydrate(dry) : dry;
        }

        public void Save(VCALENDAR entity)
        {
            try
            {
                //save events
                var events = entity.Components.OfType<VEVENT>();
                if (!events.NullOrEmpty())
                {
                    this.EventRepository.SaveAll(events);
                    var rels = events.Select(x => new REL_CALENDARS_EVENTS 
                    { 
                        Id = KeyGenerator.GetNextKey(), 
                        CalendarId = entity.Id, 
                        EventId = x.Id 
                    });

                    var rclient = this.client.GetTypedClient<REL_CALENDARS_EVENTS>();
                    //var orels = rclient.get

                    rclient.StoreAll(rels);
                    
                }

                //save calendar
                //this.cclient.Store(entity);
                //this.cclient.Save();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Patch(VCALENDAR source, Expression<Func<VCALENDAR, object>> fields, Expression<Func<VCALENDAR, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            try
            {
                //save events

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            throw new NotImplementedException();
        }

        public bool Has(string key)
        {
            throw new NotImplementedException();
        }

        public bool Has(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }
    }
}
