using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using FlowDMApi.Common.Command.Context;

namespace FlowDMApi.Common.ConnectionFactory
{
    /// <summary>
    /// </summary>
    public static class ConnectionContext
    {

        private static int MaxRetries { get; set; }
        internal static IDbConnection GetDbConnection()
        {
            // sqlserver
            return GetSqlServerDbConnection();
            // Posgresql
            //return GetPostgresqlDbConnetion(type);
        }

        private static SqlConnection GetSqlServerDbConnection()
        {
            var connectionString = ConnectionString.GetSqlServer();
            var maxRetries = 5;
            var delay = 100; // milisaniye Bekleme
            var conn = new SqlConnection { ConnectionString = connectionString };
            for (var i = 0; i <= maxRetries; i++)
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    break;
                }
                catch (Exception e)
                {
                    if (CanRetry(i))
                    {
                        //Debug.Yaz(
                        //    string.Format("{0} istisnasıyla yöntem başarısız oldu. {1} milisaniye uyuyor ve tekrar deniyor. Bağlantı girişimi # {2}.", e.GetType().Namespace, delay, i + 1));
                        if (delay > 0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(delay));
                        }
                    }
                    else
                    {
                        throw new Exception("Veritabanına bağlanırken bir hata oluştu. Ayrıntılar için innerException'a bakın.", e);
                    }
                }
            }
            return conn;
        }
        //private static NpgsqlConnection GetPostgresqlDbConnetion(DbTypes dbType)
        //{
        //    var connectionString = ConnectionString.GetPostgresql(dbType);
        //    var maxRetries = 5;
        //    var delay = 100; // milisaniye Bekleme
        //    var conn = new NpgsqlConnection { ConnectionString = connectionString };
        //    for (var i = 0; i <= maxRetries; i++)
        //    {
        //        try
        //        {
        //            if (conn.State != ConnectionState.Open)
        //            {
        //                conn.Open();
        //            }
        //            break;
        //        }
        //        catch (Exception e)
        //        {
        //            if (CanRetry(i))
        //            {
        //                //Debug.Yaz(
        //                //    string.Format("{0} istisnasıyla yöntem başarısız oldu. {1} milisaniye uyuyor ve tekrar deniyor. Bağlantı girişimi # {2}.", e.GetType().Namespace, delay, i + 1));
        //                if (delay > 0)
        //                {
        //                    Thread.Sleep(TimeSpan.FromMilliseconds(delay));
        //                }
        //            }
        //        }
        //    }
        //    return conn;
        //}
        private static bool CanRetry(int attempt)
        {
            return attempt < MaxRetries;
        }
    }
}
