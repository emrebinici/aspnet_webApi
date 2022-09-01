namespace FlowDMApi.Core.Extentions.Where
{
    class LikeNode : Node
    {
        public LikeMethod Method { get; set; }
        public MemberNode MemberNode { get; set; }
        public string Value { get; set; }
    }
}
