namespace GameNet.Messages
{
    public interface IUdpPortMessage
    {
        /// <summary>
        /// The client's session secret. Not set if the message is sent by the server.
        /// </summary>
        /// 
        string Secret { get; }

        /// <summary>
        /// The sender's UDP port.
        /// </summary>
        ushort Port { get; }
    }
}