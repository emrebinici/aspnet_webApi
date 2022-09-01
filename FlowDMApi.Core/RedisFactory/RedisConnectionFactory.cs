using System;
using System.IO;
using System.Reflection;
using StackExchange.Redis;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Core.RedisFactory
{
    internal class RedisConJson
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string DefaultDatabase { get; set; }
        public string ConnectionName { get; set; }
    }
    public static class RedisConnectionFactory
    {
        static RedisConnectionFactory()
        {
            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConString()));
        }
        private static string ConString()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rediscacheName = assembly.EscapedCodeBase + "redisconnection";
            rediscacheName = rediscacheName.ToMd5();
            var con = string.Empty;
            if (MemoryCacheManager.Contains(rediscacheName))
            {
                con = GetObjectFromCache(rediscacheName);
                return con;
            }
            else
            {
                RedisConJson item = null;
                var stringpath = ConnectionFilePath();
                using (StreamReader r = new StreamReader(stringpath))
                {
                    string json = r.ReadToEnd();
                    item = SerializerHelper.Deserialize<RedisConJson>(json);
                }

                con = string.Format("{0}:{1},defaultDatabase ={2},abortConnect = false,allowAdmin:true", item.Server, item.Port, item.DefaultDatabase);
                SetObjectFromCache(rediscacheName, con);
                return con;
            }
        }
        internal static string ConnectionFilePath()
        {
            var basedir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (basedir != null)
            {
                string configFile = Path.Combine(basedir.FullName, "RedisConnection.json");
                if (File.Exists(configFile))
                {
                    return configFile;
                }

                basedir = basedir.Parent;
            }

            throw new FileNotFoundException("Kurulum dizinlerinde 'RedisConnection.json' dosyası bulunamadı!");
        }
        internal static string GetObjectFromCache(string cacheItemName)
        {
            return MemoryCacheManager.Get<string>(cacheItemName);

        }
        internal static void SetObjectFromCache(string cacheItemName, string constring)
        {
            MemoryCacheManager.Add(cacheItemName, constring, CacheTimeEnum.Day, 1);

        }

        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

        private static readonly object Lock = new object();
        public static ConnectionMultiplexer Connection => LazyConnection.Value;
        public static void DisposeConnection()
        {
            if (LazyConnection.Value.IsConnected)
                LazyConnection.Value.Dispose();
        }
    }
}
