namespace GameNet.Messages.Serializers
{
    public class ClientSystemMessageSerializer: ObjectSerializer<ClientSystemMessage>
    {
        override public byte[] GetBytes(ClientSystemMessage message)
            => Build()
                .Enum<ClientSystemMessage.MessageType>(message.Type)
                .String(message.AckToken)
                .String(message.Secret)
                .Data;

        override public ClientSystemMessage GetObject(byte[] data)
            => new ClientSystemMessage(
                PullEnum<ClientSystemMessage.MessageType>(ref data),
                PullString(ref data),
                PullString(ref data)
            );
    }
}