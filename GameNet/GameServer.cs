using System.Net;
using GameNet.Debug;
using GameNet.Messaging;
using System.Net.Sockets;

namespace GameNet
{
    public class GameServer: Server
    {
        /// <summary>
        /// Initialize a game server.
        /// </summary>
        /// <param name="ipAddress">The IP address the server runs on.</param>
        /// <param name="port">The port the server listens on.</param>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The server debugger.</param>
        public GameServer(IPAddress ipAddress, int port, Messenger messenger, IServerDebugger debugger = null): base(ipAddress, port, messenger, debugger)
        {}

        /// <summary>
        /// Initialize a server.
        /// </summary>
        /// <param name="ipAddress">The IP address the server runs on.</param>
        /// <param name="port">The port the server listens on.</param>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The server debugger.</param>
        public GameServer(string ipAddress, int port, Messenger messenger, IServerDebugger debugger = null): this(IPAddress.Parse(ipAddress), port, messenger, debugger)
        {}

        /// <summary>
        /// Send a message to the clients.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(IMessage message)
        {
            foreach (TcpClient client in tcpClients) {
                messenger.Send(client.GetStream(), message);
            }
        }
    }
}