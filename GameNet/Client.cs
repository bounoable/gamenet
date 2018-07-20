using System;
using System.Net;
using GameNet.Debug;
using GameNet.Messaging;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet
{
    public class Client: Endpoint
    {
        /// <summary>
        /// Indicates if the client is currently connected to the server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Indicates if the client should receive data from the server.
        /// </summary>
        override protected bool ShouldReceiveData => Connected;

        protected Messenger messenger;
        protected IClientDebugger debugger;

        protected TcpClient server;

        /// <summary>
        /// Initialize the client for a client-server-connection.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        /// <param name="debugger">The client debugger.</param>
        public Client(Messenger messenger, IClientDebugger debugger = null)
        {
            this.messenger = messenger;
            this.debugger = debugger;
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        public void Connect(IPAddress ipAddress, int port)
        {
            if (Connected) {
                return;
            }

            ValidateIPAddress(ipAddress);
            ValidatePort(port);

            server = new TcpClient();

            try {
                server.Connect(ipAddress, port);
            } catch (Exception e) {
                Debug(ClientEvent.ConnectFailed, new {
                    Exception = e
                });

                return;
            }

            Connected = true;

            Debug(ClientEvent.Connected, new {
                Server = server
            });

            Task.Run(() => ReceiveData(server));
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="ipAddress">The server's IP address.</param>
        /// <param name="port">The server's port.</param>
        public void Connect(string ipAddress, int port) => Connect(IPAddress.Parse(ipAddress), port);

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        public void Disconnect()
        {
            if (!Connected) {
                Debug(ClientEvent.DisconnectFailed);

                return;
            }

            server.Close();
            server = null;
            Connected = false;

            Debug(ClientEvent.Disconnected);
        }

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
        void Debug(ClientEvent ev, EventData data = null)
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
        void Debug(ClientEvent ev, object data) => Debug(ev, new EventData(data));

        /// <summary>
        /// Trigger multiple debug events.
        /// </summary>
        /// <param name="ev">The event types.</param>
        /// <param name="data">The event data.</param>
        void Debug(ClientEvent[] ev, EventData data = null)
        {
            if (debugger != null) {
                foreach (ClientEvent evnt in ev) {
                    Debug(evnt, data);
                }
            }
        }

        /// <summary>
        /// Trigger multiple debug events.
        /// </summary>
        /// <param name="ev">The event types.</param>
        /// <param name="data">The event data.</param>
        void Debug(ClientEvent[] ev, object data) => Debug(ev, new EventData(data));

        /// <summary>
        /// Send data to the server and return the sent data.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <returns>The data that was sent.</returns>
        public Task<byte[]> Send(byte[] data) => messenger.Send(server.GetStream(), data);
    }
}
