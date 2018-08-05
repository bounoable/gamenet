namespace GameNet
{
    public class NetworkConfiguration
    {
        public const ushort DEFAULT_UDP_PORT = 25000;

        public ushort LocalUdpPort { get; set; } = DEFAULT_UDP_PORT;

        public NetworkConfiguration()
        {}

        public NetworkConfiguration(ushort localUdpPort)
        {
            LocalUdpPort = localUdpPort;
        }
    }
}