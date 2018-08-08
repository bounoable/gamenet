using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messages;
using GameNet.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    public class Client: Endpoint
    {
        /// <summary>
        /// The client configuration.
        /// </summary>
        public ClientConfiguration Config { get; }

        /// <summary>
        /// Indicates if the client is currently connected to the server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The client's session secret.
        /// </summary>
        public string Secret { get; set; }

        protected IPEndPoint ServerUdpEndpoint { get; private set; }

        /// <summary>
        /// Indicates if the client should receive data from the server.
        /// </summary>
        override protected bool ShouldReceiveData => Connected;

        protected Messenger _messenger;
        protected TcpClient _tcpServer = new TcpClient();

        /// <summary>
        /// Initialize the client for a client-server-connection.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="messenger">The messenger.</param>
        public Client(ClientConfiguration config, Messenger messenger): base(config)
        {
            if (messenger == null) {
                throw new ArgumentNullException("messenger");
            }

            Config = config;
            _messenger = messenger;
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        public void Connect(IPAddress ipAddress, int port)
        {
            if (Connected) {
                return;
            }

            ValidateIPAddress(ipAddress);
            ValidatePort(port);

            _tcpServer.Connect(ipAddress, port);

            Connected = true;

            Task.Run(() => ReceiveData(_tcpServer).ConfigureAwait(false));
            Task.Run(() => _messenger.RequestPendingAcknowledgeResponses().ConfigureAwait(false));
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        public void Connect(string ipAddress, int port) => Connect(IPAddress.Parse(ipAddress), port);

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        public void Disconnect()
        {
            if (!Connected) {
                return;
            }

            _messenger.StopRequestPendingAcknowlegeResponses();
            _tcpServer.Close();
            _tcpServer = null;
            _udpClient.Close();
            Connected = false;
        }

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
        /// Periodically send a heartbeat message to the server.
        /// </summary>
        async public Task BeginHeartbeatMessages()
        {
            while (Connected && !string.IsNullOrEmpty(Secret)) {
                await SendHeartbeatMessage();
                await Task.Delay(Config.HeartbeatInterval);
            }
        }

        /// <summary>
        /// Send a still connected message to the server.
        /// </summary>
        Task<byte[]> SendHeartbeatMessage()
            => Send(new ClientSystemMessage(ClientSystemMessage.MessageType.Heartbeat, Secret));

        /// <summary>
        /// Register the server's UDP port.
        /// </summary>
        /// <param name="message">The UDP port message from the server.</param>
        public void RegisterServerUdpPort(IUdpPortMessage message)
        {
            ServerUdpEndpoint = (IPEndPoint)_tcpServer.Client.RemoteEndPoint;
            ServerUdpEndpoint.Port = message.Port;

            Task.Run(() => ReceiveData(_udpClient, ServerUdpEndpoint).ConfigureAwait(false));
            Task.Run(() => SendUdpPortToServer().ConfigureAwait(false));
        }

        /// <summary>
        /// Send the client's local UDP port to the server.
        /// </summary>
        /// <returns>The sent bytes.</returns>
        async Task<byte[]> SendUdpPortToServer()
            => await Send(new UdpPortMessage<Client>(NetworkConfig.LocalUdpPort, Secret));
        
        /// <summary>
        /// Send data to the server and return the sent data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The data that was sent.</returns>
        async public Task<byte[]> Send(byte[] data, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    if (ServerUdpEndpoint == null)
                        return new byte[0];

                    return await _messenger.SendBytes(_udpClient, ServerUdpEndpoint, data);

                case ProtocolType.Tcp:
                default:
                    return await _messenger.SendBytes(_tcpServer.GetStream(), data);
            }
        }

        /// <summary>
        /// Send a packet to the server and return the sent data.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send(IPacket packet, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    return _messenger.SendPacket(_udpClient, ServerUdpEndpoint, packet);

                case ProtocolType.Tcp:
                default:
                    return _messenger.SendPacket(_tcpServer.GetStream(), packet);
            }
        }

        /// <summary>
        /// Send an object to the server and return the written data.
        /// The object will automatically be serialized to a message by the registered serializer.
        /// </summary>
        /// <param name="recipient">The recipient. Usually something like a TCP client.</param>
        /// <param name="object">The object to send.</param>
        /// <param name="protocol">The protocol to use.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send<T>(T obj, ProtocolType protocol = ProtocolType.Tcp)
        {
            switch (protocol) {
                case ProtocolType.Udp:
                    return _messenger.Send<T>(_udpClient, ServerUdpEndpoint, obj);

                case ProtocolType.Tcp:
                default:
                    return _messenger.Send<T>(_tcpServer.GetStream(), obj);
            }
        }
    }
}
