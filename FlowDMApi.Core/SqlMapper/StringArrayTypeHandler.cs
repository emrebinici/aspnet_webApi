using System;
using System.Data;

namespace FlowDMApi.Core.SqlMapper
{
    public class StringArrayTypeHandler : Dapper.SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
           
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : string.Join(",", (string[])value);
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value)
        {
            string[] roles = value.ToString().Split(',');
            return roles;
        }
    }
}
