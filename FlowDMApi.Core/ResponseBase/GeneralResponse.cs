using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowDMApi.Core.ResponseBase
{
    [DataContract]
    public class GeneralResponse<T> : ResponseBase
    {
        [DataMember]
        public T Data { get; set; }

        public GeneralResponse()
        {
            Success = true;
            MessageList = new List<string>();
        }
        public GeneralResponse(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }

    }
    [DataContract]
    public class GeneralResponse : ResponseBase
    {
        public GeneralResponse()
        {
            Success = true;
            MessageList = new List<string>();
        }
        public GeneralResponse(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }
    }
}
