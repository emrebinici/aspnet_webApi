using FlowDMApi.Common.DapperSessionFactoring;

namespace FlowDMApi.Core.DapperSessionFactoring
{
    interface ISession
    {
        ISQLQuery Session { get; }
    }
}