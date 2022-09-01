using System.Linq.Expressions;

namespace FlowDMApi.Core.Extentions.Where
{
    class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
