using System;
using System.Net;
using System.Linq;
using GameNet.Debug;
using GameNet.Events;
using System.Threading;
using GameNet.Messages;
using GameNet.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    public class Server: Endpoint
    {
        public ServerConfiguration Config { get; }
        public IPAddress IPAddress => Config.IPAddress;
        public int Port => Config.Port;
        public bool Active { get; private set; }

        override protected bool ShouldReceiveData => Active;

        public IEnumerable<TcpClient> TcpClients => _clients.Values
            .Select(container => container.TcpClient);

        public IEnumerable<IPEndPoint> UdpEndpoints => _clients.Values
            .Select(container => container.UdpEndpoint)
            .Where(endpoint => endpoint != null);

        #region events
        public event EventHandler<ClientConnectedEventArgs> ClientConnected = delegate {};
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected = delegate {};
        public event EventHandler<UdpPortsExchangedEventArgs> UdpPortsExchanged = delegate {};
        #endregion

        TcpListener _listener;
        protected Messenger _messenger;

        protected Dictionary<int, ClientContainer> _clients = new Dictionary<int, ClientContainer>();
        int nextClientId = 0;

        bool AcceptsClients => Active;

        /// <summary>
        /// Initialize a server.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="messenger">The messenger.</param>
        public Server(ServerConfiguration config, Messenger messenger): base(config)
        {
            ValidateIPAddress(config.IPAddress);
            ValidatePort(config.Port);

            Config = config;
            _messenger = messenger;
        }

        /// <summary>
        /// Initialize a server.
        /// </summary>
        /// <param name="ipAddress">The IP address the server runs on.</param>
        /// <param name="port">The port the server listens on.</param>
        /// <param name="messenger">The messenger.</param>
        public Server(string ipAddress, ushort port, Messenger messenger): this(new ServerConfiguration()
        {
            IPAddress = IPAddress.Parse(ipAddress),
            Port = port
        }, messenger)
        {}

        /// <summary>
        /// Validate an IP address.
        /// </summary>
        /// <param name="port">The IP address to validate.</param>
        void ValidateIPAddress(IPAddress ipAddress)
        {
            if (ipAddress == null) {
                throw new ArgumentNullException("Ip address cannot be null.");
            }
        }

        /// <summary>
        /// Validate a port number. It must be an integer between 1 and 65535.
        /// </summary>
        /// <param name="port">The port to validate.</param>
        void ValidatePort(int port)
        {
            if (port < 1 || port > 65535) {
                throw new ArgumentOutOfRangeException($"Invalid port ({port}). Port must be between 1 and 65535.");
            }
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            _listener = new TcpListener(IPAddress, Port);

            _listener.Start();

            Active = true;

            Task.Run(() => AcceptClients());
        }

        /// <summary>
        /// Stop the server and remove all clients.
        /// </summary>
        public void Stop()
        {
            Active = false;

            RemoveClients();

            _listener.Stop();
        }

        /// <summary>
        /// Start accepting clients.
        /// </summary>
        async Task AcceptClients()
        {
            while (AcceptsClients) {
                try {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    ClientContainer container = new ClientContainer(client);

                    _clients[nextClientId] = container;
                    nextClientId++;

                    Task.Run(() => ReceiveData(client));
                    Task.Run(() => SendUdpPortTo(container));

                    ClientConnected(this, new ClientConnectedEventArgs(container));
                } catch (ObjectDisposedException e) {}
            }
        }

        /// <summary>
        /// Determine if a TCP client is handled by the server.
        /// </summary>
        /// <param name="client">The TCP client.</param>
        public bool ContainsClient(TcpClient client) => TcpClients.Contains(client);

        /// <summary>
        /// Remove the clients from the server.
        /// </summary>
        public void RemoveClients()
        {
            for (int i = 0; i < _clients.Count; i++) {
                RemoveClient(_clients[i]);
            }
        }

        /// <summary>
        /// Remove a TCP client from the server.
        /// </summary>
        /// <param name="client">The TCP client.</param>
        public void RemoveClient(ClientContainer client)
        {
            try {
                client.TcpClient.GetStream().Close();
                client.TcpClient.Close();
            } catch (Exception e) {
                return;
            }

            int id = -1;

            foreach (KeyValuePair<int, ClientContainer> entry in _clients) {
                if (entry.Value != client)
                    continue;

                id = entry.Key;
                break;
            }

            if (id > -1) {
                _clients.Remove(id);
            }

            ClientDisconnected(this, new ClientDisconnectedEventArgs(client));
        }

        /// <summary>
        /// Send a UDP port message containing the server's local UDP port to a client.
        /// </summary>
        /// <param name="client">The client container.</param>
        /// <returns>The sent bytes.</returns>
        async Task<byte[]> SendUdpPortTo(ClientContainer client)
            => await SendTo<ServerUdpPortMessage>(client, new ServerUdpPortMessage(client.SecretToken, NetworkConfig.LocalUdpPort));

        /// <summary>
        /// Register a client's local UDP port.
        /// </summary>
        /// <param name="message">The UDP port message.</param>
        public void RegisterClientUdpPort(UdpPortMessage message)
        {
            foreach (ClientContainer client in _clients.Values) {
                if (client.SecretToken != message.Secret)
                    continue;
                
                client.UdpEndpoint = (IPEndPoint)client.TcpClient.Client.RemoteEndPoint;
                client.UdpEndpoint.Port = message.Port;

                Task.Run(() => ReceiveData(_udpClient, client.UdpEndpoint));

                UdpPortsExchanged(this, new UdpPortsExchangedEventArgs(client));

                return;
            }
        }

        /// <summary>
        /// Send data to the clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        async public Task Send(byte[] data, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    await Task.WhenAll(UdpEndpoints.Select(
                        endpoint => _messenger.SendBytes(_udpClient, endpoint, data)
                    )); break;

                case ProtocolType.Tcp:
                default:
                    await Task.WhenAll(TcpClients.Select(
                        client => _messenger.SendBytes(client.GetStream(), data)
                    )); break;
            }
        }

        /// <summary>
        /// Send data to a specific client and return the sent bytes.
        /// </summary>
        /// <param name="client">The recipient client container.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(ClientContainer client, byte[] data, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    return await SendTo(client.UdpEndpoint, data);

                case ProtocolType.Tcp:
                default:
                    return await SendTo(client.TcpClient, data);
            }
        }

        /// <summary>
        /// Send data to a specific client and return the sent bytes.
        /// </summary>
        /// <param name="client">The recipient.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(TcpClient client, byte[] data)
            => await _messenger.Send(client.GetStream(), data);
        
        /// <summary>
        /// Send data to a specific endpoint over the UDP client and return the sent bytes.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(IPEndPoint recipient, byte[] data)
            => await _messenger.Send(_udpClient, recipient, data);
        
        /// <summary>
        /// Send data to specific clients.
        /// </summary>
        /// <param name="clients">The recipients.</param>
        /// <param name="data">The data to send.</param>
        async public Task SendTo(IEnumerable<TcpClient> clients, byte[] data)
        {
            await Task.WhenAll(clients.Select(
                client => _messenger.Send(client.GetStream(), data)
            ));
        }

        /// <summary>
        /// Send data to specific recipients over the UDP client.
        /// </summary>
        /// <param name="recipients">The recipients.</param>
        /// <param name="data">The data to send.</param>
        async public Task SendTo(IEnumerable<IPEndPoint> recipients, byte[] data)
        {
            await Task.WhenAll(recipients.Select(
                endpoint => _messenger.Send(_udpClient, endpoint, data)
            ));
        }

        /// <summary>
        /// Send a packet to the clients.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        async public Task Send(IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    await Task.WhenAll(UdpEndpoints.Select(
                        endpoint => _messenger.SendPacket(_udpClient, endpoint, packet)
                    )); break;

                case ProtocolType.Tcp:
                default:
                    await Task.WhenAll(TcpClients.Select(
                        client => _messenger.SendPacket(client.GetStream(), packet)
                    )); break;
            }
        }

        /// <summary>
        /// Send a packet to a specific client and return the sent bytes.
        /// </summary>
        /// <param name="client">The recipient client container.</param>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(ClientContainer client, IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    return await SendTo(client.UdpEndpoint, packet);

                case ProtocolType.Tcp:
                default:
                    return await SendTo(client.TcpClient, packet);
            }
        }

        /// <summary>
        /// Send a packet to a specific client and return the sent bytes.
        /// </summary>
        /// <param name="client">The recipient.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(TcpClient client, IPacket message)
            => await _messenger.SendPacket(client.GetStream(), message);
        
        /// <summary>
        /// Send a packet to specific endpoints over the UDP client.
        /// </summary>
        /// <param name="clients">The recipient client containers.</param>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo(IEnumerable<ClientContainer> clients, IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    await Task.WhenAll(clients.Select(
                        container => SendTo(container.UdpEndpoint, packet)
                    )); break;

                case ProtocolType.Tcp:
                default:
                    await Task.WhenAll(clients.Select(
                        container => SendTo(container.TcpClient, packet)
                    )); break;
            }
        }
        
        /// <summary>
        /// Send a packet to specific clients.
        /// </summary>
        /// <param name="clients">The recipients.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo(IEnumerable<TcpClient> clients, IPacket packet)
        {
            await Task.WhenAll(clients.Select(
                client => SendTo(client, packet)
            ));
        }

        /// <summary>
        /// Send a packet to specific endpoints over the UDP client.
        /// </summary>
        /// <param name="clients">The recipients.</param>
        /// <param name="packet">The packet to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo(IEnumerable<IPEndPoint> clients, IPacket packet)
        {
            await Task.WhenAll(clients.Select(
                endpoint => SendTo(endpoint, packet)
            ));
        }

        /// <summary>
        /// Send an object to all clients.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task Send<T>(T obj, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    await Task.WhenAll(UdpEndpoints.Select(
                        endpoint => _messenger.Send(_udpClient, endpoint, obj)
                    )); break;

                case ProtocolType.Tcp:
                default:
                    await Task.WhenAll(TcpClients.Select(
                        client => _messenger.Send(client.GetStream(), obj)
                    )); break;
            }
        }

        /// <summary>
        /// Send an object to a specific client and return the sent bytes.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="client">The recipient client container.</param>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo<T>(ClientContainer client, T obj, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    return await SendTo(client.UdpEndpoint, obj);

                case ProtocolType.Tcp:
                default:
                    return await SendTo(client.TcpClient, obj);
            }
        }

        /// <summary>
        /// Send an object to a specific client and return the sent bytes.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="client">The recipient.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo<T>(TcpClient client, T obj)
            => await _messenger.Send<T>(client.GetStream(), obj);
        
        /// <summary>
        /// Send an object to a specific endpoint over the UDP client and return the sent bytes.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo<T>(IPEndPoint recipient, T obj)
            => await _messenger.Send<T>(_udpClient, recipient, obj);
        
        /// <summary>
        /// Send an object to specific clients.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="clients">The recipient client containers.</param>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo<T>(IEnumerable<ClientContainer> clients, T obj, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    await Task.WhenAll(clients.Select(
                        container => SendTo<T>(container.UdpEndpoint, obj)
                    )); break;

                case ProtocolType.Tcp:
                default:
                    await Task.WhenAll(clients.Select(
                        container => SendTo<T>(container.TcpClient, obj)
                    )); break;
            }
        }
        
        /// <summary>
        /// Send an object to specific clients.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="clients">The recipients.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo<T>(IEnumerable<TcpClient> clients, T obj)
        {
            await Task.WhenAll(clients.Select(
                client => _messenger.Send<T>(client.GetStream(), obj)
            ));
        }

        /// <summary>
        /// Send an object to specific endpoints over the UDP client.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="clients">The recipients.</param>
        /// <param name="object">The object to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo<T>(IEnumerable<IPEndPoint> clients, T obj)
        {
            await Task.WhenAll(clients.Select(
                endpoint => _messenger.Send<T>(_udpClient, endpoint, obj)
            ));
        }
    }
}
