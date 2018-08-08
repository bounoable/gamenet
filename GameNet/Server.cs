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

        public IEnumerable<TcpClient> TcpClients => _players.Values
            .Select(container => container.TcpClient);

        public IEnumerable<IPEndPoint> UdpEndpoints => _players.Values
            .Select(container => container.UdpEndpoint)
            .Where(endpoint => endpoint != null);

        #region events
        public event EventHandler<PlayerConnectedEventArgs> PlayerConnected = delegate {};
        public event EventHandler<PlayerDisconnectedEventArgs> PlayerDisconnected = delegate {};
        public event EventHandler<UdpPortsExchangedEventArgs> UdpPortsExchanged = delegate {};
        #endregion

        TcpListener _listener;
        protected Messenger _messenger;

        protected Dictionary<int, Player> _players = new Dictionary<int, Player>();
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

            Task.Run(() => AcceptClients().ConfigureAwait(false));
            Task.Run(() => KickDisconnectedPlayers().ConfigureAwait(false));
            Task.Run(() => _messenger.RequestPendingAcknowledgeResponses().ConfigureAwait(false));
        }

        /// <summary>
        /// Stop the server and remove all clients.
        /// </summary>
        public void Stop()
        {
            Active = false;

            RemovePlayers();

            _messenger.StopRequestPendingAcknowlegeResponses();
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
                    Player player = new Player(client, DateTime.Now);

                    _players[nextClientId] = player;
                    nextClientId++;

                    Task.Run(() => ReceiveData(client).ConfigureAwait(false));

                    Task.Run(async () => {
                        await SendSecretTo(player);
                        await SendUdpPortTo(player);
                    });

                    PlayerConnected(this, new PlayerConnectedEventArgs(player));
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
        public void RemovePlayers()
        {
            for (int i = 0; i < _players.Count; i++) {
                RemovePlayer(_players[i]);
            }
        }

        /// <summary>
        /// Remove a TCP client from the server.
        /// </summary>
        /// <param name="player">The player.</param>
        public void RemovePlayer(Player player)
        {
            try {
                player.TcpClient.GetStream().Close();
                player.TcpClient.Close();
            } catch (Exception e) {
                return;
            }

            int id = -1;

            foreach (KeyValuePair<int, Player> entry in _players) {
                if (entry.Value != player)
                    continue;

                id = entry.Key;
                break;
            }

            if (id > -1) {
                _players.Remove(id);
            }
        }

        /// <summary>
        /// Begin the kicking of non-responding players.
        /// </summary>
        async public Task KickDisconnectedPlayers()
        {
            while (Active) {
                await Task.Delay(5000);

                DateTime now = DateTime.Now;
                var removePlayers = new HashSet<Player>();

                foreach (Player player in _players.Values) {
                    if (player.LastHeartbeat.AddMilliseconds(Config.HeartbeatTimeout) < now) {
                        removePlayers.Add(player);
                    }
                }

                foreach (Player player in removePlayers) {
                    RemovePlayer(player);
                    PlayerDisconnected(this, new PlayerDisconnectedEventArgs(player));

                    Console.WriteLine($"Player disconnected: {player.Secret}");
                }
            }
        }

        /// <summary>
        /// Find a client by it's secret.
        /// </summary>
        /// <param name="secret">The client secret.</param>
        /// <returns>The player.</returns>
        public Player GetPlayerBySecret(string secret)
        {
            foreach (Player client in _players.Values) {
                if (client.Secret == secret) {
                    return client;
                }
            }

            return null;
        }

        async Task<byte[]> SendSecretTo(Player client)
            => await SendTo(client, new ClientSecretMessage(client.Secret));
        
        /// <summary>
        /// Send a UDP port message containing the server's local UDP port to a client.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The sent bytes.</returns>
        async Task<byte[]> SendUdpPortTo(Player player)
            => await SendTo(player, new UdpPortMessage<Server>(NetworkConfig.LocalUdpPort));

        /// <summary>
        /// Register a client's local UDP port.
        /// </summary>
        /// <param name="message">The UDP port message.</param>
        public void RegisterClientUdpPort(IUdpPortMessage message)
        {
            foreach (Player player in _players.Values) {
                if (player.Secret != message.Secret)
                    continue;
                
                player.UdpEndpoint = (IPEndPoint)player.TcpClient.Client.RemoteEndPoint;
                player.UdpEndpoint.Port = message.Port;

                Task.Run(() => ReceiveData(_udpClient, player.UdpEndpoint).ConfigureAwait(false));

                UdpPortsExchanged(this, new UdpPortsExchangedEventArgs(player));

                return;
            }
        }

        /// <summary>
        /// Notify the server about a received heartbeat from a client.
        /// </summary>
        /// <param name="clientSecret">The client's secret.</param>
        public void NotifyHeartbeat(string clientSecret)
            => GetPlayerBySecret(clientSecret)?.UpdateHearbeat();

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
        /// <param name="client">The recipient player.</param>
        /// <param name="data">The data to send.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(Player client, byte[] data, ProtocolType protocol = ProtocolType.Tcp)
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
        /// <param name="client">The recipient player.</param>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo(Player client, IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
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
        /// <param name="clients">The recipient players.</param>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo(IEnumerable<Player> clients, IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
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
        /// <param name="client">The recipient player.</param>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task<byte[]> SendTo<T>(Player client, T obj, ProtocolType protocol = ProtocolType.Tcp)
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
        /// <param name="clients">The recipient players.</param>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The sent bytes.</returns>
        async public Task SendTo<T>(IEnumerable<Player> clients, T obj, ProtocolType protocol = ProtocolType.Tcp)
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
