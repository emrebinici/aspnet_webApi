using System;

namespace FlowDMApi.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResponseCodeAttribute : Attribute
    {
        public ResponseCodeAttribute(string code)
        {
            Code = code;
        }
        public string Code { get; set; }


    }
}
