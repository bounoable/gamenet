namespace GameNet.Messages.Serializers
{
    public class DisconnectMessageSerializer: ObjectSerializer<DisconnectMessage>
    {
        override public byte[] GetBytes(DisconnectMessage message)
            => Build().String(message.Secret).String(message.AckToken).Data;

        override public DisconnectMessage GetObject(byte[] data)
            => new DisconnectMessage(PullString(ref data), PullString(ref data));
    }
}