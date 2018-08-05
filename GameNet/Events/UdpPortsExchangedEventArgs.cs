using System;
using System.Net;

namespace GameNet.Events
{
    public class UdpPortsExchangedEventArgs: EventArgs
    {
        public ClientContainer Client { get; }
        public IPEndPoint ClientEndpoint => Client.UdpEndpoint;
        public int ClientPort => ClientEndpoint.Port;

        public UdpPortsExchangedEventArgs(ClientContainer client)
        {
            Client = client;
        }
    }
}