using System;
using System.Linq;
using System.Reflection;
using FlowDMApi.Core.Attributes;

namespace FlowDMApi.Core.Extentions
{
    public static class GetEnumReponseCode
    {
        public static string GetStringCode(this Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<ResponseCodeAttribute>()
                    ?.Code
                ?? value.ToString();
        }
    }
}
