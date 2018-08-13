using System.IO;
using System.Linq;
using GameNet.Support;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GameNet.Protocol
{
    public class StreamRecipient: IRecipient
    {
        readonly Stream _stream;
        readonly static ConcurrentDictionary<Stream, bool> _blockedStreams = new ConcurrentDictionary<Stream, bool>();

        public StreamRecipient(Stream stream)
            => _stream = stream;

        async public Task Send(byte[] payload)
        {
            byte[] data = new DataBuilder().Int(payload.Length).Data.Concat(payload).ToArray();

            while (_blockedStreams.ContainsKey(_stream)) {
                await Task.Delay(5);
            }

            while (!_blockedStreams.TryAdd(_stream, true)) {
                await Task.Delay(5);
            }

            while (!_stream.CanWrite) {
                await Task.Delay(5);
            }

            await _stream.WriteAsync(data, 0, data.Length);
            
            _blockedStreams.TryRemove(_stream, out bool removed);
        }
    }
}