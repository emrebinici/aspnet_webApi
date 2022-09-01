using System.Collections.Generic;

namespace FlowDMApi.Core.Extentions.Where
{
    public abstract class SqlLamBase
    {
        internal static ISqlAdapter _defaultAdapter = new MySqlAdapter();
        internal SqlQueryBuilder _builder;
        internal LambdaResolver _resolver;

        public SqlQueryBuilder SqlBuilder { get { return _builder; } }

        public string QueryString
        {
            get { return _builder.QueryString; }
        }

        public string QueryStringPage(int pageSize, int? pageNumber = null)
        {
            return _builder.QueryStringPage(pageSize, pageNumber);
        }

        public IDictionary<string, object> QueryParameters
        {
            get { return _builder.Parameters; }
        }

        public string[] SplitColumns
        {
            get { return _builder.SplitColumns.ToArray(); }
        }

        public static void SetAdapter()
        {
            _defaultAdapter = GetAdapterInstance();
        }

        private static ISqlAdapter GetAdapterInstance()
        {

            return new MySqlAdapter();
        }
    }
}
