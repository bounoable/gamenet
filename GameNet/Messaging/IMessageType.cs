using System;

namespace GameNet.Messaging
{
    public interface IMessageType
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int TypeId { get; set; }

        /// <summary>
        /// The object type.
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// The handler for the message type.
        /// </summary>
        IMessageHandler Handler { get; }

        /// <summary>
        /// The serializer for the object type.
        /// </summary>
        IObjectSerializer Serializer { get; }
    }
}