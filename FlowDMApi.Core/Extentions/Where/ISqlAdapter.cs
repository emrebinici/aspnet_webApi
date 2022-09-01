namespace FlowDMApi.Core.Extentions.Where
{
    interface ISqlAdapter
    {
        string QueryString(string selection, string source, string conditions,
            string order, string grouping, string having);

        string QueryStringPage(string selection, string source, string conditions, string order,
            int pageSize, int pageNumber);

        string QueryStringPage(string selection, string source, string conditions, string order,
            int pageSize);

        string Table(string tableName);
        string Field(string tableName, string fieldName);
        string Parameter(string parameterId);
    }
}
