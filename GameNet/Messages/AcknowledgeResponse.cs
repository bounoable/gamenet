namespace GameNet.Messages
{
    public class AcknowledgeResponse: IAcknowledgeResponse
    {
        public string AckToken { get; }

        public AcknowledgeResponse(string token)
        {
            AckToken = token;
        }
    }
}