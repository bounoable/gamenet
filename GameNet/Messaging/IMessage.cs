namespace GameNet.Messaging
{
    public interface IMessage
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int TypeId { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        byte[] Data { get; }
    }
}