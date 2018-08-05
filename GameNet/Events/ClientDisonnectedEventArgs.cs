using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class ClientDisconnectedEventArgs: EventArgs
    {
        public ClientContainer Client { get; }
        public DateTime DisconnectedAt { get; } = DateTime.Now;

        public ClientDisconnectedEventArgs(ClientContainer client)
        {
            Client = client;
        }
    }
}