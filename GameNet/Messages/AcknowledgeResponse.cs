namespace GameNet.Messages
{
    public class AcknowledgeResponse
    {
        public byte[] AckToken { get; }

        public AcknowledgeResponse(byte[] token)
        {
            AckToken = token;
        }
    }
}