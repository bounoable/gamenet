using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messaging;
using System.Collections.Generic;

namespace GameNet
{
    public class GameNetFactory
    {
        MessageTypeConfig MessageTypeConfig { get; } = new MessageTypeConfig();

        /// <summary>
        /// Create a GameClient.
        /// </summary>
        /// <param name="debugger">The client debugger.</param>
        /// <returns>The GameClient.</returns>
        public GameClient CreateGameClient(IClientDebugger debugger = null)
        {
            Messenger messenger = new Messenger(MessageTypeConfig);
            GameClient client = new GameClient(messenger, debugger);
            MessageParser messageParser = new MessageParser(MessageTypeConfig, RecipientType.Client);

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
            Messenger messenger = new Messenger(MessageTypeConfig);
            GameServer server = new GameServer(ipAddress, port, messenger, debugger);
            MessageParser messageParser = new MessageParser(MessageTypeConfig, RecipientType.Server);

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