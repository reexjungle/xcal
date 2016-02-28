using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.repositories.contracts;
using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace reexjungle.xcal.service.repositories.concretes.dapper
{
    public class CalendarDapperRepository : ICalendarRepository, IOrmRepository, IDisposable
    {
        private readonly IDbConnectionFactory factory;
        private IDbConnection dbconnection;
        private readonly IKeyGenerator<Guid> keygenerator;
        private readonly IEventRepository eventrepository;

        private IDbConnection db => dbconnection ?? (dbconnection = factory.OpenDbConnection());

        public IDbConnectionFactory DbConnectionFactory
        {
            get { return factory; }
        }

        public CalendarDapperRepository(IDbConnectionFactory factory, IKeyGenerator<Guid> keygenerator, IEventRepository eventrepository)
        {
            dbconnection.ThrowIfNull("factory");
            keygenerator.ThrowIfNull("keygenerator");
            eventrepository.ThrowIfNull("eventrepository");

            this.factory = factory;
            this.keygenerator = keygenerator;
            this.eventrepository = eventrepository;
        }

        public VCALENDAR Find(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> FindAll(IEnumerable<Guid> keys, int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> Get(int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKeys(IEnumerable<Guid> keys, ExpectationMode mode = ExpectationMode.Optimistic)
        {
            throw new NotImplementedException();
        }

        public void Save(VCALENDAR entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(VCALENDAR source, IEnumerable<string> fields, IEnumerable<Guid> keys = null)
        {
            throw new NotImplementedException();
        }

        public void Erase(Guid key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<VCALENDAR> entities)
        {
            throw new NotImplementedException();
        }

        public void EraseAll(IEnumerable<Guid> keys = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetKeys(int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Hydrate(VCALENDAR dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> HydrateAll(IEnumerable<VCALENDAR> dry)
        {
            throw new NotImplementedException();
        }

        public VCALENDAR Dehydrate(VCALENDAR full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VCALENDAR> DehydrateAll(IEnumerable<VCALENDAR> full)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}