namespace GameNet.Messages.Serializers
{
    public class ClientSecretMessageSerializer: ObjectSerializer<ClientSecretMessage>
    {
        override public byte[] GetBytes(ClientSecretMessage message)
            => Build().String(message.Secret).Data;

        override public ClientSecretMessage GetObject(byte[] data)
            => new ClientSecretMessage(PullString(ref data));
    }
}