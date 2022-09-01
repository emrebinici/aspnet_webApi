using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlowDMApi.Core.Extentions.Where
{
    public partial class SqlQueryBuilder
    {
        internal ISqlAdapter Adapter { get; set; }

        private const string PARAMETER_PREFIX = "Param";

        private readonly List<string> _tableNames = new List<string>();
        private readonly List<string> _joinExpressions = new List<string>();
        private readonly List<string> _selectionList = new List<string>();
        private readonly List<string> _conditions = new List<string>();
        private readonly List<string> _sortList = new List<string>();
        private readonly List<string> _groupingList = new List<string>();
        private readonly List<string> _havingConditions = new List<string>();
        private readonly List<string> _splitColumns = new List<string>();
        private int _paramIndex;

        public List<string> TableNames { get { return _tableNames; } }
        public List<string> JoinExpressions { get { return _joinExpressions; } }
        public List<string> SelectionList { get { return _selectionList; } }
        public List<string> WhereConditions { get { return _conditions; } }
        public List<string> OrderByList { get { return _sortList; } }
        public List<string> GroupByList { get { return _groupingList; } }
        public List<string> HavingConditions { get { return _havingConditions; } }
        public List<string> SplitColumns { get { return _splitColumns; } }
        public int CurrentParamIndex { get { return _paramIndex; } }

        private string Source
        {
            get
            {
                var joinExpression = string.Join(" ", _joinExpressions);
                return string.Format("{0} {1}", Adapter.Table(_tableNames.First()), joinExpression);
            }
        }

        private string Selection
        {
            get
            {
                if (_selectionList.Count == 0)
                    return string.Format("{0}.*", Adapter.Table(_tableNames.First()));
                else
                    return string.Join(", ", _selectionList);
            }
        }

        private string Conditions
        {
            get
            {
                if (_conditions.Count == 0)
                    return "";
                else
                    return "WHERE " + string.Join("", _conditions);
            }
        }

        private string Order
        {
            get
            {
                if (_sortList.Count == 0)
                    return "";
                else
                    return "ORDER BY " + string.Join(", ", _sortList);
            }
        }

        private string Grouping
        {
            get
            {
                if (_groupingList.Count == 0)
                    return "";
                else
                    return "GROUP BY " + string.Join(", ", _groupingList);
            }
        }

        private string Having
        {
            get
            {
                if (_havingConditions.Count == 0)
                    return "";
                else
                    return "HAVING " + string.Join(" ", _havingConditions);
            }
        }

        public IDictionary<string, object> Parameters { get; private set; }

        public string QueryString
        {
            get { return Adapter.QueryString(Selection, Source, Conditions, Grouping, Having, Order); }
        }

        public string QueryStringPage(int pageSize, int? pageNumber = null)
        {
            if (pageNumber.HasValue)
            {
                if (_sortList.Count == 0)
                    throw new Exception("Pagination requires the ORDER BY statement to be specified");

                return Adapter.QueryStringPage(Source, Selection, Conditions, Order, pageSize, pageNumber.Value);
            }

            return Adapter.QueryStringPage(Source, Selection, Conditions, Order, pageSize);
        }

        internal SqlQueryBuilder(string tableName, ISqlAdapter adapter)
        {
            _tableNames.Add(tableName);
            Adapter = adapter;
            Parameters = new ExpandoObject();
            _paramIndex = 0;
        }

        #region helpers
        private string NextParamId()
        {
            ++_paramIndex;
            return PARAMETER_PREFIX + _paramIndex.ToString(CultureInfo.InvariantCulture);
        }

        private void AddParameter(string key, object value)
        {
            if (!Parameters.ContainsKey(key))
                Parameters.Add(key, value);
        }
        #endregion
        public void BeginExpression()
        {
            _conditions.Add("(");
        }

        public void EndExpression()
        {
            _conditions.Add(")");
        }

        public void And()
        {
            if (_conditions.Count > 0)
                _conditions.Add(" AND ");
        }

        public void Or()
        {
            if (_conditions.Count > 0)
                _conditions.Add(" OR ");
        }

        public void Not()
        {
            _conditions.Add(" NOT ");
        }

        public void QueryByField(string tableName, string fieldName, string op, object fieldValue)
        {
            var paramId = NextParamId();
            string newCondition = string.Format("{0} {1} {2}",
                Adapter.Field(tableName, fieldName),
                op,
                Adapter.Parameter(paramId));

            _conditions.Add(newCondition);
            AddParameter(paramId, fieldValue);
        }

        public void QueryByFieldLike(string tableName, string fieldName, string fieldValue)
        {
            var paramId = NextParamId();
            string newCondition = string.Format("{0} LIKE {1}",
                Adapter.Field(tableName, fieldName),
                Adapter.Parameter(paramId));

            _conditions.Add(newCondition);
            AddParameter(paramId, fieldValue);
        }

        public void QueryByFieldNull(string tableName, string fieldName)
        {
            _conditions.Add(string.Format("{0} IS NULL", Adapter.Field(tableName, fieldName)));
        }

        public void QueryByFieldNotNull(string tableName, string fieldName)
        {
            _conditions.Add(string.Format("{0} IS NOT NULL", Adapter.Field(tableName, fieldName)));
        }

        public void QueryByFieldComparison(string leftTableName, string leftFieldName, string op,
            string rightTableName, string rightFieldName)
        {
            string newCondition = string.Format("{0} {1} {2}",
            Adapter.Field(leftTableName, leftFieldName),
            op,
            Adapter.Field(rightTableName, rightFieldName));

            _conditions.Add(newCondition);
        }

        public void QueryByIsIn(string tableName, string fieldName, SqlLamBase sqlQuery)
        {
            var innerQuery = sqlQuery.QueryString;
            foreach (var param in sqlQuery.QueryParameters)
            {
                var innerParamKey = "Inner" + param.Key;
                innerQuery = Regex.Replace(innerQuery, param.Key, innerParamKey);
                AddParameter(innerParamKey, param.Value);
            }

            var newCondition = string.Format("{0} IN ({1})", Adapter.Field(tableName, fieldName), innerQuery);

            _conditions.Add(newCondition);
        }

        public void QueryByIsIn(string tableName, string fieldName, IEnumerable<object> values)
        {
            var paramIds = values.Select(x =>
            {
                var paramId = NextParamId();
                AddParameter(paramId, x);
                return Adapter.Parameter(paramId);
            });

            var newCondition = string.Format("{0} IN ({1})", Adapter.Field(tableName, fieldName), string.Join(",", paramIds));
            _conditions.Add(newCondition);
        }
    }
}
