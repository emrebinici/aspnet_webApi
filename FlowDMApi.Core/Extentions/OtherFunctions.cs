using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Core.Extentions
{
    public static class OtherFunctions
    {
        public static string ToMd5(this string item)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] btr = Encoding.UTF8.GetBytes(item);
            btr = md5.ComputeHash(btr);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in btr)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }
        public static string getTableName<T>(T obj)
        {
            // obj = Activator.CreateInstance<T>();
            return obj.GetType().Name;
        }

        private static bool IsNumber(this object e)
        {
            switch (Type.GetTypeCode(e.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static string GetModelStringBuilderMonitor(DynamicParameters item) //where T : class
        {
            StringBuilder result = new StringBuilder();
            if (item != null)
            {
                result.Append("<ol style='border:solid 1px #ccc; list-style-type:none;margin-left:5px;padding-left:10 !important;  border-radius: 5px;' >");

                //switch (item)
                //{

                Parallel.ForEach(item.ParameterNames,
                    (pi) =>
                    {
                        var val = item.Get<object>(pi);
                        if (val is IEnumerable<object> enumerable)
                        {
                            bool isNumber = false;
                            foreach (var e in enumerable)
                            {
                                isNumber = e.IsNumber();
                                break;
                            }

                            var ayrac = isNumber ? "" : "'";
                            string tmp = "";
                            foreach (var o in enumerable)
                            {
                                if (o != null)
                                {
                                    tmp += ayrac + o.ToString() + ayrac + ",";
                                }
                                else
                                {
                                    tmp += "NULL" + ",";
                                }
                            }
                            tmp = tmp.TrimEnd(',');
                            result.Append("<li style='padding-left:10 !important;'>@<strong> " + pi + "</strong> = ARRAY[" + tmp + "] </li>");
                        }
                        else if (val is Array array)
                        {
                            bool isNumber = false;
                            foreach (var e in array)
                            {
                                isNumber = e.IsNumber();
                                break;
                            }
                            var ayrac = isNumber ? "" : "'";
                            string tmp = "";
                            foreach (var o in array)
                            {
                                if (o != null)
                                {
                                    tmp += ayrac + o.ToString() + ayrac + ",";
                                }
                                else
                                {
                                    tmp += "NULL" + ",";
                                }
                            }
                            tmp = tmp.TrimEnd(',');
                            result.Append("<li style='padding-left:10 !important;'>@<strong> " + pi + "</strong> = ARRAY[" + tmp + "] </li>");
                        }
                        else
                        {
                            result.Append("<li style='padding-left:10 !important;'>@<strong> " + pi + "</strong> = " + (val ?? "NULL") + " </li>");
                        }
                    });
                //    break;
                //default:
                //    {
                //        try
                //        {
                //            var type = item.GetType();
                //            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                //            Parallel.ForEach(properties, (p, state) =>
                //            {
                //                try
                //                {
                //                    var selfValue = type.GetProperty(p.Name)?.GetValue(item, null);
                //                    result.Append(selfValue != null ? ("<li style='padding-left:10 !important;'>@<strong>" + p.Name + "</strong> = " + selfValue + "</li>") : ("<li>@<strong>" + p.Name + "</strong> =  </li>"));
                //                }
                //                catch
                //                {
                //                    result.Append("<li style='padding-left:10 !important;'><strong> Toplu İşlemlerde Parametre Görüntülenemez. </strong></li>");
                //                    state.Break();
                //                }

                //            });


                //        }
                //        catch
                //        {
                //            result.Append("<li style='padding-left:10 !important;'> Toplu İşlemlerde Parametre Görüntülenemez. </li>");
                //        }

                //        break;
                //    }
                //}
                result.Append(" </ol>");

            }
            else
            {
                // result.Append("<li style='padding-left:10 !important;'> Parametre Yok </li>");
            }
            return result.ToString();
        }


        public static bool checkForSQLInjection(string userInput)
        {
            bool isSQLInjection = false;
            string[] sqlCheckList = { "--",
                                       ";--",
                                       ";",
                                       "/*",
                                       "*/",
                                        "@@",
                                        "@",
                                        "char",
                                       "nchar",
                                       "varchar",
                                       "nvarchar",
                                       "alter",
                                       "begin",
                                       "cast",
                                       "create",
                                       "cursor",
                                       "declare",
                                       "delete",
                                       "drop",
                                       "end",
                                       "exec",
                                       "execute",
                                       "fetch",
                                       "insert",
                                       "kill",
                                       "select",
                                       "sys",
                                        "sysobjects",
                                        "syscolumns",
                                       "table",
                                       "update"
                                       };

            var CheckString = userInput.Replace("'", "''");
            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                { isSQLInjection = true; }
            }

            return isSQLInjection;
        }

        #region Cache
        public static T GetObjectFromCache<T>(string cacheItemName)
        {
            return MemoryCacheManager.Get<T>(cacheItemName);

        }
        public static void SetObjectFromCache(string cacheItemName, object constring)
        {
            MemoryCacheManager.Add(cacheItemName, constring, CacheTimeEnum.Day, 1);

        }


        #endregion

    }
}

