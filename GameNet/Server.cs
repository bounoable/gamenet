using System;
using System.Net;
using System.Linq;
using GameNet.Debug;
using System.Threading;
using GameNet.Messaging;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    public class Server: Endpoint
    {
        public IPAddress IPAddress { get; }
        public int Port { get; }
        public bool Active { get; private set; }

        /// <summary>
        /// Indicates if the server should receive data from the clients.
        /// </summary>
        override protected bool ShouldReceiveData => Active;

        public IEnumerable<TcpClient> TcpClients => tcpClients.ToArray();

        protected Messenger messenger;
        IServerDebugger debugger;
        TcpListener socket;

        protected HashSet<TcpClient> tcpClients = new HashSet<TcpClient>();

        bool AcceptsClients => Active;

        /// <summary>
        /// Initialize a server.
        /// </summary>
        /// <param name="ipAddress">The IP address the server runs on.</param>
        /// <param name="port">The port the server listens on.</param>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The server debugger.</param>
        public Server(IPAddress ipAddress, int port, Messenger messenger, IServerDebugger debugger = null)
        {
            ValidateIPAddress(ipAddress);
            ValidatePort(port);

            IPAddress = ipAddress;
            Port = port;
            this.messenger = messenger;
            this.debugger = debugger;
        }

        /// <summary>
        /// Initialize a server.
        /// </summary>
        /// <param name="ipAddress">The IP address the server runs on.</param>
        /// <param name="port">The port the server listens on.</param>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The server debugger.</param>
        public Server(string ipAddress, int port, Messenger messenger, IServerDebugger debugger = null): this(IPAddress.Parse(ipAddress), port, messenger, debugger)
        {}

        /// <summary>
        /// Validate an IP address.
        /// </summary>
        /// <param name="port">The IP address to validate.</param>
        void ValidateIPAddress(IPAddress ipAddress)
        {
            if (ipAddress == null) {
                throw new ArgumentNullException("Ip address cannot be null.");
            }
        }

        /// <summary>
        /// Validate a port number. It must be an integer between 1 and 65535.
        /// </summary>
        /// <param name="port">The port to validate.</param>
        void ValidatePort(int port)
        {
            if (port < 1 || port > 65535) {
                throw new ArgumentOutOfRangeException($"Invalid port ({port}). Port must be between 1 and 65535.");
            }
        }

        /// <summary>
        /// Trigger a debug event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        void Debug(ServerEvent ev, EventData data = null)
        {
            if (debugger != null) {
                debugger.Handle(ev, data);
            }
        }

        /// <summary>
        /// Trigger a debug event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        void Debug(ServerEvent ev, object data) => Debug(ev, new EventData(data));

        /// <summary>
        /// Trigger multiple debug events.
        /// </summary>
        /// <param name="ev">The event types.</param>
        /// <param name="data">The event data.</param>
        void Debug(ServerEvent[] ev, EventData data = null)
        {
            if (debugger != null) {
                foreach (ServerEvent evnt in ev) {
                    Debug(evnt, data);
                }
            }
        }

        /// <summary>
        /// Trigger multiple debug events.
        /// </summary>
        /// <param name="ev">The event types.</param>
        /// <param name="data">The event data.</param>
        void Debug(ServerEvent[] ev, object data) => Debug(ev, new EventData(data));

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            socket = new TcpListener(IPAddress, Port);

            try {
                socket.Start();
            } catch (SocketException e) {
                Debug(ServerEvent.ServerStartFailed, new {
                    Exception = e
                });

                return;
            }

            Active = true;

            Debug(ServerEvent.ServerStarted, new {
                Server = this
            });

            Task.Run(() => AcceptClients());
        }

        /// <summary>
        /// Stop the server and remove all clients.
        /// </summary>
        public void Stop()
        {
            Active = false;

            RemoveClients();

            try {
                socket.Stop();
            } catch (SocketException e) {
                Debug(ServerEvent.ServerStopFailed, new {
                    Exception = e
                });

                return;
            }

            Debug(ServerEvent.ServerStopped, new {
                Server = this
            });
        }

        /// <summary>
        /// Start accepting clients.
        /// </summary>
        async Task AcceptClients()
        {
            while (AcceptsClients) {
                try {
                    TcpClient client = await socket.AcceptTcpClientAsync();

                    tcpClients.Add(client);

                    Task.Run(() => ReceiveDataFromClient(client));

                    Debug(new[] {
                        ServerEvent.ClientConnected,
                        ServerEvent.TcpClientConnected
                    }, new {
                        Client = client,
                        ConntectedAt = DateTime.Now
                    });
                } catch (ObjectDisposedException e) {}
            }
        }

        /// <summary>
        /// Determine if a TCP client is handled by the server.
        /// </summary>
        /// <param name="client">The TCP client.</param>
        public bool ContainsClient(TcpClient client) => tcpClients.Contains(client);

        /// <summary>
        /// Remove a TCP client from the server.
        /// </summary>
        /// <param name="client">The TCP client.</param>
        public void RemoveClient(TcpClient client)
        {
            client.GetStream().Close();
            client.Close();
            tcpClients.Remove(client);
        }

        /// <summary>
        /// Remove the clients from the server.
        /// </summary>
        public void RemoveClients()
        {
            var tcpClients = new TcpClient[this.tcpClients.Count];

            this.tcpClients.CopyTo(tcpClients);

            foreach (TcpClient client in tcpClients) {
                RemoveClient(client);
            }
        }

        /// <summary>
        /// Start receiving data from a client.
        /// </summary>
        /// <param name="client"></param>
        Task ReceiveDataFromClient(TcpClient client) => ReceiveData(client);

        /// <summary>
        /// Send data to the clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        async public Task Send(byte[] data)
        {
            await Task.WhenAll(tcpClients.Select(
                client => messenger.Send(client.GetStream(), data)
            ));
        }
    }
}