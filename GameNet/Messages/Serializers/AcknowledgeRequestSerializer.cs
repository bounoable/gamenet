namespace GameNet.Messages.Serializers
{
    public class AcknowledgeRequestSerializer: ObjectSerializer<AcknowledgeRequest>
    {
        override public byte[] GetBytes(AcknowledgeRequest message)
            => Build().String(message.AckToken).Data;
        
        override public AcknowledgeRequest GetObject(byte[] data)
            => new AcknowledgeRequest(PullString(ref data));
    }
}