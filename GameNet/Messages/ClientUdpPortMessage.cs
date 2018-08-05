using GameNet.Messaging;
using GameNet.Messages.Handlers;
using GameNet.Messages.Serializers;

namespace GameNet.Messages
{
    public class ClientUdpPortMessage: UdpPortMessage
    {
        public const int TypeId = 0;
        
        public ClientUdpPortMessage(): base(new ClientUdpPortMessageSerializer())
        {}
        
        public ClientUdpPortMessage(Server server): base(new ClientUdpPortMessageSerializer(), new UdpPortMessageHandler(server))
        {}

        public ClientUdpPortMessage(string secret, ushort port): base(secret, port)
        {}

        public static ClientUdpPortMessage TypeForServer(Server server) => new ClientUdpPortMessage(server);
        public static ClientUdpPortMessage TypeForClient() => new ClientUdpPortMessage();
    }
}