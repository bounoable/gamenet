namespace GameNet.Messages.Serializers
{
    public class AcknowledgeMessageSerializer: ObjectSerializer<AcknowledgeMessage>
    {
        override public byte[] GetBytes(AcknowledgeMessage message) => message.AckToken;
        override public AcknowledgeMessage GetObject(byte[] ackToken) => new AcknowledgeMessage(ackToken);
    }
}