using System.Net;
using System.Linq;
using GameNet.Debug;
using GameNet.Messaging;
using System.Net.Sockets;
using System.Threading.Tasks;

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
        async public Task Send(IMessage message)
        {
            await Task.WhenAll(tcpClients.Select(
                client => messenger.Send(client.GetStream(), message)
            ));
        }

        /// <summary>
        /// Send a message to all clients.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task Send(object obj)
        {
            await Task.WhenAll(tcpClients.Select(
                client => messenger.Send(client.GetStream(), obj)
            ));
        }
    }
}