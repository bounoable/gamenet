using System.Net;

namespace GameNet
{
    public class ServerConfiguration: NetworkConfiguration
    {
        public const ushort DEFAULT_PORT = 26000;

        public IPAddress IPAddress { get; set; } = IPAddress.Parse("127.0.0.1");
        public ushort Port { get; set; } = DEFAULT_PORT;
    }
}