using System.Linq;

namespace GameNet.Messages.Serializers
{
    public class SystemMessageSerializer: AcknowledgeMessageSerializer<SystemMessage>
    {
        override public byte[] GetBytes(SystemMessage message)
        {
            byte[] ackMessageBytes = base.GetBytes(message);

            return ackMessageBytes.Concat(
                Build().Enum<SystemMessage.MessageType>(message.Type).Data
            ).ToArray();
        }

        override public SystemMessage GetObject(byte[] data)
            => new SystemMessage(PullEnum<SystemMessage.MessageType>(ref data));
    }
}