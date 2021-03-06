using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GameNet.Support
{
    public class DataBuilder
    {
        /// <summary>
        /// Get the built data.
        /// </summary>
        public byte[] Data => data.ToArray();

        /// <summary>
        /// The data builder list.
        /// </summary>
        /// <typeparam name="byte">The bytes of the data.</typeparam>
        List<byte> data = new List<byte>();

        /// <summary>
        /// Append a byte to the data.
        /// </summary>
        /// <param name="data">The byte to append</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Append(byte data) => Append(new byte[] { data });

        /// <summary>
        /// Append bytes to the data.
        /// </summary>
        /// <param name="data">The bytes to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Append(byte[] data)
        {
            this.data.AddRange(ToLittleEndian(data));

            return this;
        }

        /// <summary>
        /// Append a byte to the data.
        /// </summary>
        /// <param name="value">The byte to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Byte(byte value) => Append(value);

        /// <summary>
        /// Append bytes to the data.
        /// </summary>
        /// <param name="value">The bytes to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Bytes(byte[] value) => Append(value);

        /// <summary>
        /// Append an sbyte to the data.
        /// </summary>
        /// <param name="value">The sbyte to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder SByte(sbyte value) => Append(unchecked((byte)value));

        /// <summary>
        /// Append sbytes to the data.
        /// </summary>
        /// <param name="value">The sbytes to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder SBytes(sbyte[] value) => Append(value.Select(sb => unchecked((byte)sb)).ToArray());

        /// <summary>
        /// Append a boolean to the data.
        /// </summary>
        /// <param name="value">The boolean to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Bool(bool value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a short (16 bit int) to the data.
        /// </summary>
        /// <param name="value">The short to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Short(short value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a ushort (unsigned 16 bit int) to the data.
        /// </summary>
        /// <param name="value">The ushort to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder UShort(ushort value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append an integer to the data.
        /// </summary>
        /// <param name="value">The integer to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Int(int value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append an unsigned integer to the data.
        /// </summary>
        /// <param name="value">The unsigned integer to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder UInt(uint value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a long to the data.
        /// </summary>
        /// <param name="value">The long to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Long(long value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a ulong to the data.
        /// </summary>
        /// <param name="value">The ulong to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder ULong(ulong value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a float to the data.
        /// </summary>
        /// <param name="value">The float to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Float(float value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a double to the data.
        /// </summary>
        /// <param name="value">The double to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Double(double value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a char to the data.
        /// </summary>
        /// <param name="value">The char to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Char(char value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a string to the data.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder String(string value)
        {   
            int length = string.IsNullOrEmpty(value) ? 0 : value.Length;

            Int(length);

            return length > 0 ? Append(Encoding.Unicode.GetBytes(value)) : this;
        }

        DataBuilder Enum(Type enumType, Enum value)
        {
            if (enumType == typeof(short)) {
                return Short((short)(object)value);
            }

            if (enumType == typeof(ushort)) {
                return UShort((ushort)(object)value);
            }

            if (enumType == typeof(int)) {
                return Int((int)(object)value);
            }

            if (enumType == typeof(uint)) {
                return UInt((uint)(object)value);
            }

            if (enumType == typeof(long)) {
                return Long((long)(object)value);
            }

            if (enumType == typeof(ulong)) {
                return ULong((ulong)(object)value);
            }

            return Int((int)(object)value);
        }

        /// <summary>
        /// Append an enum value to the data.
        /// </summary>
        /// <param name="value">The enum value to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Enum<T>(T value) where T: Enum
            => Enum(typeof(T), value);

        /// <summary>
        /// Transform a byte array to little endian if the operating system uses big endian.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The corrected byte array.</returns>
        public static byte[] ToLittleEndian(byte[] data) => BitConverter.IsLittleEndian ? data : data.Reverse().ToArray();
    }
}