using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using reexmonkey.technical.data.contracts;
using reexmonkey.technical.data.concretes.extensions.ormlite;
using reexmonkey.crosscut.operations.concretes;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.io.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class CalendarOrmLiteRepository: ICalendarOrmLiteRepository
    {
        public IDbConnectionFactory DbConnectionFactory
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEventRepository EventRepository
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

        public IKeyGenerator<string> KeyGenerator { get; set; }

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

        public int? Take { get; set; }


        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKeys(IEnumerable<string> keys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<VCALENDAR> Dehydrate(IEnumerable<VCALENDAR> full)
        {
            throw new NotImplementedException();
        }


        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            throw new NotImplementedException();
        }
    }
}
