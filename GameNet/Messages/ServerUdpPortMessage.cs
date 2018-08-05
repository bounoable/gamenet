using GameNet.Messages.Handlers;

namespace GameNet.Messages
{
    public class ServerUdpPortMessage: UdpPortMessage
    {
        public ServerUdpPortMessage(Server server, string secret): base(new UdpPortMessageHandler(server), secret, server.NetworkConfig.LocalUdpPort)
        {}
    }
}