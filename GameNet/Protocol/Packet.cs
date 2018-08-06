using System;
using System.Linq;
using GameNet.Support;

namespace GameNet.Protocol
{
    public class Packet: IPacket
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        public int MessageTypeId { get; }

        /// <summary>
        /// The payload.
        /// </summary>
        public byte[] Payload { get; }

        public Packet(int typeId, byte[] payload)
        {
            MessageTypeId = typeId;
            Payload = payload;
        }
    }
}