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

        readonly Dictionary<Type, IMessageType> messageTypes = new Dictionary<Type, IMessageType>();
        readonly Dictionary<Type, int> objectTypeIds = new Dictionary<Type, int>();

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="id">The message type id.</param>
        /// <param name="objectType">The object type of the messages.</param>
        /// <param name="type">The message type.</param>
        /// <param name="conflict">The type id conflict option.</param>
        public void RegisterMessageType(int id, Type objectType, IMessageType type, TypeIdConflict conflict = TypeIdConflict.Override)
        {
            if (objectTypeIds.ContainsValue(id)) {
                if (conflict == TypeIdConflict.Override) {
                    messageTypes[objectType] = type;
                    objectTypeIds[objectType] = id;
                    UpdateNextTypeId();
                    return;
                }

                if (conflict == TypeIdConflict.KeepOld) {
                    return;
                }

                if (conflict == TypeIdConflict.GenerateNew) {
                    id = GetNextTypeId();
                }
            }

            messageTypes[objectType] = type;
            objectTypeIds[objectType] = id;
            UpdateNextTypeId();
        }

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="id">The message type id.</param>
        /// <param name="type">The message type.</param>
        /// <param name="conflict">The type id conflict option.</param>
        /// <typeparam name="T">The object type of the messages.</typeparam>
        public void RegisterMessageType<T>(int id, IMessageType type, TypeIdConflict conflict = TypeIdConflict.Override)
            => RegisterMessageType(id, typeof(T), type, conflict);
        
        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="objectType">The object type of the messages.</param>
        /// <param name="type">The message type.</param>
        public void RegisterMessageType(Type objectType, IMessageType type)
            => RegisterMessageType(GetNextTypeId(), objectType, type);

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <typeparam name="T">The object type of the messages.</typeparam>
        public void RegisterMessageType<T>(IMessageType type)
            => RegisterMessageType<T>(GetNextTypeId(), type);

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="serializer">The object serializer.</param>
        /// <param name="handler">The object handler.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType<T>(IObjectSerializer serializer, IMessageHandler handler)
        {
            int typeId = GetNextTypeId();

            RegisterMessageType<T>(typeId, new MessageType<T>(serializer, handler));

            return typeId;
        }
        
        /// <summary>
        /// Get the next unused type id for the message type registration.
        /// </summary>
        /// <returns>The type id.</returns>
        int GetNextTypeId()
        {
            int id = nextTypeId;

            UpdateNextTypeId();

            return id;
        }

        /// <summary>
        /// Increment the next type id.
        /// </summary>
        void UpdateNextTypeId()
        {
            if (objectTypeIds.Count == 0) {
                return;
            }

            int[] typeIds = objectTypeIds.Values.OrderByDescending(id => id).ToArray();
            
            nextTypeId = typeIds[0] + 1;
        }

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
        /// Get a message type by it's id.
        /// </summary>
        /// <param name="id">The type id.</param>
        /// <returns>The message type.</returns>
        public IMessageType GetTypeById(int id)
        {
            foreach (KeyValuePair<Type, int> entry in objectTypeIds) {
                if (entry.Value != id) {
                    continue;
                }

                if (messageTypes.TryGetValue(entry.Key, out IMessageType type)) {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the message type id of an object type.
        /// </summary>
        /// <param name="objType">The object type.</param>
        /// <returns>The message type id.</returns>
        public int GetTypeId(Type objType)
        {
            if (objectTypeIds.TryGetValue(objType, out int id)) {
                return id;
            }

            return default(int);
        }

        /// <summary>
        /// Get the message type id of a message type.
        /// </summary>
        /// <param name="objType">The message type.</param>
        /// <returns>The message type id.</returns>
        public int GetTypeId(IMessageType type)
        {
            foreach (KeyValuePair<Type, IMessageType> entry in messageTypes) {
                if (entry.Value != type) {
                    continue;
                }

                return GetTypeId(entry.Key);
            }

            return default(int);
        }
    }
}