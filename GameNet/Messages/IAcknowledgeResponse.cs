namespace GameNet.Messages
{
    public interface IAcknowledgeResponse
    {
        string AckToken { get; }
    }
}