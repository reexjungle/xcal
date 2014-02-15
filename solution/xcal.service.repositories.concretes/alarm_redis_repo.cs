using System;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.repositories.contracts;

namespace reexmonkey.xcal.service.repositories.concretes
{
    public class AudioAlarmRedisRepository: IAudioAlarmRedisRepository
    {
        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
        }

        public AUDIO_ALARM Hydrate(AUDIO_ALARM dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AUDIO_ALARM> Hydrate(IEnumerable<AUDIO_ALARM> dry)
        {
            throw new NotImplementedException();
        }

        public AUDIO_ALARM Dehydrate(AUDIO_ALARM full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AUDIO_ALARM> Dehydrate(IEnumerable<AUDIO_ALARM> full)
        {
            throw new NotImplementedException();
        }

        public AUDIO_ALARM Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AUDIO_ALARM> Find(IEnumerable<string> keys, int? page = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AUDIO_ALARM> Get(int? page = null)
        {
            throw new NotImplementedException();
        }

        public void Save(AUDIO_ALARM entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(AUDIO_ALARM entity)
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

        public void PatchAll(IEnumerable<AUDIO_ALARM> entities)
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

        public int? Pages
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class DisplayAlarmRepository: IDisplayAlarmRedisRepository
    {

        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
        }

        public DISPLAY_ALARM Hydrate(DISPLAY_ALARM dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DISPLAY_ALARM> Hydrate(IEnumerable<DISPLAY_ALARM> dry)
        {
            throw new NotImplementedException();
        }

        public DISPLAY_ALARM Dehydrate(DISPLAY_ALARM full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DISPLAY_ALARM> Dehydrate(IEnumerable<DISPLAY_ALARM> full)
        {
            throw new NotImplementedException();
        }

        public DISPLAY_ALARM Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DISPLAY_ALARM> Find(IEnumerable<string> keys, int? page = null)
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

        public void Patch(DISPLAY_ALARM entity)
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

        public void PatchAll(IEnumerable<DISPLAY_ALARM> entities)
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

        public int? Pages
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class EmailAlarmRepository: IEmailAlarmRedisRepository
    {

        public IRedisClientsManager RedisClientsManager
        {
            get { throw new NotImplementedException(); }
        }

        public EMAIL_ALARM Hydrate(EMAIL_ALARM dry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Hydrate(IEnumerable<EMAIL_ALARM> dry)
        {
            throw new NotImplementedException();
        }

        public DISPLAY_ALARM Dehydrate(EMAIL_ALARM full)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Dehydrate(IEnumerable<EMAIL_ALARM> full)
        {
            throw new NotImplementedException();
        }

        public EMAIL_ALARM Find(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMAIL_ALARM> Find(IEnumerable<string> keys, int? page = null)
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

        public void Patch(EMAIL_ALARM entity)
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

        public void PatchAll(IEnumerable<EMAIL_ALARM> entities)
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

        public int? Pages
        {
            get { throw new NotImplementedException(); }
        }
    }
}
