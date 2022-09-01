using System.Collections.Generic;

namespace FlowDMApi.Core.Extentions.Where
{
    public class WhereExtentionReturn
    {
        public string Coundation { get; set; }
        public IDictionary<string,object> Parameter { get; set; }
    }
}
