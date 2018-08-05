using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messages;
using GameNet.Messaging;
using System.Collections.Generic;

namespace GameNet
{
    public class GameNetFactory
    {
        public MessageTypeConfig MessageTypeConfig { get; } = new MessageTypeConfig();

        public GameNetFactory()
        {
            MessageTypeConfig.RegisterMessageType<UdpPortMessage>(new UdpPortMessage());
        }

        /// <summary>
        /// Create a game client.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <returns>The game client.</returns>
        public Client CreateClient(ushort udpPort = NetworkConfiguration.DEFAULT_UDP_PORT)
        {
            Messenger messenger = new Messenger(MessageTypeConfig);
            Client client = new Client(new NetworkConfiguration()
            {
                LocalUdpPort = udpPort
            }, messenger);

            MessageParser messageParser = new MessageParser(MessageTypeConfig, RecipientType.Client);

            client.RegisterDataHandler(messageParser);

            return client;
        }

        /// <summary>
        /// Create a game server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="tcpPort">The server's TCP port.</param>
        /// <param name="udpPort">The server's UDP port.</param>
        /// <returns>The game server.</returns>
        public Server CreateServer(IPAddress ipAddress, ushort tcpPort = ServerConfiguration.DEFAULT_PORT, ushort udpPort = NetworkConfiguration.DEFAULT_UDP_PORT)
        {
            Messenger messenger = new Messenger(MessageTypeConfig);
            Server server = new Server(new ServerConfiguration()
            {
                LocalUdpPort = udpPort,
                IPAddress = ipAddress,
                Port = tcpPort,
            }, messenger);

            MessageParser messageParser = new MessageParser(MessageTypeConfig, RecipientType.Server);

            server.RegisterDataHandler(messageParser);

            return server;
        }

        /// <summary>
        /// Create a game server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="tcpPort">The server's TCP port.</param>
        /// <param name="udpPort">The server's UDP port.</param>
        /// <returns>The game server.</returns>
        public Server CreateServer(string ipAddress, ushort tcpPort = ServerConfiguration.DEFAULT_PORT, ushort udpPort = ServerConfiguration.DEFAULT_UDP_PORT)
            => CreateServer(IPAddress.Parse(ipAddress), tcpPort, udpPort);
    }
}