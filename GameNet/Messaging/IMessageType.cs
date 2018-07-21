using System;

namespace GameNet.Messaging
{
    public interface IMessageType<T>
    {
        /// <summary>
        /// The message type id.
        /// </summary>
        int TypeId { get; set; }

        /// <summary>
        /// The handler for the message type.
        /// </summary>
        IMessageHandler<T> Handler { get; }

        /// <summary>
        /// The serializer for the object type.
        /// </summary>
        IObjectSerializer<T> Serializer { get; }
    }
}