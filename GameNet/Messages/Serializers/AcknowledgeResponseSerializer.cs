namespace GameNet.Messages.Serializers
{
    public class AcknowledgeResponseSerializer: ObjectSerializer<AcknowledgeResponse>
    {
        override public byte[] GetBytes(AcknowledgeResponse response)
            => Build().String(response.AckToken).Data;
        
        override public AcknowledgeResponse GetObject(byte[] data)
            => new AcknowledgeResponse(PullString(ref data));
    }
}