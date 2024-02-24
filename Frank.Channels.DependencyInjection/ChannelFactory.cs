using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

internal class ChannelFactory : IChannelFactory
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    
    /// <inheritdoc />
    public Channel<T> CreateUnboundedChannel<T>(ChannelSettings? options = null) where T : class => _cache.GetOrAdd(typeof(T).Name, name => CreateUnbounded<T>(name, options ?? new ChannelSettings())) as Channel<T> ?? throw new InvalidOperationException($"Channel<{typeof(T).Name}> not found");

    /// <inheritdoc />
    public Channel<T> CreateBoundedChannel<T>(ChannelSettings? options = null) where T : class => _cache.GetOrAdd(typeof(T).Name, name => CreateBounded<T>(name, options ?? new ChannelSettings())) as Channel<T> ?? throw new InvalidOperationException($"Channel<{typeof(T).Name}> not found");

    private static Channel<T> CreateBounded<T>(string name, ChannelSettings options) where T : class =>
        Channel.CreateBounded<T>(new BoundedChannelOptions(options.BoundedCapacity)
        {
            FullMode = options.BoundedFullMode
        });

    private static Channel<T> CreateUnbounded<T>(string arg, ChannelSettings options) where T : class =>
        Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
        {
            SingleReader = options.SingleReader,
            SingleWriter = options.SingleWriter
        });
}

public class ChannelSettings
{
    public bool SingleReader { get; set; } = true;
    
    public bool SingleWriter { get; set; } = true;
    
    public int BoundedCapacity { get; set; } = 100; 
    
    public BoundedChannelFullMode BoundedFullMode { get; set; } = BoundedChannelFullMode.Wait;
}