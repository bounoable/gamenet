using System.Threading.Tasks;

namespace GameNet.Messages.Handlers
{
    public class ClientSecretMessageHandler: MessageHandler<ClientSecretMessage>
    {
        Client _client;

        public ClientSecretMessageHandler(Client client)
        {
            _client = client;
        }

        override protected void HandleMessage(ClientSecretMessage message)
        {
            _client.Secret = message.Secret;
            Task.Run(() => _client.SendHeartbeatMessages());
        }
    }
}