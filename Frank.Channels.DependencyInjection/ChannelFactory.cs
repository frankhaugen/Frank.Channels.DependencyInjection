using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

internal class ChannelFactory : IChannelFactory
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    
    /// <inheritdoc />
    public Channel<T> CreateUnboundedChannel<T>(ChannelSettings? options = null) where T : class => (Channel<T>)_cache.GetOrAdd(typeof(T).Name, name => CreateUnbounded<T>(name, options ?? new ChannelSettings()));

    /// <inheritdoc />
    public Channel<T> CreateBoundedChannel<T>(ChannelSettings? options = null) where T : class => (Channel<T>)_cache.GetOrAdd(typeof(T).Name, name => CreateBounded<T>(name, options ?? new ChannelSettings()));

    private static Channel<T> CreateBounded<T>(string name, ChannelSettings options) where T : class =>
        Channel.CreateBounded<T>(new BoundedChannelOptions(options.BoundedCapacity)
        {
            FullMode = options.BoundedFullMode,
            SingleReader = options.SingleReader,
            SingleWriter = options.SingleWriter
        });

    private static Channel<T> CreateUnbounded<T>(string arg, ChannelSettings options) where T : class =>
        Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
        {
            SingleReader = options.SingleReader,
            SingleWriter = options.SingleWriter
        });
}