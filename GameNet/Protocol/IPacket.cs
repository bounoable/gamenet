namespace GameNet.Protocol
{
    public interface IPacket
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int MessageTypeId { get; }

        /// <summary>
        /// The packet payload.
        /// </summary>
        byte[] Payload { get; }
    }
}