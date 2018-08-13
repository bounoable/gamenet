using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class PlayerDisconnectedEventArgs: EventArgs
    {
        public Player Player { get; }
        public DateTime DisconnectedAt { get; } = DateTime.Now;

        public PlayerDisconnectedEventArgs(Player player)
        {
            Player = player;
        }
    }
}