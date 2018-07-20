namespace GameNet.Messaging
{
    abstract public class ClientMessageHandler<TMessage>: MessageHandler<TMessage>
    {
        public ClientMessageHandler(): base(RecipientType.Server)
        {}
    }
}