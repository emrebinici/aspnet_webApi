using System;
using System.Collections;
using System.Data;

namespace FlowDMApi.Core.Extentions.ParameterExtention
{
    public class Parameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        public DbType Type { get; private set; }

        public static Parameter Create(string name, object value)
        {
            return new Parameter { Name = name, Value = value };
        }

        public static Parameter Create(string name, DateTime? value)
        {
            return new Parameter { Name = name, Value = value, Type = DbType.DateTime};
        }

        public static Parameter Create(string name, string value)
        {
            return new Parameter { Name = name, Value = value, Type = DbType.String };
        }

        public static Parameter Create(string name, IEnumerable value)
        {
            return new Parameter { Name = name, Value = value };
        }

        public static Parameter Create(string name, object value, DbType type)
        {

            return new Parameter { Name = name, Value = value, Type = type };
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", Name, Value);
        }
    }
}
