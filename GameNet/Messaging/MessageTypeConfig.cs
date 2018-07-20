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
        Dictionary<int, IMessageType> messageTypes = new Dictionary<int, IMessageType>();

        /// <summary>
        /// Register a message type.
        /// </summary>
        /// <param name="type">The message type.</param>
        public void RegisterMessageType(IMessageType type, TypeIdConflict conflict = TypeIdConflict.Override)
        {
            if (messageTypes.ContainsKey(type.TypeId)) {
                if (conflict == TypeIdConflict.Override) {
                    messageTypes[type.TypeId] = type;
                    return;
                }

                if (conflict == TypeIdConflict.KeepOld) {
                    return;
                }

                if (conflict == TypeIdConflict.GenerateNew) {
                    type.TypeId = GetNextTypeId();
                }
            }

            messageTypes[type.TypeId] = type;
        }

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="handler">The message handler.</param>
        /// <param name="serializer">The object serializer.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType(Type objectType, IMessageHandler handler, IObjectSerializer serializer)
        {
            int typeId = GetNextTypeId();

            RegisterMessageType(new MessageType(typeId, objectType, handler, serializer));

            return typeId;
        }

        /// <summary>
        /// Register a message type and return it's ID.
        /// </summary>
        /// <param name="handler">The message handler.</param>
        /// <param name="serializer">The object serializer.</param>
        /// <returns>The generated type id.</returns>
        public int RegisterMessageType<TObject>(IMessageHandler handler, IObjectSerializer serializer)
            => RegisterMessageType(typeof(TObject), handler, serializer);
        
        /// <summary>
        /// Get the next unused type id for the message type registration.
        /// </summary>
        /// <returns>The type id.</returns>
        int GetNextTypeId()
        {
            List<int> ids = messageTypes.Keys.ToList();

            ids.Sort((a, b) => a.CompareTo(b));

            return ids.Count > 0 ? ids.Last() + 1 : 0;
        }
        
        /// <summary>
        /// Get a message type by it's id.
        /// </summary>
        /// <param name="id">The type id.</param>
        /// <returns>The message type.</returns>
        public IMessageType GetTypeById(int id)
        {
            if (messageTypes.TryGetValue(id, out IMessageType type)) {
                if (id != type.TypeId) {
                    return null;
                }

                return type;
            }

            return null;
        }

        /// <summary>
        /// Get the message type of an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public IMessageType GetTypeByObject(object obj)
        {
            Type type = obj.GetType();

            foreach (KeyValuePair<int, IMessageType> entry in messageTypes) {
                if (entry.Value.ObjectType == type) {
                    return entry.Value;
                }
            }

            return null;
        }
    }
}