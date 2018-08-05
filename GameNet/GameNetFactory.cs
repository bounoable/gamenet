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
        readonly Dictionary<Type, IMessageType> _messageTypes = new Dictionary<Type, IMessageType>();

        /// <summary>
        /// Register a message type for both the server and the client.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterMessageType<T>(IMessageType messageType)
            => _messageTypes[typeof(T)] = messageType;

        /// <summary>
        /// Create a game client.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <returns>The game client.</returns>
        public Client CreateClient(ushort udpPort = NetworkConfiguration.DEFAULT_UDP_PORT)
        {
            var types = new MessageTypeConfig();
            var messenger = new Messenger(types);
            var client = new Client(new NetworkConfiguration(udpPort), messenger);

            RegisterDefaultClientMessageTypes(client, types);
            RegisterUserMessageTypes(types);

            var messageParser = new MessageParser(types, RecipientType.Client);

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
            var types = new MessageTypeConfig();
            var messenger = new Messenger(types);
            var server = new Server(new ServerConfiguration(ipAddress, tcpPort, udpPort), messenger);

            RegisterDefaultServerMessageTypes(server, types);
            RegisterUserMessageTypes(types);

            MessageParser messageParser = new MessageParser(types, RecipientType.Server);

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
        
        /// <summary>
        /// Register the default message types for a client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="types">The message type config.</param>
        void RegisterDefaultClientMessageTypes(Client client, MessageTypeConfig types)
        {
            types.RegisterMessageType<ClientUdpPortMessage>(ClientUdpPortMessage.TypeId, ClientUdpPortMessage.TypeForClient());
            types.RegisterMessageType<ServerUdpPortMessage>(ServerUdpPortMessage.TypeId, ServerUdpPortMessage.TypeForClient(client));
        }

        /// <summary>
        /// Register the default message types for a server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="types">The message type config.</param>
        void RegisterDefaultServerMessageTypes(Server server, MessageTypeConfig types)
        {
            types.RegisterMessageType<ServerUdpPortMessage>(ServerUdpPortMessage.TypeId, ServerUdpPortMessage.TypeForServer());
            types.RegisterMessageType<ClientUdpPortMessage>(ClientUdpPortMessage.TypeId, ClientUdpPortMessage.TypeForServer(server));
        }

        /// <summary>
        /// Register the user registered message types in a message type config.
        /// </summary>
        /// <param name="types">The message type config.</param>
        void RegisterUserMessageTypes(MessageTypeConfig types)
        {
            foreach (KeyValuePair<Type, IMessageType> entry in _messageTypes) {
                types.RegisterMessageType(entry.Key, entry.Value);
            }
        }
    }
}