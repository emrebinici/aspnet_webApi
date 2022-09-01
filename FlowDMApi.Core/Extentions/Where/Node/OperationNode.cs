using System.Linq.Expressions;

namespace FlowDMApi.Core.Extentions.Where
{
    class OperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }
}
