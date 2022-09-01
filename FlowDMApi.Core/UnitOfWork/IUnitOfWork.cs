using System.Data;

namespace FlowDMApi.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IDbTransaction Transaction { get; }
        void Commit();
        void Rollback();

    }
}
