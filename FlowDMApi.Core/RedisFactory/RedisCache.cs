using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using StackExchange.Redis;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Core.RedisFactory
{
    public class RedisCache : IRedisCache
    {
        private readonly IDatabase _redisDb;
        private static IItemSerializer Serializer;
        //Connection bilgisi initialize anında alınıyor
        public RedisCache()
        {
            _redisDb = RedisConnectionFactory.Connection.GetDatabase();
            Serializer = new BinaryFormatterItemSerializer();
        }

        public ConnectionMultiplexer GetConnection()
        {

            return RedisConnectionFactory.Connection;
        }

        //Redis'e byte dizisi set yapan method.
        public object Add(string key, object entry, DateTime utcExpiry)
        {
            try
            {
                var existingValue = Get(key);
                if (existingValue != null)
                {
                    return existingValue;
                }

                var UniqKey = key;
                var expiration = utcExpiry - DateTime.Now;
                var entryBytes = Serializer.Serialize(entry);
                _redisDb.StringSet(UniqKey, entryBytes, expiration);
                return entry;
            }
            catch (RedisConnectionException e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
        //Redis'e byte dizisini get yapan method.
        public object Get(string key)
        {
            var valueBytes = _redisDb.StringGet(key);
            if (!valueBytes.HasValue)
            {
                return null;
            }
            var value = Serializer.Deserialize(valueBytes);
            return value;

        }

        //Redis'e json formatında set işlemi yapılan metot
        public void Set<T>(string key, T objectToCache, DateTime expireDate, When when = When.Always)
        {
            var expiration = expireDate - DateTime.Now;
            _redisDb.StringSet(key, SerializerHelper.Serialize(objectToCache), expiration, when);
        }
        public bool SetReturn(string key, string value, TimeSpan expireDate)
        {
            return _redisDb.StringSet(key, value, expireDate, When.NotExists);


        }
        //Redis te var olan key'e karşılık gelen value'yu alıp deserialize ettikten sonra return eden metot
        public T Get<T>(string key)
        {
            var redisObject = _redisDb.StringGet(key);

            return redisObject.HasValue ? SerializerHelper.Deserialize<T>(redisObject) : default;
        }
        //Redis te var olan key'e karşılık gelen value'yu alıp liste olarak deserialize ettikten sonra return eden metot
        public List<T> GetList<T>(string key)
        {
            var redisObject = _redisDb.StringGet(key);

            return redisObject.HasValue ? SerializerHelper.Deserialize<List<T>>(redisObject) : default;
        }

        //Redis te var olan key-value değerlerini silen metot
        public void Delete(string key)
        {
            _redisDb.KeyDelete(key);
        }

        public void FlushDatabase()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rediscacheName = assembly.EscapedCodeBase + "redisconnection";
            rediscacheName = rediscacheName.ToMd5();
            var con = string.Empty;
            if (!MemoryCacheManager.Contains(rediscacheName)) return;
            con = GetObjectFromCache(rediscacheName);
            if (!Regex.IsMatch(con,
                @"\s*(?<host>.*?)\s*:\s*(?<port>.*?)\s*,defaultDatabase\s*=\s*(?<database>.*?)\s*,")) return;
            var regexvalues = Regex.Match(con, @"\s*(?<host>.*?)\s*:\s*(?<port>.*?)\s*,defaultDatabase\s*=\s*(?<database>.*?)\s*,");
            var newcon = string.Format("{0}:{1},defaultDatabase ={2},abortConnect = false,allowAdmin=1", regexvalues.Groups["host"].Value, regexvalues.Groups["port"].Value, int.Parse(regexvalues.Groups["database"].Value));
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(newcon));
            lazyConnection.Value.GetServer(regexvalues.Groups["host"].Value + ":" + regexvalues.Groups["port"].Value).FlushDatabase(int.Parse(regexvalues.Groups["database"].Value));

        }

        public void DeleteFindKey(string pattern)
        {
            foreach (var ep in RedisConnectionFactory.Connection.GetEndPoints())
            {
                var server = RedisConnectionFactory.Connection.GetServer(ep);
                var keys = server.Keys(database: RedisConnectionFactory.Connection.GetDatabase().Database, pattern: pattern + "*").ToArray();
                _redisDb.KeyDeleteAsync(keys);
            }

        }

        internal static string GetObjectFromCache(string cacheItemName)
        {
            return MemoryCacheManager.Get<string>(cacheItemName);

        }
        //Gönderilen key parametresine göre redis'te bu key var mı yok mu bilgisini return eden metot
        public bool Exists(string key)
        {
            return _redisDb.KeyExists(key);
        }

        //Redis bağlantısını Dispose eden metot
        public void Dispose()
        {
            RedisConnectionFactory.Connection.Dispose();
        }
    }
}
