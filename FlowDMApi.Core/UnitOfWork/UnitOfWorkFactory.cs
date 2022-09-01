using System;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace FlowDMApi.Core.UnitOfWork
{
    public class UnitOfWorkFactory<TConnection> : IUnitOfWorkFactory where TConnection : IDbConnection, new() 
    {
        public int MaxRetries { get; set; }
        public float Delay { get; set; }
        private readonly string _connectionString;

        public UnitOfWorkFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("ConnectionString Boş Olamaz !");
            }
            _connectionString = connectionString;
        }
        public IUnitOfWork Create()
        {
            return new UnitOfWork(CreateOpenConnection());
        }
        private IDbConnection CreateOpenConnection()
        {
            MaxRetries = 5;
            Delay = 100; // milisaniye Bekleme
            var conn = new TConnection { ConnectionString = _connectionString };
            for (var i = 0; i <= MaxRetries; i++)
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
                        Debug.WriteLine("{0} istisnasıyla yöntem başarısız oldu. {1} milisaniye uyuyor ve tekrar deniyor. Bağlantı girişimi # {2}.",
                            e.GetType().Namespace, Delay, i + 1);
                        if (Delay > 0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(Delay));
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

        private bool CanRetry(int attempt)
        {
            return attempt < MaxRetries;
        }
    }
}
