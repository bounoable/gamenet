using System;

namespace GameNet.Messaging
{
    public class MessageType
    {
        /// <summary>
        /// The type of the objects of the messages.
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        /// The message handler.
        /// </summary>
        public IMessageHandler Handler { get; }

        /// <summary>
        /// Define a message type for the message parser.
        /// </summary>
        /// <param name="typeId">The type id.</param>
        /// <param name="handler">The message handler.</param>
        public MessageType(Type objectType, IMessageHandler handler)
        {
            ObjectType = objectType;
            Handler = handler;
        }
    }
}