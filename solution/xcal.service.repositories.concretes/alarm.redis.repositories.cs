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
    public class AudioAlarmRedisRepository: IAudioAlarmRedisRepository
    {

        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<AUDIO_ALARM> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(AUDIO_ALARM entity)
        {
            throw new NotImplementedException();
        }

        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<AUDIO_ALARM> entities)
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

        public int? PageSize
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public AUDIO_ALARM Find(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AUDIO_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> KeyGenerator { get; set; }

        public void Patch(AUDIO_ALARM entity, Expression<Func<AUDIO_ALARM, object>> fields, Expression<Func<AUDIO_ALARM, bool>> where = null)
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


        public bool Contains(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }
    }

    public class DisplayAlarmRedisRepository: IDisplayAlarmRedisRepository
    {
        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DISPLAY_ALARM Find(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DISPLAY_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<DISPLAY_ALARM> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(DISPLAY_ALARM entity)
        {
            throw new NotImplementedException();
        }


        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<DISPLAY_ALARM> entities)
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

        public int? PageSize
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        public IKeyGenerator<string> KeyGenerator { get; set; }

        public void Patch(DISPLAY_ALARM entity, Expression<Func<DISPLAY_ALARM, object>> fields, Expression<Func<DISPLAY_ALARM, bool>> where = null)
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


        public bool Contains(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }
    }

    public class EmailAlarmRedisRepository: IEmailAlarmRedisRepository
    {

        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }

        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Hydrate(IEnumerable<EMAIL_ALARM> dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(EMAIL_ALARM entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(EMAIL_ALARM patch, Expression<Func<EMAIL_ALARM, object>> fields, Expression<Func<EMAIL_ALARM, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public void Erase(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(IEnumerable<EMAIL_ALARM> entities)
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

        public int? PageSize
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public EMAIL_ALARM Find(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Find(IEnumerable<string> fkeys, IEnumerable<string> pkeys = null, int? page = null)
        {
            throw new NotImplementedException();
        }

        public IKeyGenerator<string> KeyGenerator { get; set; }


        public bool Has(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public bool Has(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }


        public bool Contains(string fkey, string pkey)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IEnumerable<string> fkeys, IEnumerable<string> pkeys, ExpectationMode mode = ExpectationMode.optimistic)
        {
            throw new NotImplementedException();
        }
    }
}
