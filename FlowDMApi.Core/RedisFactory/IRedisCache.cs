using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace FlowDMApi.Core.RedisFactory
{
    public interface IRedisCache : IDisposable
    {
        T Get<T>(string key);
        List<T> GetList<T>(string key);
        object Add(string key, object entry, DateTime utcExpiry);
        object Get(string key);
        void Set<T>(string key, T obj, DateTime expireDate, When when = When.Always);
        bool SetReturn(string key, string value, TimeSpan expireDate);
        void Delete(string key);
        void FlushDatabase();
        void DeleteFindKey(string pattern);
        bool Exists(string key);
        ConnectionMultiplexer GetConnection();
    }
}
