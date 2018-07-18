namespace GameNet.Messaging
{
    public class Message: IMessage
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        /// <value></value>
        public int Type { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        public byte[] Data { get; }

        public Message(int type, byte[] data)
        {
            Type = type;
            Data = data;
        }
    }
}