using System.Collections.Generic;

namespace FlowDMApi.Core.Extentions
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalPage { get; set; }
        public int TotalRecord { get; set; }
    }


}
