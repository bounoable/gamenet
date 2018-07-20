using GameNet.Debug;
using GameNet.Messaging;
using System.Threading.Tasks;

namespace GameNet
{
    public class GameClient: Client
    {
        /// <summary>
        /// Initialize the game client.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The client debugger.</param>
        public GameClient(Messenger messenger, IClientDebugger debugger = null): base(messenger, debugger)
        {}

        /// <summary>
        /// Send a message to the server and return the sent data.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send(IMessage message) => messenger.Send(server.GetStream(), message);

        /// <summary>
        /// Write a message to the server and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send(object obj) => messenger.Send(server.GetStream(), obj);
    }
}