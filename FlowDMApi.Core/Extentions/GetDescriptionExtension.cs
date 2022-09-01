using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FlowDMApi.Core.Extentions
{
    public class GetDescriptionExtension
    {
        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Description;
        }
    }
}
