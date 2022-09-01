
namespace FlowDMApi.Common.Log4Net
{
   public class SqlLogModel
    {
        public string Sql { get; set; }
        public object Params { get; set; }
        public bool Cache { get; set; }
    }
}
