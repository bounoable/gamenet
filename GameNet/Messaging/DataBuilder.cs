using System;
using System.Text;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class DataBuilder
    {
        /// <summary>
        /// Get the built data.
        /// </summary>
        public byte[] Data => data.ToArray();

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
            this.data.AddRange(data);

            return this;
        }

        /// <summary>
        /// Append a byte to the data.
        /// </summary>
        /// <param name="value">The byte to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Byte(byte value) => Append(value);

        /// <summary>
        /// Append a boolean to the data.
        /// </summary>
        /// <param name="value">The boolean to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Bool(bool value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append a short (16 bit int) to the data.
        /// </summary>
        /// <param name="value">The short (16 bit int) to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Short(short value) => Append(BitConverter.GetBytes(value));

        /// <summary>
        /// Append an integer to the data.
        /// </summary>
        /// <param name="value">The integer to append.</param>
        /// <returns>The data builder.</returns>
        public DataBuilder Int(int value) => Append(BitConverter.GetBytes(value));

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
            Int(value.Length);

            return Append(Encoding.Unicode.GetBytes(value));
        }
    }
}