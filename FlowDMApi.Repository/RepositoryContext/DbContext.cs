using FlowDMApi.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace FlowDMApi.Repository.RepositoryContext
{
    /// <summary>
    /// </summary>
    public class DbContext : IDbContext
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IUnitOfWork _unitOfWork;
        protected IUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = _unitOfWorkFactory.Create());
        private HomeRepository _homeRepository;
        private TokenRepository _tokenRepository;

        public HomeRepository HomeRepository => _homeRepository ??= new HomeRepository(UnitOfWork);
        public TokenRepository TokenRepository => _tokenRepository ??= new TokenRepository(UnitOfWork);
        public DbContext(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public void Commit()
        {
            try
            {
                UnitOfWork.Commit();
            }
            finally
            {
                Reset();
            }
        }

        public void Rollback()
        {
            try
            {
                UnitOfWork.Rollback();
            }
            finally
            {
                Reset();
            }
        }

        void Reset()
        {
            _unitOfWork = null;
            _homeRepository = null;
        }
    }
}
