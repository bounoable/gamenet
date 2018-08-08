using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class PlayerConnectedEventArgs: EventArgs
    {
        public Player Client { get; }
        public DateTime ConnectedAt { get; } = DateTime.Now;

        public PlayerConnectedEventArgs(Player client)
        {
            Client = client;
        }
    }
}