namespace GameNet.Messages
{
    public class ServerSystemMessage: SystemMessage<ServerSystemMessage.MessageType>
    {
        public enum MessageType
        {}

        public ServerSystemMessage(MessageType type): base(type)
        {}

        public ServerSystemMessage(MessageType type, byte[] ackToken): base(type, ackToken)
        {}
    }
}