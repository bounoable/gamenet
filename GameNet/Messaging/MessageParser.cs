using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class MessageParser: IDataHandler
    {
        RecipientType recipient;
        Dictionary<int, IMessageHandler> handlers = new Dictionary<int, IMessageHandler>();

        /// <summary>
        /// Initialize a message parser.
        /// </summary>
        /// <param name="recipient">The recipient type.</param>
        public MessageParser(RecipientType recipient)
        {
            this.recipient = recipient;
        }

        /// <summary>
        /// Register a message handler.
        /// </summary>
        /// <param name="type">The message type id.</param>
        /// <param name="handler">The message handler.</param>
        public void RegisterHandler(int type, IMessageHandler handler) => handlers[type] = handler;

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        public void Handle(byte[] data)
        {
            IMessage message = ParseMessage(data);

            if (handlers.TryGetValue(message.Type, out IMessageHandler handler)) {
                handler.Handle(message, recipient);
            }
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