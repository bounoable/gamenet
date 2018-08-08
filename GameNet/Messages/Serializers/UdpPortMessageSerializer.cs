using System;
using GameNet.Protocol;

namespace GameNet.Messages.Serializers
{
    public class UdpPortMessageSerializer<TSender>: ObjectSerializer<UdpPortMessage<TSender>>
    {
        override public byte[] GetBytes(UdpPortMessage<TSender> message)
            => Build().UShort(message.Port).String(message.Secret).Data;
        
        override public UdpPortMessage<TSender> GetObject(byte[] data)
            => new UdpPortMessage<TSender>(PullUShort(ref data), PullString(ref data));
    }
}