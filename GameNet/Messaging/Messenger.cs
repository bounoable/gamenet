using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
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
        async public Task<byte[]> Send(Stream recipient, byte[] data)
        {
            if (recipient.CanWrite) {
                await recipient.WriteAsync(data, 0, data.Length);
            }

            return data;
        }

        /// <summary>
        /// Write a message to a stream and return the written data.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send(Stream recipient, IMessage message)
            => Send(recipient, PrepareMessage(message));
        
        /// <summary>
        /// Write a message to a stream and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> Send(Stream recipient, object obj)
        {
            IMessageType<object> type = typeConfig.GetTypeByObject(obj);

            if (type == null) {
                return new byte[0];
            }

            byte[] data = type.Serializer.Serialize(obj);

            if (data == null) {
                return new byte[0];
            }

            Message message = new Message(type.TypeId, data);

            return await Send(recipient, message);
        }

        /// <summary>
        /// Transform a message to a byte array that contains
        /// the message type and the data of the message.
        /// </summary>
        /// <param name="message">The message to transform.</param>
        /// <returns>The prepared message as a byte array.</returns>
        byte[] PrepareMessage(IMessage message)
        {
            List<byte> data = new List<byte>();

            data.AddRange(ToLittleEndian(BitConverter.GetBytes(message.TypeId)));
            data.AddRange(ToLittleEndian(message.Data));

            return data.ToArray();
        }

        /// <summary>
        /// Transform a byte array to little endian if the operating system uses big endian.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The corrected byte array.</returns>
        byte[] ToLittleEndian(byte[] data) => BitConverter.IsLittleEndian ? data : data.Reverse().ToArray();
    }
}