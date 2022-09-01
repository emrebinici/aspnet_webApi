using System.Collections.Generic;
using System.Runtime.Serialization;
using FlowDMApi.Core.Extentions.Paged;

namespace FlowDMApi.Core.ResponseBase
{
    [DataContract]
    public class GeneralPagedListResponse<T> : ResponseBase
    {
        [DataMember]
        public PagedList<T> Data { get; set; }
        public GeneralPagedListResponse()
        {
            Success = true;
            MessageList = new List<string>();
        }
        public GeneralPagedListResponse(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }
    }
}
