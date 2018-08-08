using System;
using Base62;
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace GameNet
{
    public class ClientContainer
    {
        static Random _rand = new Random();
        static Base62Converter _tokenGenerator = new Base62Converter();

        public string Secret { get; }
        public DateTime JoinedAt { get; }
        public DateTime LastHeartbeat { get; private set; }

        public TcpClient TcpClient
        {
            get => _tcpClient;
            set
            {
                if (value != null) {
                    _tcpClient = value;
                }
            }
        }

        public IPEndPoint UdpEndpoint
        {
            get => _udpEndpoint;
            set
            {
                if (value != null) {
                    _udpEndpoint = value;
                }
            }
        }

        TcpClient _tcpClient;
        IPEndPoint _udpEndpoint;

        public ClientContainer(DateTime joinedAt)
        {
            JoinedAt = joinedAt;
            LastHeartbeat = JoinedAt;
        }

        public ClientContainer(TcpClient tcpClient, IPEndPoint udpEndpoint, DateTime joinedAt)
        {
            JoinedAt = joinedAt;
            LastHeartbeat = JoinedAt;
            _tcpClient = tcpClient;
            _udpEndpoint = udpEndpoint;

            byte[] buffer = new byte[8];
            _rand.NextBytes(buffer);

            string hex = BitConverter.ToString(buffer).Replace("-", string.Empty);

            Secret = _tokenGenerator.Encode(hex);
        }

        public ClientContainer(TcpClient tcpClient, DateTime joinedAt)
        {
            JoinedAt = joinedAt;
            LastHeartbeat = JoinedAt;
            _tcpClient = tcpClient;

            byte[] buffer = new byte[8];
            _rand.NextBytes(buffer);

            string hex = BitConverter.ToString(buffer).Replace("-", string.Empty);

            Secret = _tokenGenerator.Encode(hex);
        }

        /// <summary>
        /// Update the client's heartbeat.
        /// </summary>
        public void UpdateHearbeat()
            => LastHeartbeat = DateTime.Now;
    }
}