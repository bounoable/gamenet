using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class ClientConnectedEventArgs: EventArgs
    {
        public ClientContainer Client { get; }
        public DateTime ConnectedAt { get; } = DateTime.Now;

        public ClientConnectedEventArgs(ClientContainer client)
        {
            Client = client;
        }
    }
}