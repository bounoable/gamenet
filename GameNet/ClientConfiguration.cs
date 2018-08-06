namespace GameNet
{
    public class ClientConfiguration: NetworkConfiguration
    {
        public const int DEFAULT_STILL_CONNECTED_INTERVAL = 10;

        public int StillConnectedInterval { get; set; } = DEFAULT_STILL_CONNECTED_INTERVAL;

        public ClientConfiguration()
        {}

        public ClientConfiguration(int stillConnectedInterval)
        {
            StillConnectedInterval = stillConnectedInterval;
        }

        public ClientConfiguration(int stillConnectedInterval, ushort localUdpPort): base(localUdpPort)
        {
            StillConnectedInterval = stillConnectedInterval;
        }

        public ClientConfiguration(ushort localUdpPort): base(localUdpPort)
        {
        }
    }
}