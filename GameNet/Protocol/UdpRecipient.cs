using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GameNet.Protocol
{
    public class UdpRecipient: IRecipient
    {
        readonly UdpClient _client;
        readonly IPEndPoint _endpoint;

        public UdpRecipient(UdpClient client, IPEndPoint endpoint)
        {
            _client = client;
            _endpoint = endpoint;
        }

        async public Task Send(byte[] data)
            => await _client.SendAsync(data, data.Length, _endpoint);
    }
}