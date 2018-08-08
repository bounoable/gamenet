using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class PlayerDisconnectedEventArgs: EventArgs
    {
        public Player Client { get; }
        public DateTime DisconnectedAt { get; } = DateTime.Now;

        public PlayerDisconnectedEventArgs(Player client)
        {
            Client = client;
        }
    }
}