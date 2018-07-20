namespace GameNet.Messaging
{
    abstract public class ObjectSerializer<TObject>: IObjectSerializer
    {
        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        public byte[] Serialize(object obj)
        {
            if (obj.GetType() != typeof(TObject)) {
                return null;
            }

            return GetBytes((TObject)obj);
        }

        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        abstract protected byte[] GetBytes(TObject obj);

        /// <summary>
        /// Initialize a data builder.
        /// </summary>
        /// <returns>The data builder.</returns>
        protected DataBuilder Build() => new DataBuilder();
    }
}