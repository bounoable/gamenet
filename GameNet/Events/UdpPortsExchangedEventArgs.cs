using System;
using System.Net;

namespace GameNet.Events
{
    public class UdpPortsExchangedEventArgs: EventArgs
    {
        public Player Player { get; }
        public IPEndPoint PlayerEndpoint => Player.UdpEndpoint;
        public int PlayerPort => PlayerEndpoint.Port;

        public UdpPortsExchangedEventArgs(Player player)
        {
            Player = player;
        }
    }
}