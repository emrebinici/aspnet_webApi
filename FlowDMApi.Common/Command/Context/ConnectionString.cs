using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using FlowDMApi.Common.Helper;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Common.Command.Context
{
    public class ConnectionString
    {
        public static string GetSqlServer()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var rediscacheName = (assembly.Location + "sqlserverconfiguration").ToMd5();
                if (MemoryCacheManager.Contains(rediscacheName)) return MemoryCacheManager.Get<string>(rediscacheName);
                Item item;
                var stringpath = ConnectionFilePath();
                using (var r = new StreamReader(stringpath))
                {
                    var json = r.ReadToEnd();
                    item = JsonConvert.DeserializeObject<Item>(json);
                }
                var connectionString =  item.ConnectionString;
                if (connectionString != null)
                {
                        var constring = connectionString;
                        MemoryCacheManager.Add(rediscacheName, constring, CacheTimeEnum.Day, 10); // 10 Gün Configuration Dosyasını Cache de tut.
                        return constring;
                        
                }
                throw new Exception("ConnectionString Bulunamadı!");
            }
            catch (Exception e)
            {
                throw new Exception("Veritabanına bağlanamadı.");
            }
        }
        internal static string ConnectionFilePath()
        {
            var basedir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            // Üst dizinlere çıkarak config dosyası ara
            while (basedir != null)
            {
                string configFile = Path.Combine(basedir.FullName, "Connection.json");
                if (File.Exists(configFile))
                {
                    return configFile;
                }

                basedir = basedir.Parent;
            }

            throw new FileNotFoundException("Kurulum dizinlerinde 'Connection.json' dosyası bulunamadı!");
        }
        internal static void SetObjectFromCache(string cacheItemName, string constring)
        {
            MemoryCacheManager.Add(cacheItemName, constring, CacheTimeEnum.Day, 1);

        }
    }

    public class ConModel
    {
        public string server { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string database { get; set; }
        public uint port { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
    public class Item
    {
        public string ConnectionString { get; set; }
        public int ConnectionTimeOut { get; set; }
    }
}

