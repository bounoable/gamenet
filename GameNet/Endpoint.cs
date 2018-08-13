using System;
using System.Net;
using System.Linq;
using GameNet.Events;
using GameNet.Support;
using System.Threading;
using GameNet.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GameNet
{
    abstract public class Endpoint
    {
        public int UdpPort => _udpClient != null ? ((IPEndPoint)_udpClient.Client.LocalEndPoint).Port : -1;

        public NetworkConfiguration NetworkConfig { get; }
        
        abstract protected bool ShouldReceiveData { get; }

        protected UdpClient _udpClient;

        readonly HashSet<IDataHandler> _dataHandlers = new HashSet<IDataHandler>();

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

            if (!stream.CanRead)
                return;

            while (ShouldReceiveData) {
                var lengthBuffer = new byte[sizeof(int)];

                if (await stream.ReadAsync(lengthBuffer, 0, sizeof(int)) == 0)
                    continue;

                int length = DataHelper.GetInt(lengthBuffer);
                int receivedByteCount = 0;
                var payload = new byte[0];

                while (receivedByteCount < length) {
                    int remaining = length - receivedByteCount;
                    byte[] buffer = new byte[remaining];

                    int received = await stream.ReadAsync(buffer, 0, remaining);
                    receivedByteCount += received;

                    if (received == 0) {
                        return;
                    }

                    payload = payload.Concat(buffer.Take(received)).ToArray();
                }

                HandleReceivedData(payload.ToArray(), new TcpRecipient(client));
            }
        }

        /// <summary>
        /// Let the endpoint receive data from an endpoint over an UDP client.
        /// </summary>
        /// <param name="client">The UDP client.</param>
        /// <param name="endpoint">The remote endpoint</param>
        protected async Task ReceiveData(UdpClient client, IPEndPoint endpoint)
        {
            await Task.Run(() => {
                while (ShouldReceiveData) {
                    byte[] data = client.Receive(ref endpoint);

                    if (data.Length == 0)
                        continue;

                    HandleReceivedData(data, new UdpRecipient(client, endpoint));
                }
            });
        }

        /// <summary>
        /// Call the registered data handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="recipient">The sender of the data.</param>
        protected void HandleReceivedData(byte[] data, IRecipient sender)
        {
            foreach (IDataHandler handler in _dataHandlers) {
                handler.Handle(data, sender);
            }
        }
    }
}