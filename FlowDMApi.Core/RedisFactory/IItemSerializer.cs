namespace FlowDMApi.Core.RedisFactory
{
    public interface IItemSerializer
    {
        byte[] Serialize(object item);
        object Deserialize(byte[] itemBytes);
    }
}
