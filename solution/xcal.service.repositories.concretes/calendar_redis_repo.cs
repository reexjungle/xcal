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
        public IEventRepository EventRepository { get; set; }


        public IRedisClientsManager RedisClientsManager
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> Hydrate(IEnumerable<VCALENDAR> dry)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> Find(IEnumerable<string> keys, int? page = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> KeyGenerator
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Save(VCALENDAR entity)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void EraseAll(IEnumerable<string> keys = null)
        {
            throw new NotImplementedException();
        }

        public int? Pages
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
