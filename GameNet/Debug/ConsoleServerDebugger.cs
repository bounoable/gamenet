using System;
using System.Net;
using System.Net.Sockets;

namespace GameNet.Debug
{
    public class ConsoleServerDebugger: ServerDebugger
    {
        public ConsoleServerDebugger()
        {
            RegisterHandlers();
        }

        void RegisterHandlers()
        {
            On(ServerEvent.ServerStarted, data => {
                var server = data.Get<Server>();

                Console.WriteLine($"Server has started on {server.IPAddress.ToString()}:{server.Port}");
            });

            On(ServerEvent.ServerStartFailed, data => {
                var e = data.Get<Exception>();

                Console.WriteLine($"Server start has failed: {e.Message}");
            });

            On(ServerEvent.ServerStopped, () => {
                Console.WriteLine($"Server has stopped.");
            });

            On(ServerEvent.ServerStopFailed, data => {
                var e = data.Get<Exception>();

                Console.WriteLine($"Server stop has failed: {e.Message}");
            });

            On(ServerEvent.ClientConnected, data => {
                var client = data.Get<TcpClient>();

                IPEndPoint ep = (IPEndPoint)client.Client.RemoteEndPoint;

                Console.WriteLine($"Client has connected: {ep.Address.ToString()}:{ep.Port}");
            });
        }
    }
}