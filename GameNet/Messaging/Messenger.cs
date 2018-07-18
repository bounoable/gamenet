using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class Messenger
    {
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
        async public Task<byte[]> Send(Stream recipient, IMessage message)
            => await Send(recipient, PrepareMessage(message));

        /// <summary>
        /// Transform a message to a byte array that contains
        /// the message type and the data of the message.
        /// </summary>
        /// <param name="message">The message to transform.</param>
        /// <returns>The prepared message as a byte array.</returns>
        byte[] PrepareMessage(IMessage message)
        {
            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(message.Type));
            data.AddRange(message.Data);

            return data.ToArray();
        }
    }
}