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

        public string SecretToken { get; }

        TcpClient _tcpClient;
        IPEndPoint _udpEndpoint;

        public ClientContainer()
        {}

        public ClientContainer(TcpClient tcpClient = null, IPEndPoint udpEndpoint = null)
        {
            _tcpClient = tcpClient;
            _udpEndpoint = udpEndpoint;

            byte[] buffer = new byte[8];
            _rand.NextBytes(buffer);

            string hex = BitConverter.ToString(buffer).Replace("-", string.Empty);

            SecretToken = _tokenGenerator.Encode(hex);
        }
    }
}