namespace GameNet.Messaging
{
    public interface IObjectSerializer<T>
    {
        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        byte[] Serialize(T obj);

        /// <summary>
        /// Deserialize bytes back into the object.
        /// </summary>
        /// <param name="data">The byte array</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The deserialized object.</returns>
        T Deserialize(byte[] data);
    }
}