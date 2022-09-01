using System.Collections.Generic;
using FlowDMApi.Core.Extentions.ParameterExtention;

namespace FlowDMApi.Common.DapperSessionFactoring
{
    public interface ISQLQuery : IQuery
    {
        IQuery CreateSQLQuery(string queryString);
        IQuery CreateSQLQuery(string queryString, List<Parameter> parameters);

        IQuery CreateSQLQuery(string sql, string orderProperty,
            string orderDirection);

    }
}