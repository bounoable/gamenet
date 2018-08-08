using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GameNet.Protocol
{
    public class StreamRecipient: IRecipient
    {
        readonly Stream _stream;

        public StreamRecipient(Stream stream)
            => _stream = stream;

        async public Task Send(byte[] data)
        {
            if (_stream.CanWrite) {
                await _stream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}