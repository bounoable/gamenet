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
    }
}