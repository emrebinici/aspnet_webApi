using FlowDMApi.Core.RedisFactory;
using FlowDMApi.Repository.RepositoryContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowDMApi.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public DbContext _context;
        public readonly IRedisCache Redis = new RedisCache();

        public BaseController()
        {
            _context = RepositoryContext.New();
        }
    }
}
