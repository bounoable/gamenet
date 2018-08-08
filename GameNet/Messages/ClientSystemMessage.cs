namespace GameNet.Messages
{
    public class ClientSystemMessage: SystemMessage<ClientSystemMessage.MessageType>
    {
        public enum MessageType
        {
            Heartbeat
        }

        public string Secret { get; }

        public ClientSystemMessage(MessageType type, string secret): base(type)
        {
            Secret = secret;
        }

        public ClientSystemMessage(MessageType type, byte[] ackToken, string secret): base(type, ackToken)
        {
            Secret = secret;
        }
    }
}