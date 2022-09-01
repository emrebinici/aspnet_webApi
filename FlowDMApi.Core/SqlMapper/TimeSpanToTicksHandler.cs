using System;
using System.Data;

namespace FlowDMApi.Core.SqlMapper
{
    public class TimeSpanToTicksHandler : Dapper.SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                :   new TimeSpan((long)value);
            parameter.DbType = DbType.Time;
        }

        public object Parse(Type destinationType, object value)
        {
            Type t = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            return (value == null) ? null : Convert.ChangeType(value, t);
        }
    }
}
