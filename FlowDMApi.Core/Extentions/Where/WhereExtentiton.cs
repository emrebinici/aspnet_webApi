using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FlowDMApi.Core.Extentions.Where
{
    public class WhereExtentiton<T> 
    {

        public static WhereExtentionReturn Method(Expression<Func<T, bool>> expression)
        {
            SqlLam<T> _sql = new SqlLam<T>();
            _sql.Dispose();
            _sql.Where(expression);
            var param = _sql.QueryParameters;
            var returnModel = new WhereExtentionReturn()
            {
                Coundation = Regex.Match(_sql.QueryString, @"WHERE \s*(?<value>\s*.*)\s*").Groups["value"]
                    .ToString(),
                Parameter = param
            };
            return returnModel;
        }
       
    }
}