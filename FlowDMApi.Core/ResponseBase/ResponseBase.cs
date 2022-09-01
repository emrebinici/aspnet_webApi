using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowDMApi.Core.ResponseBase
{
    [DataContract]
    public abstract class ResponseBase
    {
        /// <summary>
        /// Bu alan transaction ın api dönüşünde commit durumunu temsil eder
        /// </summary>
        public bool Success { get; set; }
        [DataMember]
        public string ResponseCode { get; set; }
        [DataMember]
        public List<string> MessageList { get; set; }

        public ResponseBase()
        {
            Success = true;
            MessageList = new List<string>();
        }
        public ResponseBase(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }
    }
}
