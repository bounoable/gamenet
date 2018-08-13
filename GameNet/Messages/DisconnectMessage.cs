namespace GameNet.Messages
{
    public class DisconnectMessage: AcknowledgeRequest
    {
        public string Secret { get; }

        public DisconnectMessage(string secret, string ackToken = null): base(ackToken)
        {
            Secret = secret;
        }
    }
}