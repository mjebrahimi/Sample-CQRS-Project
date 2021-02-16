using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sample.Core.Common
{
    public class ChannelQueue<TMessage> where TMessage : class
    {
        private readonly Channel<TMessage> _serviceChannel;

        public ChannelQueue()
        {
            _serviceChannel = Channel.CreateBounded<TMessage>(new BoundedChannelOptions(4000)
            {
                SingleReader = false,
                SingleWriter = false
            });
        }

        public async Task AddAsync(TMessage message, CancellationToken cancellationToken)
        {
            await _serviceChannel.Writer.WriteAsync(message, cancellationToken);
        }

        public IAsyncEnumerable<TMessage> ReadAsync(CancellationToken cancellationToken)
        {
            return _serviceChannel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
