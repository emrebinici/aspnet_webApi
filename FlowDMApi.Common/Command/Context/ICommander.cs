using System.Collections.Generic;
using System.Data;
using Dapper;
using FlowDMApi.Common.Command.Paged;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Common.Command.Context
{
    internal interface ICommander
    {
        QueryReturnModel<T> Query<T>(string query, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text, CacheTimeEnum? cacheTimeType = null, int expireTime = 1, bool dynamicRedisCache = false);
        SqlMapper.GridReader QueryMultiple(List<string> querylist, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = 60, CommandType? commandType = CommandType.Text);
        PagedList<T> ToPagedList<T>(string query, DynamicParameters param = null, int pageIndex = 1, int pageSize = 25, IDbTransaction transaction = null, int? commandTimeout = 60, CacheTimeEnum? cacheTime = null, int expireTime = 1, bool dynamicRedisCache = false);
        int Execute(string sql, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text);
        T ExecuteScalar<T>(string sql, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text, CacheTimeEnum? cacheTimeType = null, int expireTime = 1, bool dynamicRedisCache = false);
        long EntitySaveChange(string query, DynamicParameters entity, int? commandTimeout = null,
            IDbTransaction transaction = null,
            bool lastid = false, string processname = null, string idcolumnname = null);
    }
}
