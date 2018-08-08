using System;
using System.Net;

namespace GameNet.Events
{
    public class UdpPortsExchangedEventArgs: EventArgs
    {
        public Player Client { get; }
        public IPEndPoint ClientEndpoint => Client.UdpEndpoint;
        public int ClientPort => ClientEndpoint.Port;

        public UdpPortsExchangedEventArgs(Player client)
        {
            Client = client;
        }
    }
}