using System;
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
    }
}