using System;
using System.IO;
using System.Net;
using System.Linq;
using GameNet.Support;
using GameNet.Messages;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Protocol
{
    public class Messenger: IDataHandler
    {
        public bool Initialized { get; private set; } = false;

        readonly MessengerConfig _config;
        readonly MessageTypeConfig _typeConfig;
        readonly HashSet<PendingAcknowledgeMessage> _pendingAcknowledgeMessages = new HashSet<PendingAcknowledgeMessage>();

        /// <summary>
        /// Initialize a messenger.
        /// </summary>
        /// <param name="config">The messenger config.</param>
        /// <param name="typeConfig">The message type config.</param>
        public Messenger(MessengerConfig config, MessageTypeConfig typeConfig)
        {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            if (typeConfig == null) {
                throw new ArgumentNullException("typeConfig");
            }

            _config = config;
            _typeConfig = typeConfig;
        }

        /// <summary>
        /// Initialize the messenger.
        ///     - Periodically request pending acknowledge responses.
        /// </summary>
        public void Init()
        {
            Task.Run(() => RequestPendingAcknowledgeResponses()).ConfigureAwait(false);

            Initialized = true;
        }

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="recipient">The sender of the data.</param>
        public void Handle(byte[] data, IRecipient sender)
        {
            Console.WriteLine("Paket angekommen");

            // Check if the data contains at least the message type id.
            if (data.Length < sizeof(int))
                return;
            
            IPacket packet = ParsePacket(data);
            IMessageType type = _typeConfig.GetTypeById(packet.MessageTypeId);

            Console.WriteLine(type);

            if (type == null || type.Serializer == null)
                return;

            object obj = type.Serializer.Deserialize(data.Skip(sizeof(int)).ToArray());

            if (obj is IAcknowledgeMessage) {
                var message = (IAcknowledgeMessage)obj;

                Send(sender, new AcknowledgeResponse(message.AckToken));

                // TODO: AcknowledgeResponse testen...
            }

            if (obj is AcknowledgeResponse) {
                var response = (AcknowledgeResponse)obj;

                _pendingAcknowledgeMessages.RemoveWhere(pending => pending.Message.AckToken == response.AckToken);
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
                return await SendBytes(recipient, (byte[])(IEnumerable<byte>)obj);
            }

            if (obj is IPacket) {
                return await SendPacket(recipient, (IPacket)obj);
            }

            if (obj is IAcknowledgeMessage) {
                _pendingAcknowledgeMessages.Add(new PendingAcknowledgeMessage((IAcknowledgeMessage)obj, recipient));
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
            IMessageType type = _typeConfig.GetType<T>();

            if (type == null || type.Serializer == null) {
                return null;
            }

            byte[] data = type.Serializer.Serialize(obj);

            if (data == null) {
                return null;
            }

            int typeId = _typeConfig.GetTypeId(type);

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
        /// <returns></returns>
        async Task RequestPendingAcknowledgeResponses()
        {
            while (Initialized) {
                await Task.Delay(_config.AcknowledgeMessageInterval);

                PendingAcknowledgeMessage removePending = null;
                
                foreach (PendingAcknowledgeMessage pending in _pendingAcknowledgeMessages) {
                    Send(pending.Recipient, pending.Message);
                    
                    pending.Tries++;

                    if (pending.Tries >= _config.AcknowledgeMessageRetries) {
                        removePending = pending;
                    }
                }

                if (removePending != null) {
                    _pendingAcknowledgeMessages.Remove(removePending);
                }
            }
        }
    }
}