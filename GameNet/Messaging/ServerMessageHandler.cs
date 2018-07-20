namespace GameNet.Messaging
{
    abstract public class ServerMessageHandler<TMessage>: MessageHandler<TMessage>
    {
        public ServerMessageHandler(): base(RecipientType.Client)
        {}
    }
}