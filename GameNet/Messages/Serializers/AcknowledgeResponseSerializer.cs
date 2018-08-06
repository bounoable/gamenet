namespace GameNet.Messages.Serializers
{
    public class AcknowledgeResponseSerializer: ObjectSerializer<AcknowledgeResponse>
    {
        override public byte[] GetBytes(AcknowledgeResponse response) => response.AckToken;
        override public AcknowledgeResponse GetObject(byte[] token) => new AcknowledgeResponse(token);
    }
}