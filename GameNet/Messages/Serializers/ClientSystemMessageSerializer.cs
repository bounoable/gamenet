namespace GameNet.Messages.Serializers
{
    public class ClientSystemMessageSerializer: ObjectSerializer<ClientSystemMessage>
    {
        override public byte[] GetBytes(ClientSystemMessage message)
            => Build()
                .Append(message.AckToken)
                .Enum<ClientSystemMessage.MessageType>(message.Type)
                .String(message.Secret)
                .Data;

        override public ClientSystemMessage GetObject(byte[] data)
        {
            byte[] ackToken = PullBytes(ref data, AcknowledgeMessage.AckTokenSize);
            ClientSystemMessage.MessageType type = PullEnum<ClientSystemMessage.MessageType>(ref data);
            string secret = PullString(ref data);

            return new ClientSystemMessage(type, ackToken, secret);
        }
    }
}