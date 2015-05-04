using reexjungle.xmisc.foundation.concretes;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reexjungle.technical.data.concretes.extensions.redis
{
    public static class RedisExtensions
    {
        public static string CreateUrn<T>(this string key)
        {
            return string.Format("urn:{0}:{1}", typeof(T).Name.ToLowerInvariant(), key);
        }

        public static void MergeAll<T, Tkey>(this IRedisClient redis, IEnumerable<T> entities, IEnumerable<T> oentities, IRedisTransaction transaction)
            where Tkey : IEquatable<Tkey>, IComparable<Tkey>
            where T : class, IContainsKey<Tkey>, new()
        {
            if (!oentities.NullOrEmpty())
            {
                var incoming = entities.Except(oentities).ToArray();
                if (!incoming.NullOrEmpty()) transaction.QueueCommand(x => x.StoreAll(incoming));
                var outgoing = oentities.Except(entities).ToArray();
                if (!outgoing.NullOrEmpty())
                    transaction.QueueCommand(x => x.As<T>().DeleteByIds(outgoing.Select(y => y.Id).ToArray()));
            }
            else transaction.QueueCommand(x => x.StoreAll(entities));
        }

        public static void MergeAll<T>(this IRedisClient redis, IEnumerable<T> entities, IEnumerable<T> oentities, IRedisTransaction transaction)
    where T : class, IContainsKey<string>, new()
        {
            redis.MergeAll<T, string>(entities, oentities, transaction);
        }

        public static void RemoveAll<T, Tkey>(this IRedisClient redis, IEnumerable<T> oentities, IRedisTransaction transaction)
            where Tkey : IEquatable<Tkey>, IComparable<Tkey>
            where T : class, IContainsKey<Tkey>, new()
        {
            if (!oentities.NullOrEmpty()) transaction.QueueCommand(x => x.As<T>().DeleteByIds(oentities.Select(y => y.Id).ToArray()));
        }

        public static void RemoveAll<T>(this IRedisClient redis, IEnumerable<T> oentities, IRedisTransaction transaction)
where T : class, IContainsKey<string>, new()
        {
            redis.RemoveAll<T, string>(oentities, transaction);
        }
    }
}