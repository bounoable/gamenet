using System;
using System.Linq;
using GameNet.Support;

namespace GameNet.Protocol
{
    public class Packet: IPacket
    {
        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// The payload.
        /// </summary>
        public byte[] Payload { get; }

        public Packet(string type, byte[] payload)
        {
            MessageType = type;
            Payload = payload;
        }
    }
}