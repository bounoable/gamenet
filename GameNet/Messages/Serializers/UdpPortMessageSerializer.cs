using GameNet.Messaging;

namespace GameNet.Messages.Serializers
{
    public class UdpPortMessageSerializer: ObjectSerializer<UdpPortMessage>
    {
        override public byte[] GetBytes(UdpPortMessage message)
            => Build().String(message.Secret).UShort(message.Port).Data;
        
        override public UdpPortMessage GetObject(byte[] data)
            => new UdpPortMessage(PullString(ref data), PullUShort(ref data));
    }
}