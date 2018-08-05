using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameNet.Support
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
        /// Convert a byte to an sbyte.
        /// </summary>
        /// <param name="data">The byte.</param>
        /// <returns>The sbyte.</returns>
        public static sbyte ConvertToSByte(byte data) => unchecked((sbyte)data);

        /// <summary>
        /// Convert bytes to sbytes.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The sbyte array.</returns>
        public static sbyte[] ConvertToSBytes(byte[] data) => Array.ConvertAll(data, b => unchecked((sbyte)b));

        /// <summary>
        /// Convert bytes into a boolean.
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
        /// Convert bytes to a ushort.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The ushort.</returns>
        public static ushort ConvertToUShort(byte[] data) => BitConverter.ToUInt16(data, 0);

        /// <summary>
        /// Convert bytes to an int.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The integer.</returns>
        public static int ConvertToInt(byte[] data) => BitConverter.ToInt32(data, 0);

        /// <summary>
        /// Convert bytes to an uint.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The unsigned integer.</returns>
        public static uint ConvertToUInt(byte[] data) => BitConverter.ToUInt32(data, 0);

        /// <summary>
        /// Convert bytes to a long.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The long.</returns>
        public static long ConvertToLong(byte[] data) => BitConverter.ToInt64(data, 0);

        /// <summary>
        /// Convert bytes to an ulong.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The unsigned long.</returns>
        public static ulong ConvertToULong(byte[] data) => BitConverter.ToUInt64(data, 0);

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
        /// Get a byte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The byte.</returns>
        public static byte GetByte(byte[] data, int offset = 0)
            => data[offset];
        
        /// <summary>
        /// Get an sbyte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The sbyte.</returns>
        public static sbyte GetSByte(byte[] data, int offset = 0)
            => unchecked((sbyte)data[offset]);

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
            => ConvertToShort(Slice(data, offset, sizeof(short)));
        
        /// <summary>
        /// Get a ushort (unsigned 16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The ushort (unsigned 16 bit int).</returns>
        public static ushort GetUShort(byte[] data, int offset = 0)
            => ConvertToUShort(Slice(data, offset, sizeof(ushort)));
        
        /// <summary>
        /// Get an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        public static int GetInt(byte[] data, int offset = 0)
            => ConvertToInt(Slice(data, offset, sizeof(int)));
        
        /// <summary>
        /// Get an unsigned integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The unsigned integer.</returns>
        public static uint GetUInt(byte[] data, int offset = 0)
            => ConvertToUInt(Slice(data, offset, sizeof(uint)));
        
        /// <summary>
        /// Get a long from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The long.</returns>
        public static long GetLong(byte[] data, int offset = 0)
            => ConvertToLong(Slice(data, offset, sizeof(long)));

        /// <summary>
        /// Get a ulong from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The ulong.</returns>
        public static ulong GetULong(byte[] data, int offset = 0)
            => ConvertToULong(Slice(data, offset, sizeof(ulong)));
        
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
        /// Get an enum from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The enum value.</returns>
        public static T GetEnum<T>(byte[] data, int offset = 0) where T: Enum
        {
            Type enumType = Enum.GetUnderlyingType(typeof(T));
            int size = Marshal.SizeOf(enumType);
            byte[] enumBytes = Slice(data, offset, size);

            if (enumType == typeof(short)) {
                return (T)(object)ConvertToShort(enumBytes);
            }

            if (enumType == typeof(ushort)) {
                return (T)(object)ConvertToShort(enumBytes);
            }

            if (enumType == typeof(int)) {
                return (T)(object)ConvertToInt(enumBytes);
            }

            if (enumType == typeof(uint)) {
                return (T)(object)ConvertToUInt(enumBytes);
            }

            if (enumType == typeof(long)) {
                return (T)(object)ConvertToLong(enumBytes);
            }

            if (enumType == typeof(ulong)) {
                return (T)(object)ConvertToULong(enumBytes);
            }

            return (T)(object)ConvertToInt(enumBytes);
        }

        /// <summary>
        /// Pull a byte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The byte.</returns>
        public static byte PullByte(ref byte[] data)
        {
            byte result = GetByte(data);
            data = data.Skip(sizeof(byte)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull an sbyte from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The sbyte.</returns>
        public static sbyte PullSByte(ref byte[] data)
        {
            sbyte result = GetSByte(data);
            data = data.Skip(sizeof(byte)).ToArray();

            return result;
        }
        
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
        /// Pull a ushort (unsigned 16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The ushort (unsigned 16 bit int).</returns>
        public static ushort PullUShort(ref byte[] data)
        {
            ushort result = GetUShort(data);
            data = data.Skip(sizeof(ushort)).ToArray();

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
        /// Pull an unsigned integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The unsigned integer.</returns>
        public static uint PullUInt(ref byte[] data)
        {
            uint result = GetUInt(data);
            data = data.Skip(sizeof(uint)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a long from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The long.</returns>
        public static long PullLong(ref byte[] data)
        {
            long result = GetLong(data);
            data = data.Skip(sizeof(long)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a ulong from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The ulong.</returns>
        public static ulong PullULong(ref byte[] data)
        {
            ulong result = GetULong(data);
            data = data.Skip(sizeof(ulong)).ToArray();

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
            data = data.Skip(result.Length * sizeof(char) + sizeof(int)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull an enum value from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The enum value.</returns>
        public static T PullEnum<T>(ref byte[] data) where T: Enum
        {
            Type enumType = Enum.GetUnderlyingType(typeof(T));
            T result = GetEnum<T>(data);
            int size = Marshal.SizeOf(enumType);

            data = data.Skip(size).ToArray();

            return result;
        }
    }
}