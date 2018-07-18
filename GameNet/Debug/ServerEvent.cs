namespace GameNet.Debug
{
    public enum ServerEvent
    {
        ServerStarted,
        ServerStartFailed,
        ServerStopped,
        ServerStopFailed,
        ClientConnected,
        TcpClientConnected,
        UdpClientConnected,
    }
}