using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlowDMApi.Common.Command.Paged;
using FlowDMApi.Core.Extentions;
using FlowDMApi.Core.Extentions.Cache;
using FlowDMApi.Core.RedisFactory;
using FlowDMApi.Core.SqlMapper;
using Dapper;
using Newtonsoft.Json;

namespace FlowDMApi.Common.Command.Context
{
    public class Commander : ICommander
    {
        private readonly IRedisCache redisCache = new RedisCache();
        public Commander()
        {
            SqlMapper.AddTypeHandler(typeof(string[]), new StringArrayTypeHandler());
            SqlMapper.AddTypeHandler(typeof(Dictionary<string, string>), new JsonObjectTypeHandler());
            SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeTypeHandler());
            SqlMapper.AddTypeHandler(typeof(TimeSpan), new TimeSpanToTicksHandler());
            SqlMapper.AddTypeHandler(typeof(Enum), new StringEnumTypeHandler());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        public QueryReturnModel<T> Query<T>(string query, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text, CacheTimeEnum? cacheTimeType = null, int expireTime = 1, bool dynamicRedisCache = false)
        {
            if (cacheTimeType == null)
            {
                var result = new QueryReturnModel<T>()
                {
                    Items = QueryRun<T>(string.Format(query), param, transaction, commandTimeout, commandType, null, 0),
                    QueryModelCacheName = null
                };
                return result;
            }
            var uniqkey = new StringBuilder();
            uniqkey.Append(query);
            if (param != null)
            {
                var paramList = param.ParameterNames.Select(x => new { ParameterName = x, ParameterValue = param.Get<object>(x) }).ToList();
                uniqkey.Append(SerializerHelper.Serialize(paramList));
            }
            var redisKey = uniqkey.ToString().ToMd5();
            var rediscacheKey = "QueryCache" + GenerateDynamicCacheName(dynamicRedisCache, query).Replace("||", "|") + ":" + redisKey;
            if (!redisCache.Exists(rediscacheKey))
            {
                var cacheList = transaction.Connection
                    .Query<T>(string.Format(query), param, transaction, false, commandTimeout)?.ToList();


                if (cacheList == null || cacheList.Count < 1)
                    return new QueryReturnModel<T>()
                    {
                        Items = cacheList,
                        QueryModelCacheName = rediscacheKey
                    };
                var jsonserializedat = JsonConvert.SerializeObject(cacheList);

                switch (cacheTimeType)
                {
                    case CacheTimeEnum.Year:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddYears(expireTime));
                        break;
                    case CacheTimeEnum.Month:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddMonths(expireTime));
                        break;
                    case CacheTimeEnum.Day:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddDays(expireTime));
                        break;
                    case CacheTimeEnum.Hour:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddHours(expireTime));
                        break;
                    case CacheTimeEnum.Minute:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddMinutes(expireTime));
                        break;
                    case CacheTimeEnum.Second:
                        redisCache.Add(rediscacheKey, jsonserializedat, DateTime.Now.AddSeconds(expireTime));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(cacheTimeType), cacheTimeType, null);
                }

                return new QueryReturnModel<T>()
                {
                    Items = cacheList,
                    QueryModelCacheName = rediscacheKey
                };
            }
            Type typeParameterType = typeof(T);
            var resultType = typeParameterType.FullName;

            if (resultType == "System.Object")
            {
                var result = JsonConvert.DeserializeObject<dynamic>(redisCache.Get(rediscacheKey).ToString());
                return result;
            }
            else
            {
                var result = new QueryReturnModel<T>()
                {
                    Items = JsonConvert.DeserializeObject<IEnumerable<T>>(redisCache.Get(rediscacheKey)
                    .ToString()),
                    QueryModelCacheName = rediscacheKey

                };
                return result;
            }
        }

        public SqlMapper.GridReader QueryMultiple(List<string> query, DynamicParameters param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType)
        {
            var multisql = string.Join(";", query);
            return transaction?.Connection.QueryMultiple(multisql, param, transaction);
        }

        public PagedList<T> ToPagedList<T>(string query, DynamicParameters param = null, int pageIndex = 1, int pageSize = 25, IDbTransaction transaction = null, int? commandTimeout = 60, CacheTimeEnum? cacheTime = null, int expireTime = 1, bool dynamicRedisCache = false)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (query.TrimEnd().EndsWith(";"))
            {
                query = query.TrimEnd().TrimEnd(';');
            }
            string rowCountSql = $"select COUNT(*) as TotalCount from ({query});";
            var startRow = (pageIndex - 1) * pageSize;
            var monitorsql = string.Format(query + " limit " + "{0},{1};", startRow, pageSize);
            var sql = rowCountSql + monitorsql;

            if (string.IsNullOrEmpty(query))
            {
                throw new Exception("T-Sql Sorgusu bulunamadı");
            }
            if (cacheTime == null)
            {
                var multiresult = transaction?.Connection.QueryMultiple(sql, param, transaction);
                var resulttotal = multiresult.ReadSingle<int>();
                var resultset = multiresult.Read<T>(false).ToList();
                var totalPage = Math.Ceiling(Convert.ToDouble(resulttotal.ToString()) / Convert.ToDouble(pageSize.ToString()));
                return new PagedList<T>(resultset, pageSize, pageIndex, resulttotal,
                    Convert.ToInt32(totalPage));
            }
            var uniqkey = new StringBuilder();
            uniqkey.Append(query);
            uniqkey.Append(pageIndex);
            uniqkey.Append(pageSize);
            if (param != null)
            {
                var paramList = param.ParameterNames.Select(x => new { ParameterName = x, ParameterValue = param.Get<object>(x) }).ToList();
                uniqkey.Append(SerializerHelper.Serialize(paramList));
            }

            var redisKey = "QueryCache" + GenerateDynamicCacheName(dynamicRedisCache, query).Replace("||", "|") + ":" + uniqkey.ToString().ToMd5();

            if (!redisCache.Exists(redisKey))
            {
                var multiresult2 = transaction?.Connection.QueryMultiple(sql, param, transaction);
                var resulttotal2 = multiresult2.ReadSingle<int>();
                var resultset2 = multiresult2.Read<T>(false).ToList();
                var totalPage2 = Math.Ceiling(Convert.ToDouble(resulttotal2.ToString()) / Convert.ToDouble(pageSize.ToString()));
                PagedList<T> jsonserializedat = new PagedList<T>(resultset2, pageSize, pageIndex, resulttotal2,
               Convert.ToInt32(totalPage2));
                if (resultset2.Count > 0 && resultset2.Count > 0)
                {
                    switch (cacheTime)
                    {
                        case CacheTimeEnum.Year:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddYears(expireTime));
                            break;
                        case CacheTimeEnum.Month:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddMonths(expireTime));
                            break;
                        case CacheTimeEnum.Day:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddDays(expireTime));
                            break;
                        case CacheTimeEnum.Hour:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddHours(expireTime));
                            break;
                        case CacheTimeEnum.Minute:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddMinutes(expireTime));
                            break;
                        case CacheTimeEnum.Second:
                            redisCache.Add(redisKey, jsonserializedat, DateTime.Now.AddSeconds(expireTime));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(cacheTime), cacheTime, null);
                    }
                }
                return jsonserializedat;
            }
            Type typeParameterType = typeof(T);
            var resultType = typeParameterType.FullName;
            if (resultType == "System.Object")
            {
                dynamic d = JsonConvert.DeserializeObject(redisCache.Get(redisKey).ToString());
                var Items = (IEnumerable<T>)d.Items;
                var PageSize = (int)d.PageSize;
                var PageIndex = (int)d.PageIndex;
                var TotalPage = (int)d.TotalPage;
                var TotalRecord = (int)d.TotalRecord;

                return new PagedList<T>(Items, PageSize, PageIndex, TotalRecord, TotalPage);
            }

            return (PagedList<T>)redisCache.Get(redisKey);
        }

        public int Execute(string sql, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text)
        {
            DeleteDynamicCache(sql);
            return transaction.Connection.Execute(sql, param, transaction, commandTimeout, commandType);
        }

        public T ExecuteScalar<T>(string sql, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text, CacheTimeEnum? cacheTimeType = null, int expireTime = 1, bool dynamicRedisCache = false)
        {
            if (cacheTimeType == null)
            {
                var resullt = transaction.Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);

                return resullt;
            }
            var uniqkey = new StringBuilder();
            uniqkey.Append(sql);
            if (param != null)
            {
                var paramList = param.ParameterNames.Select(x => new { ParameterName = x, ParameterValue = param.Get<object>(x) }).ToList();
                uniqkey.Append(SerializerHelper.Serialize(paramList));
            }
            var redisKey = "QueryCache" + GenerateDynamicCacheName(dynamicRedisCache, sql).Replace("||", "|") + ":" + uniqkey.ToString().ToMd5();

            if (!redisCache.Exists(redisKey))
            {
                var cacheList = transaction.Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
                if (cacheList == null) return default;
                switch (cacheTimeType)
                {
                    case CacheTimeEnum.Year:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddYears(expireTime));
                        break;
                    case CacheTimeEnum.Month:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddMonths(expireTime));
                        break;
                    case CacheTimeEnum.Day:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddDays(expireTime));
                        break;
                    case CacheTimeEnum.Hour:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddHours(expireTime));
                        break;
                    case CacheTimeEnum.Minute:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddMinutes(expireTime));
                        break;
                    case CacheTimeEnum.Second:
                        redisCache.Add(redisKey, cacheList, DateTime.Now.AddSeconds(expireTime));
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(cacheTimeType), cacheTimeType, null);
                }
                return cacheList;
            }
            var result = (T)redisCache.Get(redisKey);
            return result;
        }

        private IEnumerable<T> QueryRun<T>(string query, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text, CacheTimeEnum? cacheTimeType = null, int expireTime = 1, bool dymanamiccache = false)
        {
            return transaction?.Connection.Query<T>(string.Format(query), param, transaction, false, commandTimeout);
        }

        public long EntitySaveChange(string query, DynamicParameters param, int? commandTimeout = null, IDbTransaction transaction = null,
            bool lastid = false, string processname = null, string idcolumnname = null)
        {
            if (lastid)
            {
                DeleteDynamicCache(query);
                if (string.IsNullOrEmpty(idcolumnname))
                {
                    query = query + (query.TrimEnd().EndsWith(";") ? "" : ";") + "SELECT SCOPE_IDENTITY();";
                }
                else
                {
                    query = query.TrimEnd().TrimEnd(';').TrimEnd() + " RETURNING " + idcolumnname + ";";
                }
                return QueryRun<long>(query, param, transaction).SingleOrDefault();
            }
            var result = Execute(query, param, transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
            return result;
        }

        private string GenerateDynamicCacheName(bool dynamicRedisCache, string query)
        {
            var dynamiccachename = string.Empty;
            if (!dynamicRedisCache) return dynamiccachename;
            const string regexpattern = @"([Ff][Rr][Oo][Mm]|[Jj][Oo][Ii][Nn])[ \n\v\t\d\a\b\f\r\e\s]*(?<table>\S+)[ ]*";
            if (Regex.IsMatch(query, regexpattern,
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline))
            {
                var tableNames = new List<string>();
                var match = Regex.Match(query, regexpattern,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
                while (match.Success)
                {
                    if (tableNames.FirstOrDefault(x => x.Equals(match.Groups["table"].Value.Replace("\"", ""))) ==
                        null)
                    {
                        dynamiccachename += "|" + match.Groups["table"].Value.Replace("\"", "").TrimEnd(' ').TrimStart(' ') + "|";
                        tableNames.Add(match.Groups["table"].Value.Replace("\"", "").TrimEnd(' ').TrimStart(' '));
                    }

                    match = match.NextMatch();

                }

                tableNames.Clear();
            }

            return dynamiccachename;
        }
        private void DeleteDynamicCache(string query)
        {
            // Mysql
            //const string regexpattern = @"(from|join|JOIN|FROM|Join|From|insert into|INSERT INTO|Insert Into|Insert into|Replace Into|Replace into|replace into|Update|update|UPDATE)[ ]*(?<table>\S+)[ ]*";
            //Postgre
            const string regexpattern = @"([Dd][Ee][Ll][Ee][Tt][Ee][ \n\v\t\d\a\b\f\r\e]*[Ff][Rr][Oo][Mm]|[Ii][Nn][Ss][Ee][Rr][Tt][ \n\v\t\d\a\b\f\r\e]*[Ii][Nn][Tt][Oo]|[Uu][Pp][Dd][Aa][Tt][Ee]|[Rr][Ee][Pp][Ll][Aa][Cc][Ee][ \n\v\t\d\a\b\f\r\e]*[Ii][Nn][Tt][Oo])[ \n\v\t\d\a\b\f\r\e]*(?<table>[a-zA-Z0-9_.-]+)[ \t\n\r\f\v]*[\(]?";
            if (Regex.IsMatch(query, regexpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline))
            {
                var match = Regex.Match(query, regexpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
                while (match.Success)
                {
                    redisCache.DeleteFindKey("*|" + match.Groups["table"].Value.Replace("\"", "").TrimEnd(' ').TrimStart(' ') + "|*");
                    match = match.NextMatch();
                }
            }
        }

    }
}
