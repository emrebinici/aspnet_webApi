using System.Collections.Generic;

namespace FlowDMApi.Common.Command.Context
{
    public class QueryReturnModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string QueryModelCacheName { get; set; }
    }
}
