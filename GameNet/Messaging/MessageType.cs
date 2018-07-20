using System;

namespace GameNet.Messaging
{
    public class MessageType: IMessageType
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The object type.
        /// </summary>
        public Type ObjectType { get; }

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
        /// <paran name="typeId">The message type id.</param>
        /// <paran name="objectType">The object type.</param>
        /// <param name="handler">The message handler.</param>
        /// <param name="serializer">The object serializer.</param>
        public MessageType(int typeId, Type objectType, IMessageHandler handler, IObjectSerializer serializer)
        {
            TypeId = typeId;
            ObjectType = objectType;
            Handler = handler;
            Serializer = serializer;
        }
    }
}