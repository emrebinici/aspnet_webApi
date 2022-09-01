using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using FlowDMApi.Common.Command.Paged;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Common.DapperSessionFactoring
{
    public interface IQuery
    {
        object UniqueResult();
        T UniqueResult<T>();
        long ExecuteUpdate();
        IQuery CacheTime(int time);
        IQuery DynamicCache(CacheTimeEnum cacheType);
        IQuery DynamicCache(CacheTimeEnum cacheType, int time);
        IQuery StaticCache(CacheTimeEnum cacheType);
        IQuery SetTimeout(int timeout);
        IQuery SetParameterList(string name, IEnumerable vals);
        IQuery SqlWatchEnable();
        IQuery SetParameter(string name, object val, DbType type);
        IQuery SetParameter(string name, object val);
        IQuery SetParameters(object entity);
        List<T> ToTransformedList<T>() where T : class;
        List<T> ToList<T>();
        List<T> List<T>();
        IQuery SetParameterIfExist(string parameterName, object parameterValue, DbType type);
        IQuery SetParameterIfExist(string parameterName, object parameterValue);
        IQuery SetParameterListIfExist(string parameterName, IEnumerable parameterValue);
        T TransformedUniqueResult<T>() where T : class;
        long ExecuteUpdateAndGetInsertId(string idcolumnname);
        long ExecuteUpdateAndGetInsertId();
        PagedList<T> ToTransformedDbPagedList<T>(int pageIndex, int pageSize);

    }
}