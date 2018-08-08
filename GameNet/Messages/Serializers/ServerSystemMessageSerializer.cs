namespace GameNet.Messages.Serializers
{
    public class ServerSystemMessageSerializer: ObjectSerializer<ServerSystemMessage>
    {
        override public byte[] GetBytes(ServerSystemMessage message)
            => Build()
                .Bytes(message.AckToken)
                .Enum<ServerSystemMessage.MessageType>(message.Type)
                .Data;

        override public ServerSystemMessage GetObject(byte[] data)
        {
            byte[] ackToken = PullBytes(ref data, 8);
            ServerSystemMessage.MessageType type = PullEnum<ServerSystemMessage.MessageType>(ref data);

            return new ServerSystemMessage(type, ackToken);
        }
    }
}