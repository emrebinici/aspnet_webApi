using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowDMApi.Core.ResponseBase
{
    [DataContract]
    public class GeneralListResponse<T> : ResponseBase
    {
        [DataMember]
        public List<T> Data { get; set; }
        public GeneralListResponse()
        {
            Success = true;
            MessageList = new List<string>();
        }
        public GeneralListResponse(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }
    }
}
