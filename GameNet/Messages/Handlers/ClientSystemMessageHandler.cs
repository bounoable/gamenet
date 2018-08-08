namespace GameNet.Messages.Handlers
{
    public class ClientSystemMessageHandler: MessageHandler<ClientSystemMessage>
    {
        Server _server;

        public ClientSystemMessageHandler(Server server)
        {
            _server = server;
        }

        override protected void HandleMessage(ClientSystemMessage message)
        {
            switch (message.Type) {
                case ClientSystemMessage.MessageType.Heartbeat:
                    _server.NotifyHeartbeat(message.Secret);
                    break;
            }
        }
    }
}