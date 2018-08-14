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
        public static GameNetFactory Instance { get; } = new GameNetFactory();

        readonly Dictionary<Type, IMessageType> _messageTypes = new Dictionary<Type, IMessageType>();
        readonly Dictionary<Type, IMessageType> _clientMessageTypes = new Dictionary<Type, IMessageType>();
        readonly Dictionary<Type, IMessageType> _serverMessageTypes = new Dictionary<Type, IMessageType>();

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
        /// Register a message type for the client.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterClientMessageType<T>(IMessageType messageType)
            => _clientMessageTypes[typeof(T)] = messageType;
        
        /// <summary>
        /// Register a message type for the client.
        /// </summary>
        /// <param name="serializer">The message object serializer.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterClientMessageType<T>(IObjectSerializer serializer, IMessageHandler handler)
            => RegisterClientMessageType<T>(new MessageType<T>(serializer, handler));
        
        /// <summary>
        /// Register a message type for the server.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterServerMessageType<T>(IMessageType messageType)
            => _serverMessageTypes[typeof(T)] = messageType;
        
        /// <summary>
        /// Register a message type for the server.
        /// </summary>
        /// <param name="serializer">The message object serializer.</param>
        /// <typeparam name="T">The object type of the message.</typeparam>
        public void RegisterServerMessageType<T>(IObjectSerializer serializer, IMessageHandler handler)
            => RegisterServerMessageType<T>(new MessageType<T>(serializer, handler));

        /// <summary>
        /// Create a game client.
        /// </summary>
        /// <param name="config">The client configuration.</param>
        /// <returns>The game client.</returns>
        public Client CreateClient(ushort udpPort = NetworkConfiguration.DEFAULT_UDP_PORT)
        {
            var types = new MessageTypeConfig();
            var messenger = new Messenger(types);
            var client = new Client(new ClientConfiguration(udpPort), messenger);

            RegisterUserMessageTypes(types);

            foreach (KeyValuePair<Type, IMessageType> item in _clientMessageTypes) {
                types.RegisterMessageType(item.Key, item.Value);
            }

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
            var messenger = new Messenger(types);
            var server = new Server(new ServerConfiguration(ipAddress, tcpPort, udpPort), messenger);

            RegisterUserMessageTypes(types);

            foreach (KeyValuePair<Type, IMessageType> item in _serverMessageTypes) {
                types.RegisterMessageType(item.Key, item.Value);
            }

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
        /// Register the user registered message types in a message type config.
        /// </summary>
        /// <param name="types">The message type config.</param>
        void RegisterUserMessageTypes(MessageTypeConfig types)
        {
            foreach (KeyValuePair<Type, IMessageType> item in _messageTypes) {
                types.RegisterMessageType(item.Key, item.Value);
            }
        }
    }
}