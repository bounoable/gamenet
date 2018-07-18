using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messaging;
using System.Collections.Generic;

namespace GameNet
{
    public class GameNetFactory
    {
        Dictionary<Type, IMessageHandler> messageHandlers = new Dictionary<Type, IMessageHandler>();

        /// <summary>
        /// Register a message handler for the servers and clients.
        /// </summary>
        /// <param name="handler">The message handler.</param>
        public void RegisterMessageType<TMessage>(IMessageHandler handler) => messageHandlers[typeof(TMessage)] = handler;

        /// <summary>
        /// Create a GameClient.
        /// </summary>
        /// <param name="debugger">The client debugger.</param>
        /// <returns>The GameClient.</returns>
        public GameClient CreateGameClient(IClientDebugger debugger = null)
        {
            Messenger messenger = new Messenger();
            GameClient client = new GameClient(messenger, debugger);
            MessageParser messageParser = new MessageParser(RecipientType.Client);

            foreach (KeyValuePair<Type, IMessageHandler> handler in messageHandlers) {
                messageParser.RegisterMessageType(handler.Key, handler.Value);
            }

            client.RegisterDataHandler(messageParser);

            return client;
        }

        /// <summary>
        /// Create a GameServer.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        /// <param name="debugger">The server debugger.</param>
        /// <returns>The GameServer.</returns>
        public GameServer CreateGameServer(IPAddress ipAddress, int port, IServerDebugger debugger = null)
        {
            Messenger messenger = new Messenger();
            GameServer server = new GameServer(ipAddress, port, messenger, debugger);
            MessageParser messageParser = new MessageParser(RecipientType.Server);

            foreach (KeyValuePair<Type, IMessageHandler> handler in messageHandlers) {
                messageParser.RegisterMessageType(handler.Key, handler.Value);
            }

            server.RegisterDataHandler(messageParser);

            return server;
        }

        /// <summary>
        /// Create a GameServer.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        /// <param name="debugger">The server debugger.</param>
        /// <returns>The GameServer.</returns>
        public GameServer CreateGameServer(string ipAddress, int port, IServerDebugger debugger = null)
            => CreateGameServer(IPAddress.Parse(ipAddress), port, debugger);
    }
}