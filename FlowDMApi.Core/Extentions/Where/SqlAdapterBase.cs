namespace FlowDMApi.Core.Extentions.Where
{
    class SqlAdapterBase
    {
        public string QueryString(string selection, string source, string conditions, string order, string grouping, string having)
        {
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5}",
                selection, source, conditions, order, grouping, having);
        }
    }
    class SqlServerAdapterBase : SqlAdapterBase
    {
        public string QueryStringPage(string source, string selection, string conditions, string order,
            int pageSize)
        {
            return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}",
                selection, source, conditions, order, pageSize);
        }


        public string Table(string tableName)
        {
            return string.Format("{0}", tableName);
        }

        public string Field(string tableName, string fieldName)
        {
            return string.Format("{0}.{1}", tableName, fieldName);
        }

        public string Parameter(string parameterId)
        {
            return "@" + parameterId;
        }
    }
    class MySqlAdapter : SqlServerAdapterBase, ISqlAdapter
    {
        public string QueryStringPage(string source, string selection, string conditions, string order,
            int pageSize, int pageNumber)
        {
            return string.Format("SELECT {0} FROM {1} {2} {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY",
                selection, source, conditions, order, pageSize * (pageNumber - 1), pageSize);
        }
    }
}
