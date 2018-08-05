using GameNet.Protocol;

namespace GameNet.Messages.Handlers
{
    public class UdpPortMessageHandler: MessageHandler<UdpPortMessage>
    {
        Server server;
        Client client;

        public UdpPortMessageHandler(Server server)
        {
            this.server = server;
        }

        public UdpPortMessageHandler(Client client)
        {
            this.client = client;
        }

        override protected void HandleMessage(UdpPortMessage message)
        {
            if (client != null) {
                client.RegisterServerUdpPort(message);
            } else if (server != null) {
                server.RegisterClientUdpPort(message);
            }
        }
    }
}