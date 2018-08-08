namespace GameNet.Protocol
{
    public class MessengerConfig
    {
        public int AcknowledgeMessageRetries { get; set; } = 5;
        public int AcknowledgeMessageInterval { get; set; } = 1000;

        public MessengerConfig()
        {}

        public MessengerConfig(int acknowledgeMessageRetries, int acknowledgeMessageInterval)
        {
            AcknowledgeMessageRetries = acknowledgeMessageRetries;
            AcknowledgeMessageInterval = acknowledgeMessageInterval;
        }
    }
}