using System;
using System.Net.Sockets;

namespace GameNet
{
    public class ClientConnectedEventArgs: EventArgs
    {
        public TcpClient TcpClient { get; }
        public DateTime ConnectedAt { get; } = DateTime.Now;

        public ClientConnectedEventArgs(TcpClient client)
        {
            TcpClient = client;
        }
    }
}