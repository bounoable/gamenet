using System;

namespace GameNet.Messaging
{
    public class MessageType<T>: IMessageType<T>
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The message handler.
        /// </summary>
        public IMessageHandler<T> Handler { get; }

        /// <summary>
        /// The serializer for the object type.
        /// </summary>
        public IObjectSerializer<T> Serializer { get; }

        /// <summary>
        /// Define a message type for the message parser.
        /// </summary>
        /// <paran name="typeId">The message type id.</param>
        /// <param name="handler">The object handler.</param>
        /// <param name="serializer">The object serializer.</param>
        public MessageType(int typeId, IMessageHandler<T> handler, IObjectSerializer<T> serializer)
        {
            TypeId = typeId;
            Handler = handler;
            Serializer = serializer;
        }
    }
}