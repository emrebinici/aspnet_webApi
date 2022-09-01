

namespace FlowDMApi.Repository.RepositoryContext
{
    internal interface IDbContext
    {
        void Commit();
        void Rollback();
    }
}
