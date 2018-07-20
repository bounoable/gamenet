using GameNet.Messaging;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    abstract public class Endpoint
    {
        abstract protected bool ShouldReceiveData { get; }

        HashSet<IDataHandler> dataHandlers = new HashSet<IDataHandler>();

        /// <summary>
        /// Register a data handler for the received data.
        /// </summary>
        /// <param name="handler">The data handler.</param>
        public void RegisterDataHandler(IDataHandler handler) => dataHandlers.Add(handler);

        /// <summary>
        /// Let the client receive data from the server.
        /// </summary>
        protected async Task ReceiveData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (ShouldReceiveData) {
                if (stream.CanRead && stream.DataAvailable) {
                    var data = new byte[client.ReceiveBufferSize];

                    await stream.ReadAsync(data, 0, client.ReceiveBufferSize);

                    HandleReceivedData(data);
                }
            }
        }

        /// <summary>
        /// Call the registered data handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        protected void HandleReceivedData(byte[] data)
        {
            foreach (IDataHandler handler in dataHandlers) {
                handler.Handle(data);
            }
        }
    }
}