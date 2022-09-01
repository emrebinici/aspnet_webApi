using System;
using System.Data;
using FlowDMApi.Core.Extentions;

namespace FlowDMApi.Core.SqlMapper
{
    public class JsonObjectTypeHandler : Dapper.SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : SerializerHelper.Serialize(value);
            parameter.DbType = DbType.String;
        }
        public object Parse(Type destinationType, object value)
        {
            return SerializerHelper.DeserializeObject(value.ToString(),destinationType);
        }
    }
}
