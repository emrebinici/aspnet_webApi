using System;

namespace FlowDMApi.Core.Helper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ChannelAttribute : Attribute
    {
        public ChannelAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        
    }
}
