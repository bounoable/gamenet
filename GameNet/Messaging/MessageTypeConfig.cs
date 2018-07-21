using System;
using System.Linq;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public enum TypeIdConflict
    {
        Override,
        KeepOld,
        GenerateNew
    }
    
    public class MessageTypeConfig
    {
        int nextTypeId = 0;
        Dictionary<Type, object> messageTypes = new Dictionary<Type, object>();

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="conflict">The type id conflict option.</param>
        public void RegisterMessageType<T>(IMessageType<T> type, TypeIdConflict conflict = TypeIdConflict.Override)
        {
            if (messageTypes.ContainsKey(typeof(T))) {
                if (conflict == TypeIdConflict.Override) {
                    messageTypes[typeof(T)] = type;
                    return;
                }

                if (conflict == TypeIdConflict.KeepOld) {
                    return;
                }

                if (conflict == TypeIdConflict.GenerateNew) {
                    type.TypeId = GetNextTypeId();
                }
            }

            messageTypes[typeof(T)] = type;
        }

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="handler">The object handler.</param>
        /// <param name="conflict">The type id conflict option.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType<T>(IObjectSerializer<T> serializer, IMessageHandler<T> handler, TypeIdConflict conflict = TypeIdConflict.Override)
        {
            int typeId = GetNextTypeId();

            RegisterMessageType(new MessageType<T>(typeId, handler, serializer));

            return typeId;
        }
        
        /// <summary>
        /// Get the next unused type id for the message type registration.
        /// </summary>
        /// <returns>The type id.</returns>
        int GetNextTypeId()
        {
            int id = nextTypeId;
            nextTypeId++;

            return id;
        }

        /// <summary>
        /// Get a message type by it's object type.
        /// </summary>
        /// <param name="objType">The object type.</param>
        /// <returns>The message type.</returns>
        public IMessageType<dynamic> GetType(Type objType)
        {
            if (messageTypes.TryGetValue(objType, out object type)) {
                return (MessageType<dynamic>)type;
            }

            return null;
        }

        /// <summary>
        /// Get a message type by it's object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The message type.</returns>
        public IMessageType<T> GetType<T>() => (IMessageType<T>)GetType(typeof(T));
        
        /// <summary>
        /// Get a message type by it's id.
        /// </summary>
        /// <param name="id">The type id.</param>
        /// <returns>The message type.</returns>
        public IMessageType<dynamic> GetTypeById(int id)
        {
            foreach (KeyValuePair<Type, object> entry in messageTypes) {
                IMessageType<object> type = (IMessageType<object>)entry.Value;

                if (type.TypeId == id) {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the message type of an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public IMessageType<dynamic> GetTypeByObject(object obj)
        {
            Type objType = obj.GetType();

            if (messageTypes.TryGetValue(objType, out object type)) {
                return (IMessageType<dynamic>)type;
            }

            return null;
        }
    }
}