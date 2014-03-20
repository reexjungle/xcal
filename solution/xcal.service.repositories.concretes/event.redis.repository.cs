using System;
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
    public class EventRedisRepository: IEventRedisRepository
    {
        private IRedisTypedClient<VEVENT> client = null;
        private IRedisClientsManager manager = null;
        private int? pages= null;

        private IRedisTypedClient<VEVENT> redis
        {
            get { return client ?? manager.GetClient().As<VEVENT>(); }
        }

        public IRedisClientsManager RedisClientsManager
        {
            get { return this.manager; }
            set
            {
                if (value == null) throw new ArgumentNullException("RedisClientsManager");
                this.manager = value;
            }
        }

        public int? Pages
        {
            get { return this.pages; }
            set { this.pages = value;}
        }

        public IAudioAlarmRepository AudioAlarmRepository
        {
            get { throw new NotImplementedException();}
            set { throw new NotImplementedException();}
        }

        public IDisplayAlarmRepository DisplayAlarmRepository
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEmailAlarmRepository EmailAlarmRepository
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public EventRedisRepository()
        {

        }

        public EventRedisRepository(IRedisClientsManager manager, int? pages = null)
        {
            if (manager == null) throw new ArgumentNullException("Null Redis Client Manager");
            this.manager = manager;
            if (pages == null) throw new ArgumentNullException("Null pages");
            this.pages = pages;
            this.client = manager.GetClient().As<VEVENT>();
            if(this.client == null) throw new ArgumentNullException("Null Redis client");
        }


        public VEVENT Hydrate(VEVENT dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
        {
            throw new NotImplementedException();
        }

        public VEVENT Dehydrate(VEVENT full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Dehydrate(IEnumerable<VEVENT> full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(VEVENT entity)
        {
            throw new NotImplementedException();
        }

        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<VEVENT> entities)
        {
            throw new NotImplementedException();
        }


        public void EraseAll(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public void EraseAll()
        {
            throw new NotImplementedException();
        }


        public VEVENT Find(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> KeyGenerator { get; set; }

        public void Patch(VEVENT entity, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
        {
            throw new NotImplementedException();
        }


        public bool Has(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public bool Has(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }
    }
}
