using GameNet.Messaging;
using GameNet.Messages.Handlers;
using GameNet.Messages.Serializers;

namespace GameNet.Messages
{
    public class ServerUdpPortMessage: UdpPortMessage
    {
        public const int TypeId = 1;
        
        public ServerUdpPortMessage(): base(new ServerUdpPortMessageSerializer())
        {}

        public ServerUdpPortMessage(Client client): base(new ServerUdpPortMessageSerializer(), new UdpPortMessageHandler(client))
        {}

        public ServerUdpPortMessage(string secret, ushort port): base(secret, port)
        {}

        public static ServerUdpPortMessage TypeForClient(Client client) => new ServerUdpPortMessage(client);
        public static ServerUdpPortMessage TypeForServer() => new ServerUdpPortMessage();
    }
}