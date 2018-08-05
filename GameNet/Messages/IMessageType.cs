using System;

namespace GameNet.Messages
{
    public interface IMessageType
    {
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