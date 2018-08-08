using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messages;
using GameNet.Protocol;
using GameNet.Messages.Handlers;
using System.Collections.Generic;
using GameNet.Messages.Serializers;

namespace GameNet
{
    public class GameNetFactory
    {
        public MessengerConfig MessengerConfig { get; } = new MessengerConfig();

        readonly Dictionary<Type, IMessageType> _messageTypes = new Dictionary<Type, IMessageType>();

        /// <summary>
        /// Register a message type for both the server and the client.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterMessageType<T>(IMessageType messageType)
            => _messageTypes[typeof(T)] = messageType;
        
        /// <summary>
        /// Register a message type for both the server and the client.
        /// </summary>
        /// <param name="serializer">The message object serializer.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterMessageType<T>(IObjectSerializer serializer, IMessageHandler handler)
            => RegisterMessageType<T>(new MessageType<T>(serializer, handler));

        /// <summary>
        /// Create a game client.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <returns>The game client.</returns>
        public Client CreateClient(ushort udpPort = NetworkConfiguration.DEFAULT_UDP_PORT)
        {
            var types = new MessageTypeConfig();
            var messenger = new Messenger(MessengerConfig, types);
            var client = new Client(new ClientConfiguration(udpPort), messenger);

            RegisterDefaultClientMessageTypes(client, types);
            RegisterUserMessageTypes(types);

            client.RegisterDataHandler(messenger);

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
            var messenger = new Messenger(MessengerConfig, types);
            var server = new Server(new ServerConfiguration(ipAddress, tcpPort, udpPort), messenger);

            RegisterDefaultServerMessageTypes(server, types);
            RegisterUserMessageTypes(types);

            server.RegisterDataHandler(messenger);

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
            types.RegisterMessageType<UdpPortMessage<Client>>(
                (int)DefaultMessageTypes.ClientUdpPort,
                new UdpPortMessageSerializer<Client>()
            );

            types.RegisterMessageType<UdpPortMessage<Server>>(
                (int)DefaultMessageTypes.ServerUdpPort,
                new UdpPortMessageSerializer<Server>(),
                new UdpPortMessageHandler<Server>(client)
            );

            types.RegisterMessageType<ServerSystemMessage>(
                (int)DefaultMessageTypes.ServerSystemMessage,
                new ServerSystemMessageSerializer(),
                new ServerSystemMessageHandler(client)
            );

            types.RegisterMessageType<ClientSystemMessage>(
                (int)DefaultMessageTypes.ClientSystemMessage,
                new ClientSystemMessageSerializer()
            );

            types.RegisterMessageType<ClientSecretMessage>(
                (int)DefaultMessageTypes.ClientSecret,
                new ClientSecretMessageSerializer(),
                new ClientSecretMessageHandler(client)
            );
        }

        /// <summary>
        /// Register the default message types for a server.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="types">The message type config.</param>
        void RegisterDefaultServerMessageTypes(Server server, MessageTypeConfig types)
        {
            types.RegisterMessageType<UdpPortMessage<Server>>(
                (int)DefaultMessageTypes.ServerUdpPort,
                new UdpPortMessageSerializer<Server>()
            );

            types.RegisterMessageType<UdpPortMessage<Client>>(
                (int)DefaultMessageTypes.ClientUdpPort,
                new UdpPortMessageSerializer<Client>(),
                new UdpPortMessageHandler<Client>(server)
            );

            types.RegisterMessageType<ClientSystemMessage>(
                (int)DefaultMessageTypes.ClientSystemMessage,
                new ClientSystemMessageSerializer(),
                new ClientSystemMessageHandler(server)
            );
            
            types.RegisterMessageType<ServerSystemMessage>(
                (int)DefaultMessageTypes.ServerSystemMessage,
                new ServerSystemMessageSerializer()
            );

            types.RegisterMessageType<ClientSecretMessage>(
                (int)DefaultMessageTypes.ClientSecret,
                new ClientSecretMessageSerializer()
            );
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