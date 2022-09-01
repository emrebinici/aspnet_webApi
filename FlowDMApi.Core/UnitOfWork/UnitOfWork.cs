using System.Data;

namespace FlowDMApi.Core.UnitOfWork
{
 
    public class UnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction { get; set; }
        private IDbConnection _Connection { get; }


        public UnitOfWork(IDbConnection connection)
        {
            _Connection = connection;

        }
        public IDbTransaction Transaction => _transaction ?? (_transaction = _Connection.BeginTransaction(IsolationLevel.ReadCommitted));

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
                _Connection.Close();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _Connection?.Dispose();
                _transaction = null;

            }
        }

        public void Rollback()
        {
            try
            {
                _transaction?.Rollback();
                _Connection?.Dispose();
            }
            finally
            {
                _transaction?.Dispose();
                _Connection?.Dispose();
                _transaction = null;
            }
        }
    }
}