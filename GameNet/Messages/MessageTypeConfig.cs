using System;
using System.Linq;
using System.Collections.Generic;

namespace GameNet.Messages
{   
    public class MessageTypeConfig
    {
        readonly Dictionary<Type, IMessageType> messageTypes = new Dictionary<Type, IMessageType>();

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="id">The message type id.</param>
        /// <param name="objectType">The object type of the messages.</param>
        /// <param name="type">The message type.</param>
        public void RegisterMessageType(Type objectType, IMessageType type)
        {
            messageTypes[objectType] = type;
        }

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <typeparam name="T">The object type of the messages.</typeparam>
        public void RegisterMessageType<T>(IMessageType type)
            => RegisterMessageType(typeof(T), type);
        
        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="handler">The message handler.</param>
        /// <typeparam name="T">The object type of the messages.</typeparam>
        public void RegisterMessageType<T>(IObjectSerializer serializer = null, IMessageHandler handler = null)
            => RegisterMessageType<T>(new MessageType<T>(serializer, handler));

        /// <summary>
        /// Get a message type by it's object type.
        /// </summary>
        /// <param name="objType">The object type.</param>
        /// <returns>The message type.</returns>
        public IMessageType GetType(Type objType)
        {
            if (messageTypes.TryGetValue(objType, out IMessageType type)) {
                return type;
            }

            return null;
        }

        /// <summary>
        /// Get a message type by it's object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The message type.</returns>
        public IMessageType GetType<T>() => GetType(typeof(T));

        /// <summary>
        /// Get a message type by the object type name.
        /// </summary>
        /// <param name="id">The type id.</param>
        /// <returns>The message type.</returns>
        public IMessageType GetTypeById(string id)
        {
            foreach (IMessageType type in messageTypes.Values) {       
                if (type.Id == id) {
                    return type;
                }
            }

            return null;
        }
    }
}