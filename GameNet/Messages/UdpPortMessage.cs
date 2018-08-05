using GameNet.Messaging;
using GameNet.Messages.Handlers;
using GameNet.Messages.Serializers;

namespace GameNet.Messages
{
    public class UdpPortMessage: IMessageType
    {
        public string Secret { get; }
        public ushort Port { get; }
        public IObjectSerializer Serializer { get; } = new UdpPortMessageSerializer();
        public IMessageHandler Handler { get; }

        public UdpPortMessage()
        {}

        public UdpPortMessage(string secret, ushort port)
        {
            Secret = secret;
            Port = port;
        }

        public UdpPortMessage(IMessageHandler handler, string secret, ushort port)
        {
            Handler = handler;
            Secret = secret;
            Port = port;
        }
    }
}