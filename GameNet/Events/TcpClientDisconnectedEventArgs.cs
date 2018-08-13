using System;
using System.Net.Sockets;

namespace GameNet.Events
{
    public class TcpClientDisconnectedEventArgs: EventArgs
    {
        public TcpClient Client { get; }

        public TcpClientDisconnectedEventArgs(TcpClient client)
        {
            Client = client;
        }
    }
}