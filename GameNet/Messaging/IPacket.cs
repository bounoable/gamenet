namespace GameNet.Messaging
{
    public interface IPacket
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int MessageTypeId { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        byte[] Data { get; }
    }
}