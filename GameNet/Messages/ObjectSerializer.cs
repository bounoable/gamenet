using System;
using GameNet.Support;

namespace GameNet.Messages
{
    abstract public class ObjectSerializer<T>: IObjectSerializer where T: class
    {
        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        public byte[] Serialize(object obj)
        {
            if (obj.GetType() != typeof(T)) {
                return new byte[0];
            }

            return GetBytes((T)obj);
        }

        /// <summary>
        /// Serialize an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        abstract public byte[] GetBytes(T obj);

        /// <summary>
        /// Deserialize bytes back into the object.
        /// </summary>
        /// <param name="data">The byte array</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(byte[] data) => (object)GetObject(data);

        /// <summary>
        /// Deserialize bytes back into the object.
        /// </summary>
        /// <param name="data">The byte array</param>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The deserialized object.</returns>
        abstract public T GetObject(byte[] data);

        /// <summary>
        /// Initialize a data builder.
        /// </summary>
        /// <returns>The data builder.</returns>
        protected DataBuilder Build() => new DataBuilder();

        /// <summary>
        /// Pull a byte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The byte.</returns>
        public static byte PullByte(ref byte[] data) => DataHelper.PullByte(ref data);

        /// <summary>
        /// Pull bytes from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="count">The count.</param>
        /// <returns>The byte.</returns>
        public static byte[] PullBytes(ref byte[] data, int count) => DataHelper.PullBytes(ref data, count);

        /// <summary>
        /// Pull an sbyte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The sbyte.</returns>
        public static sbyte PullSByte(ref byte[] data) => DataHelper.PullSByte(ref data);

        /// <summary>
        /// Pull sbytes from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="count">The count.</param>
        /// <returns>The sbytes.</returns>
        public static sbyte[] PullSBytes(ref byte[] data, int count) => DataHelper.PullSBytes(ref data, count);

        /// <summary>
        /// Pull a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The boolean.</returns>
        protected static bool PullBool(ref byte[] data) => DataHelper.PullBool(ref data);

        /// <summary>
        /// Pull a short (16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The short (16 bit int).</returns>
        protected static short PullShort(ref byte[] data) => DataHelper.PullShort(ref data);

        /// <summary>
        /// Pull a ushort (unsigned 16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The ushort (unsigned 16 bit int).</returns>
        protected static ushort PullUShort(ref byte[] data) => DataHelper.PullUShort(ref data);

        /// <summary>
        /// Pull an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The integer.</returns>
        protected static int PullInt(ref byte[] data) => DataHelper.PullInt(ref data);

        /// <summary>
        /// Pull an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The integer.</returns>
        protected static uint PullUInt(ref byte[] data) => DataHelper.PullUInt(ref data);

        /// <summary>
        /// Pull a long from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The long.</returns>
        public static long PullLong(ref byte[] data) => DataHelper.PullLong(ref data);

        /// <summary>
        /// Pull a ulong from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The ulong.</returns>
        public static ulong PullULong(ref byte[] data) => DataHelper.PullULong(ref data);

        /// <summary>
        /// Pull a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The float.</returns>
        protected static float PullFloat(ref byte[] data) => DataHelper.PullFloat(ref data);

        /// <summary>
        /// Pull a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The double.</returns>
        protected static double GetDouble(ref byte[] data) => DataHelper.PullDouble(ref data);

        /// <summary>
        /// Pull a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The char.</returns>
        protected static char PullChar(ref byte[] data) => DataHelper.PullChar(ref data);

        /// <summary>
        /// Pull a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The string.</returns>
        protected static string PullString(ref byte[] data) => DataHelper.PullString(ref data);

        /// <summary>
        /// Pull an enum value from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>The enum value.</returns>
        protected static TEnum PullEnum<TEnum>(ref byte[] data) where TEnum: Enum => DataHelper.PullEnum<TEnum>(ref data);
    }
}