using System;
using System.Linq.Expressions;

namespace FlowDMApi.Core.Extentions.Where
{
    public class SqlLam<T> : SqlLamBase
    {
        public SqlLam()
        {
            _builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T>(), _defaultAdapter);
            _resolver = new LambdaResolver(_builder);
        }

        public SqlLam(Expression<Func<T, bool>> expression) : this()
        {
            Where(expression);
        }

        internal SqlLam(SqlQueryBuilder builder, LambdaResolver resolver)
        {
            _builder = builder;
            _resolver = resolver;
        }

        public SqlLam<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public SqlLam<T> And(Expression<Func<T, bool>> expression)
        {
            _builder.And();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlLam<T> Or(Expression<Func<T, bool>> expression)
        {
            _builder.Or();
            _resolver.ResolveQuery(expression);
            return this;
        }
        public void Dispose()
        {
            this.SqlBuilder.Parameters.Clear();
            this.SqlBuilder.GroupByList.Clear();
            this.SqlBuilder.HavingConditions.Clear();
            this.SqlBuilder.JoinExpressions.Clear();
            this.SqlBuilder.OrderByList.Clear();
            this.SqlBuilder.SelectionList.Clear();
            this.SqlBuilder.WhereConditions.Clear();
        }
    }
}
