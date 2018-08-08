using System.Net.Sockets;
using System.Threading.Tasks;

namespace GameNet.Protocol
{
    public class TcpRecipient: IRecipient
    {
        readonly TcpClient _client;

        public TcpRecipient(TcpClient client)
            => _client = client;

        async public Task Send(byte[] data)
        {
            NetworkStream stream = _client.GetStream();
            
            if (stream.CanWrite) {
                await _client.GetStream().WriteAsync(data, 0, data.Length);
            }
        }
    }
}