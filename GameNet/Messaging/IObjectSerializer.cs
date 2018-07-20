namespace GameNet.Messaging
{
    public interface IObjectSerializer
    {
        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        byte[] Serialize(object obj);
    }
}