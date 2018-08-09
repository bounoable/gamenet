using System;
using System.IO;
using System.Net;
using System.Linq;
using GameNet.Support;
using GameNet.Messages;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GameNet.Protocol
{
    public class Messenger: IDataHandler
    {
        bool ShouldRequestPendingAckResponses { get; set; }

        public MessageTypeConfig TypeConfig { get; }

        readonly ConcurrentDictionary<string, PendingAcknowledgeRequest> _pendingAcknowledgeRequests = new ConcurrentDictionary<string, PendingAcknowledgeRequest>();

        /// <summary>
        /// Initialize a messenger.
        /// </summary>
        /// <param name="typeConfig">The message type config.</param>
        public Messenger(MessageTypeConfig typeConfig)
        {
            if (typeConfig == null) {
                throw new ArgumentNullException("typeConfig");
            }

            TypeConfig = typeConfig;
        }

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="recipient">The sender of the data.</param>
        public void Handle(byte[] data, IRecipient sender)
        {
            // Check if the data contains at least the message type id.
            if (data.Length < sizeof(int))
                return;
            
            IPacket packet = ParsePacket(data);
            IMessageType type = TypeConfig.GetTypeById(packet.MessageTypeId);

            if (type == null || type.Serializer == null)
                return;

            object obj = type.Serializer.Deserialize(data.Skip(sizeof(int)).ToArray());

            if (obj is IAcknowledgeRequest) {
                var message = (IAcknowledgeRequest)obj;

                Send(sender, new AcknowledgeResponse(message.AckToken));
            }

            if (obj is IAcknowledgeResponse) {
                var response = (IAcknowledgeResponse)obj;

                _pendingAcknowledgeRequests.TryRemove(response.AckToken, out PendingAcknowledgeRequest removed);
            }

            type.Handler?.Handle(obj);
        }

        /// <summary>
        /// Parse the message from received data.
        /// </summary>
        /// <param name="raw">The received data.</param>
        /// <returns>The parsed message.</returns>
        IPacket ParsePacket(byte[] raw)
        {
            int type = DataHelper.GetInt(raw);
            byte[] data = raw.Skip(sizeof(int)).ToArray();

            return new Packet(type, data);
        }

        /// <summary>
        /// Send data to a recipient and return the sent data.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> SendBytes(IRecipient recipient, byte[] data)
        {
            await recipient.Send(data);

            return data;
        }

        /// <summary>
        /// Write data to a stream and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP/UDP client.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendBytes(Stream recipient, byte[] data)
            => SendBytes(new StreamRecipient(recipient), data);

        /// <summary>
        /// Send data to an endpoint over a UDP client and return the sent data.
        /// </summary>
        /// <param name="client">The udp client.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendBytes(UdpClient client, IPEndPoint recipient, byte[] data)
            => SendBytes(new UdpRecipient(client, recipient), data);

        /// <summary>
        /// Send a packet to a recipient and return the sent data.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendPacket(IRecipient recipient, IPacket packet)
            => SendBytes(recipient, PreparePacket(packet));

        /// <summary>
        /// Send a packet to a stream and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client's stream.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendPacket(Stream recipient, IPacket packet)
            => SendBytes(recipient, PreparePacket(packet));
        
        /// <summary>
        /// Send a packet to an endpoint over a UDP client and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually a TCP client's stream.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> SendPacket(UdpClient client, IPEndPoint recipient, IPacket packet)
            => SendBytes(client, recipient, PreparePacket(packet));

        /// <summary>
        /// Send a packet to a recipient and return the sent data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client's stream.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> Send<T>(IRecipient recipient, T obj)
        {
            if (obj is IEnumerable<byte>) {
                return await SendBytes(recipient, ((IEnumerable<byte>)obj).ToArray());
            }

            if (obj is IPacket) {
                return await SendPacket(recipient, (IPacket)obj);
            }

            if (obj is IAcknowledgeRequest) {
                var request = (IAcknowledgeRequest)obj;

                _pendingAcknowledgeRequests.TryAdd(request.AckToken, new PendingAcknowledgeRequest(request, recipient));
            }

            IPacket packet = CreatePacketFromObject(obj);

            if (packet == null) {
                return new byte[0];
            }

            return await SendPacket(recipient, packet);
        }
        
        /// <summary>
        /// Send a packet to a stream and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client's stream.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send<T>(Stream recipient, T obj)
            => Send<T>(new StreamRecipient(recipient), obj);

        /// <summary>
        /// Send a packet to an endpoint over a UDP client and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="client">The UDP client.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="obj">The object to send.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The sent bytes.</returns>
        public Task<byte[]> Send<T>(UdpClient client, IPEndPoint recipient, T obj)
            => Send<T>(new UdpRecipient(client, recipient), obj);

        /// <summary>
        /// Create a packet from an object.
        /// </summary>
        /// <param name="obj">The object to send.</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The created packet.</returns>
        IPacket CreatePacketFromObject<T>(T obj)
        {
            IMessageType type = TypeConfig.GetType<T>();

            if (type == null || type.Serializer == null) {
                return null;
            }

            byte[] data = type.Serializer.Serialize(obj);

            if (data == null) {
                return null;
            }

            int typeId = TypeConfig.GetTypeId(type);

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

        /// <summary>
        /// Request acknowledge responses for the acknowledge
        /// messages that have not been responded to yet.
        /// </summary>
        async public Task RequestPendingAcknowledgeResponses()
        {
            ShouldRequestPendingAckResponses = true;

            while (ShouldRequestPendingAckResponses) {
                await Task.Delay(100);

                if (_pendingAcknowledgeRequests.Count == 0)
                    continue;
                
                var removePending = new HashSet<PendingAcknowledgeRequest>();

                DateTime now = DateTime.Now;
                
                foreach (PendingAcknowledgeRequest pending in _pendingAcknowledgeRequests.Values) {
                    if (pending.LastTry.AddMilliseconds(pending.Message.Timeout) >= now)
                        continue;
                    
                    Send(pending.Recipient, pending.Message);
                    
                    pending.Tries++;
                    pending.LastTry = now;

                    if (pending.Tries >= pending.Message.Retries) {
                        removePending.Add(pending);
                    }
                }

                if (removePending.Count > 0) {
                    foreach (PendingAcknowledgeRequest remove in removePending) {
                        _pendingAcknowledgeRequests.TryRemove(remove.Message.AckToken, out PendingAcknowledgeRequest removed);
                    }
                }
            }
        }


        /// <summary>
        /// Stop the resending of pending acknowledge messages.
        /// </summary>
        public void StopRequestPendingAcknowlegeResponses()
            => ShouldRequestPendingAckResponses = false;
    }
}