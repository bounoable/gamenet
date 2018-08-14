using GameNet.Events;

namespace GameNet.Messages.Handlers
{
    public class ServerSystemMessageHandler: MessageHandler<ServerSystemMessage>
    {
        Client _client;

        public ServerSystemMessageHandler(Client client)
        {
            _client = client;
        }

        override protected void HandleMessage(ServerSystemMessage message)
        {
            switch (message.Type) {
                case ServerSystemMessage.MessageType.ConnectionEstablished:
                    _client.NotifyConnectionEstablished();
                    break;
            }
        }
    }
}