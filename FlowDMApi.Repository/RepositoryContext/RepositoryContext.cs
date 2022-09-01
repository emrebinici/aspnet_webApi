using System.Data.SqlClient;
using FlowDMApi.Common.Command.Context;
using FlowDMApi.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Npgsql;

namespace FlowDMApi.Repository.RepositoryContext
{
    public sealed class RepositoryContext
    {
        public static DbContext New()
        {
            return new DbContext(new UnitOfWorkFactory<SqlConnection>(ConnectionString.GetSqlServer()));

            //return new DbContext(new UnitOfWorkFactory<MySqlConnection>(ConnnectionString.GetMysql), httpContextAccessor);

            //return new DbContext(new UnitOfWorkFactory<NpgsqlConnection>(ConnnectionString.GetPostgreSql), httpContextAccessor);
        }
    }
}
