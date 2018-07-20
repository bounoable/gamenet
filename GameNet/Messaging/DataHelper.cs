using System;
using System.Text;
using System.Linq;

namespace GameNet.Messaging
{
    public static class DataHelper
    {
        /// <summary>
        /// Transform received data (little endian) to the local endian type.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <returns>Data of the correct endian type for this system.</returns>
        public static byte[] ToSystemEndian(byte[] data)
            => BitConverter.IsLittleEndian ? data : data.Reverse().ToArray();
        
        /// <summary>
        /// Slice out a part of bytes from a byte array.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The number of bytes to skip.</param>
        /// <param name="count">The number of bytes to take.</param>
        /// <returns>The sliced out bytes.</returns>
        public static byte[] Slice(byte[] buffer, int offset, int count)
            => buffer.Skip(offset).Take(count).ToArray();
        
        /// <summary>
        /// Transform bytes into a boolean.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The boolean.</returns>
        public static bool ConvertToBool(byte[] data) => BitConverter.ToBoolean(data, 0);

        /// <summary>
        /// Convert bytes to a short.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The short.</returns>
        public static short ConvertToShort(byte[] data) => BitConverter.ToInt16(data, 0);

        /// <summary>
        /// Convert bytes to an int.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The integer.</returns>
        public static int ConvertToInt(byte[] data) => BitConverter.ToInt32(data, 0);

        /// <summary>
        /// Convert bytes to a float.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The float.</returns>
        public static float ConvertToFloat(byte[] data) => BitConverter.ToSingle(data, 0);

        /// <summary>
        /// Convert bytes to a double.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The double.</returns>
        public static double ConvertToDouble(byte[] data) => BitConverter.ToDouble(data, 0);

        /// <summary>
        /// Convert bytes to a char.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The char.</returns>
        public static char ConvertToChar(byte[] data) => BitConverter.ToChar(data, 0);

        /// <summary>
        /// Convert bytes to a string with UTF-16 encoding..
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The string.</returns>
        public static string ConvertToString(byte[] data) => Encoding.Unicode.GetString(data);

        /// <summary>
        /// Get a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The boolean.</returns>
        public static bool GetBool(byte[] data, int offset = 0)
            => ConvertToBool(Slice(data, offset, sizeof(bool)));
        
        /// <summary>
        /// Get a short (16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The short (16 bit int).</returns>
        public static short GetShort(byte[] data, int offset = 0)
            => ConvertToShort(Slice(data, offset, sizeof(int)));
        
        /// <summary>
        /// Get an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        public static int GetInt(byte[] data, int offset = 0)
            => ConvertToInt(Slice(data, offset, sizeof(int)));
        
        /// <summary>
        /// Get a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The float.</returns>
        public static float GetFloat(byte[] data, int offset = 0)
            => ConvertToFloat(Slice(data, offset, sizeof(float)));
        
        /// <summary>
        /// Get a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The double.</returns>
        public static double GetDouble(byte[] data, int offset = 0)
            => ConvertToFloat(Slice(data, offset, sizeof(double)));
        
        /// <summary>
        /// Get a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The char.</returns>
        public static char GetChar(byte[] data, int offset = 0)
            => ConvertToChar(Slice(data, offset, sizeof(char)));
        
        /// <summary>
        /// Get a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The string.</returns>
        public static string GetString(byte[] data, int offset = 0)
            => ConvertToString(Slice(data, offset + sizeof(int), GetInt(data, offset) * sizeof(char)));
        
        /// <summary>
        /// Pull a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The boolean.</returns>
        public static bool PullBool(ref byte[] data)
        {
            bool result = GetBool(data);
            data = data.Skip(sizeof(bool)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a short (16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The short (16 bit int).</returns>
        public static short PullShort(ref byte[] data)
        {
            short result = GetShort(data);
            data = data.Skip(sizeof(short)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        public static int PullInt(ref byte[] data)
        {
            int result = GetInt(data);
            data = data.Skip(sizeof(int)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The float.</returns>
        public static float PullFloat(ref byte[] data)
        {
            float result = GetFloat(data);
            data = data.Skip(sizeof(float)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The double.</returns>
        public static double PullDouble(ref byte[] data)
        {
            double result = GetDouble(data);
            data = data.Skip(sizeof(double)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The char.</returns>
        public static char PullChar(ref byte[] data)
        {
            char result = GetChar(data);
            data = data.Skip(sizeof(char)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The string.</returns>
        public static string PullString(ref byte[] data)
        {
            string result = GetString(data);
            data = data.Skip(result.Length * sizeof(char)).ToArray();

            return result;
        }
    }
}