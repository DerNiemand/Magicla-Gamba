
public interface INetworkReciever
{
    public abstract void HandleMessage(long SenderId,int messageType, string message);


}
