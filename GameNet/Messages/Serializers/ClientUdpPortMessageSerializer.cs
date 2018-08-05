using GameNet.Messaging;

namespace GameNet.Messages.Serializers
{
    public class ClientUdpPortMessageSerializer: ObjectSerializer<ClientUdpPortMessage>
    {
        override public byte[] GetBytes(ClientUdpPortMessage message)
            => Build().String(message.Secret).UShort(message.Port).Data;
        
        override public ClientUdpPortMessage GetObject(byte[] data)
            => new ClientUdpPortMessage(PullString(ref data), PullUShort(ref data));
    }
}