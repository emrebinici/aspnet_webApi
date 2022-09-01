using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using AutoMapper;
using FlowDMApi.Common.DapperSessionFactoring;
using FlowDMApi.Core.Extentions.ParameterExtention;
using FlowDMApi.Core.RedisFactory;
using FlowDMApi.Core.UnitOfWork;

namespace FlowDMApi.Common.RepositoryBase
{
    public abstract class RepositoryBase
    {
        public readonly IRedisCache Redis = new RedisCache();
        private IUnitOfWork _unitOfWork;
        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ISQLQuery Session => new SqlQueryFactoring(_unitOfWork.Transaction);
        public IQuery CreateSQLQuery(string sql, string orderProperty,
            string orderDirection)
        {
            if (!String.IsNullOrWhiteSpace(orderProperty))
            {
                // Sıralama yönü yoksa ASC yap
                if (String.IsNullOrWhiteSpace(orderDirection))
                {
                    orderDirection = "ASC";
                }

                // Order by varsa kaldır
                int existingOrderPos = sql.LastIndexOf(" order ", StringComparison.InvariantCultureIgnoreCase);
                int existingfromPos = sql.LastIndexOf(" from ", StringComparison.InvariantCultureIgnoreCase);

                if (existingfromPos > 0 && existingfromPos > existingOrderPos)
                {
                    existingOrderPos = -1;
                }

                if (existingOrderPos > 0)
                {
                    sql = sql.Substring(0, existingOrderPos);
                }
                // Order by ekle
                sql += String.Format(" order by {0} {1}", orderProperty, orderDirection);
            }

            return Session.CreateSQLQuery(sql);
        }
        protected virtual IQuery CreateSQLQuery(string sql, List<Parameter> parameters)
        {
            return Session.CreateSQLQuery(sql, parameters);
        }
        protected virtual IQuery CreateSQLQuery(string sql)
        {
            return Session.CreateSQLQuery(sql);
        }

        protected virtual T CreateSQLScalarSingle<T>(string sql, string columnAlias, DataType columnType, params Parameter[] parameters) where T : IConvertible
        {
            return CreateSQLQuery(sql, parameters.ToList()).UniqueResult<T>();
        }

        protected virtual T CreateSQLQuerySingle<T>(string sql, List<Parameter> parameters) where T : class, new()
        {
            return CreateSQLQuery(sql, parameters).TransformedUniqueResult<T>();
        }
        protected virtual List<T> CreateSQLQueryList<T>(string sql, params Parameter[] parameters) where T : class, new()
        {
            return CreateSQLQueryList<T>(sql, parameters.ToList());
        }
        protected virtual List<T> CreateSQLQueryList<T>(string sql, List<Parameter> parameters) where T : class, new()
        {
            return CreateSQLQuery(sql, parameters).ToTransformedList<T>();
        }

        protected virtual List<T> CreateSQLScalarList<T>(string sql, string columnAlias, DataType columnType, params Parameter[] parameters)
        {
            return CreateSQLQuery(sql, parameters.ToList()).ToList<T>();
        }
        public virtual long? UniqueResultSql(string sqlStr)
        {
            return (long?)Session.CreateSQLQuery(sqlStr).UniqueResult<long>();
        }
        public virtual T UniqueResultSql<T>(string sqlStr, string aliasName) where T : IConvertible
        {
            return
                Session.CreateSQLQuery(sqlStr)
                    .UniqueResult<T>();
        }
        public virtual T UniqueResultSql<T>(string sqlStr) where T : IConvertible
        {
            return
                CreateSQLQuery(sqlStr, null)
                    .UniqueResult<T>();
        }

        public virtual IList<T> List<T>(string sqlStr, string alias)
        {
            return
                Session.CreateSQLQuery(sqlStr)
                    .List<T>();
        }
        public virtual T UniqueResultSql<T>(string sqlStr, string aliasName, params Parameter[] parameters) where T : IConvertible
        {
            var query = CreateSQLQuery(sqlStr, parameters?.ToList());
            return query.UniqueResult<T>();
        }
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1622:GenericTypeParameterDocumentationMustHaveText",
            Justification = "Reviewed. Suppression is OK here.")]
        public virtual TSecond Map<TFirst, TSecond>(TFirst data)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<TFirst, TSecond>(); });
            var mapper = new Mapper(config);
            return mapper.Map<TFirst, TSecond>(data);
        }
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1622:GenericTypeParameterDocumentationMustHaveText",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<TSecond> Map<TFirst, TSecond>(IList<TFirst> data)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<TFirst, TSecond>(); });
            var mapper = new Mapper(config);
            return mapper.Map<IList<TFirst>, IList<TSecond>>(data);
        }
        protected internal string getDate(DateTime dateTime)
        {
            return "'" + dateTime.ToString("yyyy-MM-dd") + "'";
        }
        protected internal string getDateTime(DateTime dateTime)
        {
            return "'" + dateTime.ToString("yyyy-MM-dd HH:mm") + "'";
        }
        protected internal string getTime(TimeSpan timeSpan)
        {
            return "'" + timeSpan.ToString("hh\\:mm") + "'";
        }

        public virtual long ExecuteUpdate(string sqlStr, params Parameter[] parameters)
        {
            var query = CreateSQLQuery(sqlStr, parameters?.ToList());
            return query.ExecuteUpdate();
        }
        public virtual T UniqueResultSql<T>(string sqlStr, params Parameter[] parameters) where T : IConvertible
        {
            var query = CreateSQLQuery(sqlStr, parameters?.ToList());
            return query.UniqueResult<T>();
        }
        public string GetEnglishText(string text)
        {
            var returnValue = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] >= 'a' && text[i] <= 'z') || (text[i] >= 'A' && text[i] <= 'Z') ||
                    (text[i] >= '0' && text[i] <= '9'))
                    returnValue.Append(text[i]);

                switch (text[i])
                {
                    case ' ':
                    case '-':
                        returnValue.Append('_');
                        break;

                    case 'ı':
                        returnValue.Append('i');
                        break;

                    case 'İ':
                        returnValue.Append('I');
                        break;

                    case 'ğ':
                        returnValue.Append('g');
                        break;

                    case 'Ğ':
                        returnValue.Append('G');
                        break;

                    case 'ü':
                        returnValue.Append('u');
                        break;

                    case 'Ü':
                        returnValue.Append('U');
                        break;

                    case 'ş':
                        returnValue.Append('s');
                        break;

                    case 'Ş':
                        returnValue.Append('S');
                        break;

                    case 'ç':
                        returnValue.Append('c');
                        break;

                    case 'Ç':
                        returnValue.Append('C');
                        break;

                    case 'ö':
                        returnValue.Append('o');
                        break;

                    case 'Ö':
                        returnValue.Append('O');
                        break;
                }
            }
            return returnValue.ToString().ToUpperInvariant();
        }
    }
}
