using System.Net;
using GameNet.Debug;
using GameNet.Messaging;
using System.Collections.Generic;

namespace GameNet
{
    public class GameNetFactory
    {
        Dictionary<int, IMessageHandler> messageHandlers = new Dictionary<int, IMessageHandler>();

        public void RegisterMessageHandler(int messageType, IMessageHandler handler) => messageHandlers[messageType] = handler;

        public GameClient CreateGameClient(IClientDebugger debugger = null)
        {
            Messenger messenger = new Messenger();
            GameClient client = new GameClient(messenger, debugger);
            MessageParser messageParser = new MessageParser(RecipientType.Client);

            foreach (KeyValuePair<int, IMessageHandler> handler in messageHandlers) {
                messageParser.RegisterHandler(handler.Key, handler.Value);
            }

            client.AddDataHandler(messageParser);

            return client;
        }

        public GameServer CreateGameServer(IPAddress ipAddress, int port, IServerDebugger debugger = null)
        {
            Messenger messenger = new Messenger();
            GameServer server = new GameServer(ipAddress, port, messenger, debugger);
            MessageParser messageParser = new MessageParser(RecipientType.Server);

            foreach (KeyValuePair<int, IMessageHandler> handler in messageHandlers) {
                messageParser.RegisterHandler(handler.Key, handler.Value);
            }

            server.AddDataHandler(messageParser);

            return server;
        }
    }
}