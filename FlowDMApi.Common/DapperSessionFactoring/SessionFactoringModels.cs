using Dapper;
using FlowDMApi.Core.Extentions.Cache;

namespace FlowDMApi.Common.DapperSessionFactoring
{
    public class SessionFactoringModels
    {
        public SessionFactoringModels()
        {
            Parameters = new DynamicParameters();
        }
        public string Query { get; set; }
        public int? TimeOut { get; set; }
        public int CacheTime { get; set; }
        public bool DynamicCache { get; set; }
        public DynamicParameters Parameters { get; set; }
        public string ProcessName { get; set; }
        public bool SqlWatch { get; set; }
        public CacheTimeEnum? CacheTimeEnum { get; set; }
    }
}