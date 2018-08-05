using GameNet.Protocol;
using GameNet.Messages.Handlers;
using GameNet.Messages.Serializers;

namespace GameNet.Messages
{
    public class UdpPortMessage: IMessageType
    {
        public string Secret { get; }
        public ushort Port { get; }
        public IObjectSerializer Serializer { get; protected set; }
        public IMessageHandler Handler { get; protected set; }

        public UdpPortMessage(IObjectSerializer serializer)
        {
            Serializer = serializer;
        }

        public UdpPortMessage(IObjectSerializer serializer, IMessageHandler handler)
        {
            Serializer = serializer;
            Handler = handler;
        }

        public UdpPortMessage(string secret, ushort port)
        {
            Secret = secret;
            Port = port;
        }
    }
}