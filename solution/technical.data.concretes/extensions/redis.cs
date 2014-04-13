using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.technical.data.concretes.extensions.redis
{
    public static class RedisExtensions
    {
        public static string CreateUrn<T>(this string key)
        {
            return string.Format("urn:{0}:{1}", typeof(T).Name.ToLowerInvariant(), key);
        }
    }
}
