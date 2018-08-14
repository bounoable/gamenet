using System;
using System.Net;

namespace GameNet.Support
{
    public static class Validator
    {
        /// <summary>
        /// Validate an IP address.
        /// </summary>
        /// <param name="port">The IP address to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ip address is null.</exception>
        public static void ValidateIPAddress(IPAddress ip)
        {
            if (ip == null) {
                throw new ArgumentNullException("Ip address cannot be null.");
            }
        }

        /// <summary>
        /// Validate a port number. It must be an integer between 1 and 65535.
        /// </summary>
        /// <param name="port">The port to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given port is invalid.</exception>
        public static void ValidatePort(int port)
        {
            if (port < 1 || port > 65535) {
                throw new ArgumentOutOfRangeException($"Invalid port ({port}). Port must be between 1 and 65535.");
            }
        }
    }
}