namespace GameNet
{
    public class ClientConfiguration: NetworkConfiguration
    {
        public const int DEFAULT_HEARTBEAT_INTERVAL = 20000;

        public int HeartbeatInterval { get; set; } = DEFAULT_HEARTBEAT_INTERVAL;

        public ClientConfiguration()
        {}

        public ClientConfiguration(int heartbeatInterval)
        {
            HeartbeatInterval = heartbeatInterval;
        }

        public ClientConfiguration(int heartbeatInterval, ushort localUdpPort): base(localUdpPort)
        {
            HeartbeatInterval = heartbeatInterval;
        }

        public ClientConfiguration(ushort localUdpPort): base(localUdpPort)
        {
        }
    }
}