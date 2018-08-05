using GameNet.Messages.Handlers;

namespace GameNet.Messages
{
    public class ClientUdpPortMessage: UdpPortMessage
    {
        public ClientUdpPortMessage(Client client, string secret): base(new UdpPortMessageHandler(client), secret, client.NetworkConfig.LocalUdpPort)
        {}
    }
}