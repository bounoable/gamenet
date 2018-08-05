namespace GameNet.Protocol
{
    public class Packet: IPacket
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        /// <value></value>
        public int MessageTypeId { get; }

        /// <summary>
        /// The message data.
        /// </summary>
        public byte[] Data { get; }

        public Packet(int messageTypeId, byte[] data)
        {
            MessageTypeId = messageTypeId;
            Data = data;
        }
    }
}