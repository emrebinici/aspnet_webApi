using System;
using System.Collections.Generic;

namespace FlowDMApi.Core.Extentions.Paged
{
    [Serializable]
    public class PagedList<T> : IPagedList<T>
    {

        public PagedList(IEnumerable<T> list, int pageSize, int pageIndex, int totalRecord, int totalPage)
        {
            PageSize = Math.Abs(pageSize);
            PageIndex = pageIndex;
            Items = list;
            TotalRecord = totalRecord;
            TotalPage = totalPage;
        }
        public IEnumerable<T> Items { get; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPage { get; set; }
    }
}