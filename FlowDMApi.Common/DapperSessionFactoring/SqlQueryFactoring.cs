using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using FlowDMApi.Common.Command.Context;
using FlowDMApi.Common.Command.Paged;
using FlowDMApi.Core.Extentions.Cache;
using FlowDMApi.Core.Extentions.ParameterExtention;
namespace FlowDMApi.Common.DapperSessionFactoring
{
    public class SqlQueryFactoring : ISQLQuery
    {
        private static readonly object Padlock = new object();
        private SessionFactoringModels _builderActions;
        private Commander _commandContext;
        private IDbTransaction _transaction;
        private List<string> _parameterNameList;
        private bool _searchedParameterNamesFromQueryString;

        public SqlQueryFactoring(IDbTransaction transaction)
        {
            if (_commandContext == null)
            {
                lock (Padlock)
                {
                    if (_commandContext == null)
                    {
                        _commandContext = new Commander();
                    }
                }
            }

            _transaction = transaction;
            _builderActions = new SessionFactoringModels();
            _parameterNameList = new List<string>();
            _searchedParameterNamesFromQueryString = false;
        }
        private void SearchParametersInSqlString()
        {
            if (!_searchedParameterNamesFromQueryString)
            {
                const string regexpattern = "\\@([\\w.$]+|\"[^\"]+\" | '[^'] + ')";
                if (Regex.IsMatch(_builderActions.Query, regexpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline))
                {
                    var match = Regex.Match(_builderActions.Query, regexpattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
                    while (match.Success)
                    {

                        if (_parameterNameList.FirstOrDefault(x => x.Equals(match.Groups[1].Value)) == null)
                        {
                            _parameterNameList.Add(match.Groups[1].Value.TrimStart('@'));
                        }
                        match = match.NextMatch();
                    }
                }
                _searchedParameterNamesFromQueryString = true;
            }
        }
        public object UniqueResult()
        {
            return _commandContext.ExecuteScalar<object>(
                sql: _builderActions.Query,
                param: _builderActions.Parameters,
                transaction: _transaction,
                commandTimeout: _builderActions.TimeOut,
                expireTime: _builderActions.CacheTime,
                cacheTimeType: _builderActions.CacheTimeEnum,
                dynamicRedisCache: _builderActions.DynamicCache);
        }
        public T UniqueResult<T>()
        {

            if (typeof(T).IsValueType)
            {
                return _commandContext.ExecuteScalar<T>(
                    sql: _builderActions.Query,
                    param: _builderActions.Parameters,
                    transaction: _transaction,
                    commandTimeout: _builderActions.TimeOut,
                    expireTime: _builderActions.CacheTime,
                    cacheTimeType: _builderActions.CacheTimeEnum,
                    dynamicRedisCache: _builderActions.DynamicCache);
            }

            var objList = _commandContext.Query<T>(
                query: _builderActions.Query,
                commandTimeout: _builderActions.TimeOut,
                param: _builderActions.Parameters,
                transaction: _transaction,
                dynamicRedisCache: _builderActions.DynamicCache,
                cacheTimeType: _builderActions.CacheTimeEnum,
                expireTime: _builderActions.CacheTime
            ).Items?.ToList();
            var obj = default(T);
            if (objList != null)
            {
                obj = objList.SingleOrDefault();
            }
            return obj == null ? default(T) : obj;
        }
        /// <summary>
        /// Postgresql kullanımı.
        /// </summary>
        /// <param name="idcolumnname"></param>
        /// <returns></returns>
        public long ExecuteUpdateAndGetInsertId(string idcolumnname)
        {
            return _commandContext.EntitySaveChange(
                query: _builderActions.Query,
                commandTimeout: _builderActions.TimeOut,
                param: _builderActions.Parameters,
                transaction: _transaction,
                lastid: true,
                processname: _builderActions.ProcessName,
                idcolumnname: idcolumnname);
        }
        /// <summary>
        /// Mysql kullanımı.
        /// </summary>
        /// <returns></returns>
        public long ExecuteUpdateAndGetInsertId()
        {
            return _commandContext.EntitySaveChange(
                query: _builderActions.Query,
                commandTimeout: _builderActions.TimeOut,
                param: _builderActions.Parameters,
                transaction: _transaction,
                lastid: true,
                processname: _builderActions.ProcessName,
                idcolumnname: "");
        }

        public PagedList<T> ToTransformedDbPagedList<T>(int pageIndex, int pageSize)
        {
            return _commandContext.ToPagedList<T>(query: _builderActions.Query,
                param: _builderActions.Parameters,
                transaction: _transaction,
                pageIndex: pageIndex,
                pageSize: pageSize,
                commandTimeout: _builderActions.TimeOut,
                cacheTime: _builderActions.CacheTimeEnum,
                expireTime: _builderActions.CacheTime);
        }
        public IQuery SetBulkParameter(IEnumerable parameters)
        {
            return this;
        }

        public IQuery CacheTime(int time)
        {
            _builderActions.CacheTime = time;
            return this;
        }
        public long ExecuteUpdate()
        {
            return _commandContext.EntitySaveChange(query: _builderActions.Query, _builderActions.Parameters, _builderActions.TimeOut, _transaction, false, _builderActions.ProcessName);

        }

        public IQuery DynamicCache(CacheTimeEnum cacheType)
        {
            _builderActions.DynamicCache = true;
            _builderActions.CacheTimeEnum = cacheType;
            return this;
        }

        public IQuery DynamicCache(CacheTimeEnum cacheType, int time)
        {
            _builderActions.CacheTime = time;
            _builderActions.DynamicCache = true;
            _builderActions.CacheTimeEnum = cacheType;
            return this;
        }
        public IQuery StaticCache(CacheTimeEnum cacheType)
        {
            _builderActions.DynamicCache = false;
            _builderActions.CacheTimeEnum = cacheType;
            return this;
        }
        public IQuery SetTimeout(int timeout)
        {
            _builderActions.TimeOut = timeout;
            return this;
        }

        public IQuery SetParameterList(string name, IEnumerable vals)
        {
            _builderActions.Parameters.Add(name, vals);
            return this;
        }

        public IQuery SetParameter(string name, object val, DbType type)
        {
            _builderActions.Parameters.Add(name, val, type);
            return this;
        }

        public IQuery SqlWatchEnable()
        {
            _builderActions.SqlWatch = true;
            return this;
        }
        public IQuery SetParameter(string name, object val)
        {
            _builderActions.Parameters.Add(name, val);
            return this;

        }

        public List<T> ToTransformedList<T>() where T : class
        {
            var result = _commandContext.Query<T>(
                query: _builderActions.Query,
                param: _builderActions.Parameters,
                transaction: _transaction,
                commandTimeout: _builderActions.TimeOut,
                expireTime: _builderActions.CacheTime,
                cacheTimeType: _builderActions.CacheTimeEnum,
                dynamicRedisCache: _builderActions.DynamicCache
                );
            return result?.Items?.ToList();
        }

        public List<T> ToList<T>()
        {
            return _commandContext.Query<T>(
                query: _builderActions.Query,
                param: _builderActions.Parameters,
                transaction: _transaction,
                commandTimeout: _builderActions.TimeOut,
                cacheTimeType: _builderActions.CacheTimeEnum,
                expireTime: _builderActions.CacheTime,
                dynamicRedisCache: _builderActions.DynamicCache
            ).Items?.ToList();
        }

        public List<T> List<T>()
        {
            return _commandContext.Query<T>(
                query: _builderActions.Query,
                param: _builderActions.Parameters,
                transaction: _transaction,
                commandTimeout: _builderActions.TimeOut,
                cacheTimeType: _builderActions.CacheTimeEnum,
                expireTime: _builderActions.CacheTime,
                dynamicRedisCache: _builderActions.DynamicCache
            ).Items?.ToList();
        }

        public IQuery SetParameterIfExist(string parameterName, object parameterValue, DbType type)
        {
            SearchParametersInSqlString();
            parameterName = parameterName.TrimStart().TrimStart('@');
            if (_parameterNameList.Contains(parameterName))
            {
                _builderActions.Parameters.Add(parameterName, parameterValue, type);
                return this;
            }
            return this;
        }

        public IQuery SetParameterIfExist(string parameterName, object parameterValue)
        {
            SearchParametersInSqlString();
            parameterName = parameterName.TrimStart().TrimStart('@');
            if (_parameterNameList.Contains(parameterName))
            {
                _builderActions.Parameters.Add(parameterName, parameterValue);
                return this;
            }
            return this;
        }

        public IQuery SetParameterListIfExist(string parameterName, IEnumerable parameterValue)
        {
            SearchParametersInSqlString();
            parameterName = parameterName.TrimStart().TrimStart('@');
            if (_parameterNameList.Contains(parameterName))
            {
                _builderActions.Parameters.Add(parameterName, parameterValue);
                return this;
            }
            return this;
        }


        public T TransformedUniqueResult<T>() where T : class
        {
            return _commandContext.Query<T>(
                query: _builderActions.Query,
                commandTimeout: _builderActions.TimeOut,
                param: _builderActions.Parameters,
                transaction: _transaction,
                dynamicRedisCache: _builderActions.DynamicCache,
                cacheTimeType: _builderActions.CacheTimeEnum,
                expireTime: _builderActions.CacheTime
                ).Items?.SingleOrDefault();
        }


        public IQuery SetParameters(object entity)
        {
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                var parametername = propertyInfo.Name;
                var parametervalue = propertyInfo.GetValue(entity);
                _builderActions.Parameters.Add(parametername, parametervalue);
            }
            return this;
        }


        public IQuery CreateSQLQuery(string queryString)
        {
            _builderActions.Query = queryString;
            return this;
        }

        public IQuery CreateSQLQuery(string queryString, List<Parameter> parameters)
        {
            var returnObject = CreateSQLQuery(queryString);
            foreach (var item in parameters)
            {
                returnObject.SetParameter(item.Name, item.Value, item.Type);
            }
            return this;
        }

        public IQuery CreateSQLQuery(string sql, string orderProperty,
            string orderDirection)
        {
            if (!String.IsNullOrWhiteSpace(orderProperty))
            {
                // Sıralama yönü yoksa ASC yap
                if (String.IsNullOrWhiteSpace(orderDirection))
                {
                    orderDirection = "ASC";
                }

                // Order by varsa kaldır
                int existingOrderPos = sql.LastIndexOf(" order ", StringComparison.InvariantCultureIgnoreCase);
                int existingfromPos = sql.LastIndexOf(" from ", StringComparison.InvariantCultureIgnoreCase);

                if (existingfromPos > 0 && existingfromPos > existingOrderPos)
                {
                    existingOrderPos = -1;
                }

                if (existingOrderPos > 0)
                {
                    sql = sql.Substring(0, existingOrderPos);
                }
                // Order by ekle
                sql += String.Format(" order by {0} {1}", orderProperty, orderDirection);
            }

            _builderActions.Query = sql;
            return this;
        }
    }
}