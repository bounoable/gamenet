using System;
using System.Net;
using System.Net.Sockets;

namespace GameNet.Debug
{
    public class ConsoleClientDebugger: ClientDebugger
    {
        public ConsoleClientDebugger()
        {
            RegisterHandlers();
        }

        void RegisterHandlers()
        {
            On(ClientEvent.Connected, data => {
                var server = data.Get<TcpClient>();

                IPEndPoint ep = (IPEndPoint)server.Client.RemoteEndPoint;

                Console.WriteLine($"Client connected to server {ep.Address.ToString()}:{ep.Port}");
            });

            On(ClientEvent.ConnectFailed, data => {
                var e = data.Get<Exception>();

                Console.WriteLine($"Client to server connection failed: {e.Message}");
            });

            On(ClientEvent.Disconnected, () => {
                Console.WriteLine($"Client disconnected from server.");
            });

            On(ClientEvent.DisconnectFailed, data => {
                var e = data.Get<Exception>();

                Console.WriteLine($"Client could not disconnect from server: {e.Message}");
            });
        }
    }
}