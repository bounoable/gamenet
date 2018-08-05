namespace GameNet.Messages
{
    abstract public class ClientMessageHandler<T>: MessageHandler<T>
    {
        public ClientMessageHandler(): base(RecipientType.Server)
        {}
    }
}