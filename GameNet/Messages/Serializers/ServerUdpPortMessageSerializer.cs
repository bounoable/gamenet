using GameNet.Protocol;

namespace GameNet.Messages.Serializers
{
    public class ServerUdpPortMessageSerializer: ObjectSerializer<ServerUdpPortMessage>
    {
        override public byte[] GetBytes(ServerUdpPortMessage message)
            => Build().String(message.Secret).UShort(message.Port).Data;
        
        override public ServerUdpPortMessage GetObject(byte[] data)
            => new ServerUdpPortMessage(PullString(ref data), PullUShort(ref data));
    }
}