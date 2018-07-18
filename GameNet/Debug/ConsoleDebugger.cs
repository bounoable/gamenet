using System;

namespace GameNet.Debug
{
    public class ConsoleDebugger: ServerDebugger
    {
        public ConsoleDebugger()
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
        }
    }
}