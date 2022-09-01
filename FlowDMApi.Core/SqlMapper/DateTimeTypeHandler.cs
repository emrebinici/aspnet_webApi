using System;
using System.Data;

namespace FlowDMApi.Core.SqlMapper
{
   public class DateTimeTypeHandler : Dapper.SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = (value == null)
                ? (object) DBNull.Value
                : Convert.ToDateTime((string)value).ToString("yyyy-MM-dd HH:mm:ss");
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value)
        {
            Type t = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

            return  (value == null) ? null : Convert.ChangeType(value, t);
        }
    }
}
