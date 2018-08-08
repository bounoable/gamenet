namespace GameNet.Messages.Serializers
{
    public class ServerSystemMessageSerializer: ObjectSerializer<ServerSystemMessage>
    {
        override public byte[] GetBytes(ServerSystemMessage message)
            => Build()
                .Enum<ServerSystemMessage.MessageType>(message.Type)
                .String(message.AckToken)
                .Data;

        override public ServerSystemMessage GetObject(byte[] data)
            => new ServerSystemMessage(
                PullEnum<ServerSystemMessage.MessageType>(ref data),
                PullString(ref data)
            );
    }
}