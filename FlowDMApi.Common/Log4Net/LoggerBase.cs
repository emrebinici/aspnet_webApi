using System;
using System.IO;
using System.Reflection;
using System.Xml;
using FlowDMApi.Core.Extentions.Cache;
using log4net;

namespace FlowDMApi.Common.Log4Net
{
    public class LoggerBase
    {
        public void Method()
        {
            var log4netConfig = GetObjectFromCache("Altiva.MobileLog4netFile");
            if (log4netConfig != null)
            {
                
            }
            else
            {
                 log4netConfig = new XmlDocument();
                log4netConfig.Load(File.OpenRead(ConnectionFilePath()));
                SetObjectFromCache("Altiva.MobileLog4netFile", log4netConfig);
            }
        
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

     
        internal static string ConnectionFilePath()
        {
            var basedir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (basedir != null)
            {
                var configFile = Path.Combine(basedir.FullName, "log4net.config");
                if (File.Exists(configFile))
                {
                    return configFile;
                }

                basedir = basedir.Parent;
            }

            throw new FileNotFoundException("Kurulum dizinlerinde 'log4net.json' dosyası bulunamadı!");
        }
        internal static XmlDocument GetObjectFromCache(string cacheItemName)
        {
            return MemoryCacheManager.Get<XmlDocument>(cacheItemName);

        }
        internal static void SetObjectFromCache(string cacheItemName, XmlDocument document)
        {
            MemoryCacheManager.Add(cacheItemName, document, CacheTimeEnum.Day, 1);

        }
    }
}
