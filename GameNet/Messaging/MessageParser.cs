using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class MessageParser: IDataHandler
    {
        MessageTypeConfig typeConfig;
        RecipientType recipient;

        /// <summary>
        /// Initialize a message parser.
        /// </summary>
        /// <param name="typeConfig">The message type config.</param>
        /// <param name="recipient">The recipient type.</param>
        public MessageParser(MessageTypeConfig typeConfig, RecipientType recipient)
        {
            this.typeConfig = typeConfig;
            this.recipient = recipient;
        }

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        public void Handle(byte[] data)
        {
            IMessage message = ParseMessage(data);
            IMessageType type = typeConfig.GetTypeById(message.TypeId);

            if (type == null) {
                return;
            }

            type.Handler.Handle(message, recipient);
        }

        /// <summary>
        /// Parse the message from received data.
        /// </summary>
        /// <param name="raw">The received data.</param>
        /// <returns>The parsed message.</returns>
        IMessage ParseMessage(byte[] raw)
        {
            int type = BitConverter.ToInt32(raw, 0);
            byte[] data = raw.Skip(4).Take(raw.Length - 4).ToArray();

            return new Message(type, data);
        }
    }
}