using System;

namespace GameNet.Messages
{
    public class MessageType<T>: IMessageType
    {
        /// <summary>
        /// The message handler.
        /// </summary>
        public IMessageHandler Handler { get; }

        /// <summary>
        /// The serializer for the object type.
        /// </summary>
        public IObjectSerializer Serializer { get; }

        /// <summary>
        /// Define a message type for the message parser.
        /// </summary>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="handler">The object handler.</param>
        public MessageType(IObjectSerializer serializer = null, IMessageHandler handler = null)
        {
            Handler = handler;
            Serializer = serializer;
        }

        /// <summary>
        /// Define a message type for the message parser.
        /// </summary>
        /// <param name="serializer">The object serializer.</param>
        public MessageType(IObjectSerializer serializer)
        {
            Serializer = serializer;
        }

        /// <summary>
        /// Define a message type for the message parser.
        /// </summary>
        /// <param name="handler">The object handler.</param>
        public MessageType(IMessageHandler handler)
        {
            Handler = handler;
        }
    }
}