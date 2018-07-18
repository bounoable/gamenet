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
    public class Server
    {
        public IPAddress IPAddress { get; }
        public int Port { get; }
        public bool Active { get; private set; }

        public IEnumerable<TcpClient> TcpClients => tcpClients.ToArray();

        protected Messenger messenger;
        IServerDebugger debugger;
        TcpListener socket;

        protected HashSet<TcpClient> tcpClients = new HashSet<TcpClient>();
        HashSet<IDataHandler> dataHandlers = new HashSet<IDataHandler>();

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
        /// Register a data handler for the received data.
        /// </summary>
        /// <param name="handler">The data handler.</param>
        public void RegisterDataHandler(IDataHandler handler) => dataHandlers.Add(handler);

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

            AcceptClients();
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
        async void AcceptClients()
        {
            while (AcceptsClients) {
                try {
                    var client = await socket.AcceptTcpClientAsync();
                    AddClient(client);
                } catch (ObjectDisposedException e) {}
            }
        }

        /// <summary>
        /// Add a TCP client to the server.
        /// </summary>
        /// <param name="client">The TCP client.</param>
        public void AddClient(TcpClient client)
        {
            tcpClients.Add(client);

            ReceiveDataFromClient(client);

            Debug(new[] {
                ServerEvent.ClientConnected,
                ServerEvent.TcpClientConnected
            }, new {
                Client = client,
                ConntectedAt = DateTime.Now
            });
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
        void ReceiveDataFromClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            Task.Run(() => {
                while (Active && tcpClients.Contains(client)) {
                    if (stream.CanRead && stream.DataAvailable) {
                        var data = new byte[client.ReceiveBufferSize];

                        stream.Read(data, 0, client.ReceiveBufferSize);

                        HandleReceivedData(data);
                    }
                }
            });
        }

        /// <summary>
        /// Call the registered data handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        void HandleReceivedData(byte[] data)
        {
            Task.Run(() => {
                foreach (IDataHandler handler in dataHandlers) {
                    handler.Handle(data);
                }
            });
        }

        /// <summary>
        /// Send data to the clients.
        /// </summary>
        /// <param name="data">The data to send.</param>
        public void Send(byte[] data)
        {
            foreach (TcpClient client in tcpClients) {
                messenger.Send(client.GetStream(), data);
            }
        }
    }
}