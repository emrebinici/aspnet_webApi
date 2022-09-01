using System;
using System.Runtime.Caching;

namespace FlowDMApi.Core.Extentions.Cache
{
    public static class MemoryCacheManager
    {
        private static readonly ObjectCache Cache;

        static MemoryCacheManager()
        {
            Cache = MemoryCache.Default;
        }

        public static void Add<T>(string key, T source, CacheTimeEnum cacheType , int cacheTime)
        {
            switch (cacheType)
            {
                case CacheTimeEnum.Second:
                    Cache.Add(key, source, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(cacheTime) });
                    break;
                case CacheTimeEnum.Minute:
                    Cache.Add(key, source, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTime) });
                    break;
                case CacheTimeEnum.Hour:
                    Cache.Add(key, source, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
                    break;
                case CacheTimeEnum.Day:
                    Cache.Add(key, source, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddDays(cacheTime) });
                    break;
                case CacheTimeEnum.Month:
                    Cache.Add(key, source, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMonths(cacheTime) });
                    break;
            }
        
        }

        public static bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        public static T Get<T>(string key)
        {
            return (T)Cache.Get(key);
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }
    }
}
