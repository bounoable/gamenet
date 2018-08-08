using System.Net;

namespace GameNet
{
    public class ServerConfiguration: NetworkConfiguration
    {
        public const ushort DEFAULT_PORT = 26000;
        public const int DEFAULT_HEARTBEAT_TIMEOUT = 15000;

        public IPAddress IPAddress { get; set; } = IPAddress.Parse("127.0.0.1");
        public ushort Port { get; set; } = DEFAULT_PORT;
        public int HeartbeatTimeout { get; set; } = DEFAULT_HEARTBEAT_TIMEOUT;

        public ServerConfiguration()
        {}

        public ServerConfiguration(ushort localUdpPort): base(localUdpPort)
        {}

        public ServerConfiguration(IPAddress ipAddress, ushort tcpPort, ushort udpPort): base(udpPort)
        {
            IPAddress = ipAddress;
            Port = tcpPort;
        }
    }
}