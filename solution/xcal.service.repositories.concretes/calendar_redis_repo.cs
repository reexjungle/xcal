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
    public class CalendarRedisRepository : ICalendarRedisRepository
    {
        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
        }

        public VEVENT Hydrate(VEVENT dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VEVENT> Hydrate(IEnumerable<VEVENT> dry)
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

        public IEnumerable<VEVENT> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> IdProvider
        {
            get { throw new NotImplementedException(); }
        }

        public void Save(VEVENT entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(VEVENT source, Expression<Func<VEVENT, object>> fields, Expression<Func<VEVENT, bool>> where = null)
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

        public void EraseAll(IEnumerable<string> keys = null)
        {
            throw new NotImplementedException();
        }

        public int? Pages
        {
            get { throw new NotImplementedException(); }
        }
    }
}
