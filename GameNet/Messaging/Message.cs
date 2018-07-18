namespace GameNet.Messaging
{
    public class Message: IMessage
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        /// <value></value>
        public int TypeId { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        public byte[] Data { get; }

        public Message(int typeId, byte[] data)
        {
            TypeId = typeId;
            Data = data;
        }
    }
}