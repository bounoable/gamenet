namespace GameNet.Protocol
{
    public interface IPacket
    {
        /// <summary>
        /// The message type.
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// The packet payload.
        /// </summary>
        byte[] Payload { get; }
    }
}