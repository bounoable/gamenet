namespace GameNet.Messages
{
    public class ServerSystemMessage: SystemMessage<ServerSystemMessage.MessageType>
    {
        public enum MessageType
        {
            ConnectionEstablished
        }

        public ServerSystemMessage(MessageType type): base(type)
        {}

        public ServerSystemMessage(MessageType type, string ackToken): base(type, ackToken)
        {}
    }
}