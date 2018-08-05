using GameNet.Messaging;

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
            // TODO: UdpPortMessage in ServerUdpPortMessage und ClientUdpPortMessage aufteilen und Port durch Interface IUdpPortMessage bereitstellen.

            System.Console.WriteLine("waaas");

            if (client != null) {
                client.RegisterServerUdpPort(message);
            } else if (server != null) {
                server.RegisterClientUdpPort(message);
            }
        }
    }
}