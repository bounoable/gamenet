namespace GameNet.Messages.Serializers
{
    abstract public class AcknowledgeMessageSerializer<T>: ObjectSerializer<T> where T: AcknowledgeMessage
    {
        override public byte[] GetBytes(T message) => message.AckToken;
        override abstract public T GetObject(byte[] data);
    }
}