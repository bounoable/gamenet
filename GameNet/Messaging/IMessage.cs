namespace GameNet.Messaging
{
    public interface IMessage
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int Type { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        byte[] Data { get; }
    }
}