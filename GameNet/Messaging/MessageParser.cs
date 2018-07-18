using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class MessageParser: IDataHandler
    {
        RecipientType recipient;
        int nextTypeId = 0;
        Dictionary<int, MessageType> messageTypes = new Dictionary<int, MessageType>();

        /// <summary>
        /// Initialize a message parser.
        /// </summary>
        /// <param name="recipient">The recipient type.</param>
        public MessageParser(RecipientType recipient)
        {
            this.recipient = recipient;
        }

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="handler">The message handler.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType(Type objectType, IMessageHandler handler)
        {
            int typeId = nextTypeId;
            nextTypeId++;

            messageTypes[typeId] = new MessageType(objectType, handler);

            return typeId;
        }

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="handler">The message handler.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType<TMessage>(IMessageHandler handler)
            => RegisterMessageType(typeof(TMessage), handler);

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        public void Handle(byte[] data)
        {
            IMessage message = ParseMessage(data);

            if (!messageTypes.TryGetValue(message.TypeId, out MessageType type)) {
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