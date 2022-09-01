using System.Collections.Generic;

namespace FlowDMApi.Core.Extentions.Paged
{
    internal interface IPagedList<out T>
    {
        IEnumerable<T> Items { get; }
        int PageIndex { set; }
        int PageSize { set; }
        int TotalPage { set; }
        int TotalRecord { set; }
    }
}