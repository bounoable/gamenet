using System;
using System.Linq;
using GameNet.Support;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GameNet.Protocol
{
    public class TcpRecipient: StreamRecipient
    {
        public TcpRecipient(TcpClient client): base(client.GetStream())
        {}
    }
}