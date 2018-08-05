namespace GameNet.Messages
{
    abstract public class ServerMessageHandler<T>: MessageHandler<T>
    {
        public ServerMessageHandler(): base(RecipientType.Client)
        {}
    }
}