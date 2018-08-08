using GameNet.Protocol;
using GameNet.Messages.Handlers;
using System.Collections.Generic;
using GameNet.Messages.Serializers;

namespace GameNet.Messages
{
    public class UdpPortMessage<TSender>: IUdpPortMessage
    {
        public ushort Port { get; }
        public string Secret { get; }

        public UdpPortMessage(ushort port)
        {
            Port = port;
        }

        public UdpPortMessage(ushort port, string secret)
        {
            Port = port;
            Secret = secret;
        }
    }
}