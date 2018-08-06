using System;
using System.IO;
using System.Net;
using System.Linq;
using GameNet.Messages;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Protocol
{
    public class Messenger
    {
        MessageTypeConfig typeConfig;

        public Messenger(MessageTypeConfig typeConfig)
        {
            this.typeConfig = typeConfig;
        }

        /// <summary>
        /// Write data to a stream and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP/UDP client.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> SendBytes(Stream recipient, byte[] data)
        {
            if (recipient.CanWrite) {
                await recipient.WriteAsync(data, 0, data.Length);
            }

            return data;
        }

        /// <summary>
        /// Send data to an endpoint over a UDP client and return the sent data.
        /// </summary>
        /// <param name="client">The udp client.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> SendBytes(UdpClient client, IPEndPoint recipient, byte[] data)
        {
            await client.SendAsync(data, data.Length, recipient);

            return data;
        }

        /// <summary>
        /// Send a packet to a stream and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendPacket(Stream recipient, IPacket packet)
            => SendBytes(recipient, PreparePacket(packet));
        
        /// <summary>
        /// Send a packet to an endpoint over a UDP client and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendPacket(UdpClient client, IPEndPoint recipient, IPacket packet)
            => SendBytes(client, recipient, PreparePacket(packet));
        
        /// <summary>
        /// Send a packet to a stream and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> Send<T>(Stream recipient, T obj)
        {
            if (obj is IEnumerable<byte>) {
                return await SendBytes(recipient, (byte[])(IEnumerable<byte>)obj);
            }

            if (obj is IPacket) {
                return await SendPacket(recipient, (IPacket)obj);
            }

            IPacket packet = CreatePacketFromObject(obj);

            if (packet == null) {
                return new byte[0];
            }

            return await SendPacket(recipient, packet);
        }

        /// <summary>
        /// Send a packet to an endpoint over a UDP client and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="client">The UDP client.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="obj">The object to send.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> Send<T>(UdpClient client, IPEndPoint recipient, T obj)
        {
            if (obj is IEnumerable<byte>) {
                return await SendBytes(client, recipient, (byte[])(IEnumerable<byte>)obj);
            }

            if (obj is IPacket) {
                return await SendPacket(client, recipient, (IPacket)obj);
            }

            IPacket packet = CreatePacketFromObject(obj);

            if (packet == null) {
                return null;
            }

            return await SendPacket(client, recipient, packet);
        }

        /// <summary>
        /// Create a packet from an object.
        /// </summary>
        /// <param name="obj">The object to send.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The created packet.</returns>
        IPacket CreatePacketFromObject<T>(T obj)
        {
            IMessageType type = typeConfig.GetType<T>();

            if (type == null || type.Serializer == null) {
                return null;
            }

            byte[] data = type.Serializer.Serialize(obj);

            if (data == null) {
                return null;
            }

            int typeId = typeConfig.GetTypeId(type);

            return new Packet(typeId, data);
        }

        /// <summary>
        /// Transform a packet to a byte array that contains
        /// the message type and the data of the message.
        /// </summary>
        /// <param name="packet">The packet to transform.</param>
        /// <returns>The prepared packet as a byte array.</returns>
        byte[] PreparePacket(IPacket packet)
            => ToLittleEndian(BitConverter.GetBytes(packet.MessageTypeId))
                .Concat(ToLittleEndian(packet.Payload))
                .ToArray();

        /// <summary>
        /// Transform a byte array to little endian if the operating system uses big endian.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The corrected byte array.</returns>
        byte[] ToLittleEndian(byte[] data) => BitConverter.IsLittleEndian ? data : data.Reverse().ToArray();
    }
}