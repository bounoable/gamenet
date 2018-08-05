using System;
using System.Net;
using System.Threading;
using GameNet.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    abstract public class Endpoint
    {
        public int UdpPort => _udpClient != null ? ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port : -1;

        public NetworkConfiguration NetworkConfig { get; }
        
        abstract protected bool ShouldReceiveData { get; }

        protected UdpClient _udpClient;

        HashSet<IDataHandler> _dataHandlers = new HashSet<IDataHandler>();

        /// <summary>
        /// Initialize an endpoint.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public Endpoint(NetworkConfiguration config)
        {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            NetworkConfig = config;
            _udpClient = new UdpClient(NetworkConfig.LocalUdpPort);
        }

        /// <summary>
        /// Register a data handler for the received data.
        /// </summary>
        /// <param name="handler">The data handler.</param>
        public void RegisterDataHandler(IDataHandler handler) => _dataHandlers.Add(handler);

        /// <summary>
        /// Let the endpoint receive data from a TCP client.
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
        /// Let the endpoint receive data from an endpoint over an UDP client.
        /// </summary>
        /// <param name="client">The UDP client.</param>
        /// <param name="endpoint">The remote endpoint</param>
        /// <returns></returns>
        protected async Task ReceiveData(UdpClient client, IPEndPoint endpoint)
        {
            await Task.Run(() => {
                while (ShouldReceiveData) {
                    byte[] data = client.Receive(ref endpoint);

                    if (data.Length == 0)
                        continue;

                    HandleReceivedData(data);
                }
            });
        }

        /// <summary>
        /// Call the registered data handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        protected void HandleReceivedData(byte[] data)
        {
            foreach (IDataHandler handler in _dataHandlers) {
                handler.Handle(data);
            }
        }
    }
}