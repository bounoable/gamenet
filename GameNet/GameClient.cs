using GameNet.Debug;
using GameNet.Messaging;
using System.Threading.Tasks;

namespace GameNet
{
    public class GameClient: Client
    {
        public GameClient(Messenger messenger, IClientDebugger debugger = null): base(messenger, debugger)
        {}

        /// <summary>
        /// Send a message to the server and return the sent data.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> Send(IMessage message) => await messenger.Send(server.GetStream(), message);
    }
}