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
        public MessageType(IObjectSerializer serializer, IMessageHandler handler)
        {
            Handler = handler;
            Serializer = serializer;
        }
    }
}