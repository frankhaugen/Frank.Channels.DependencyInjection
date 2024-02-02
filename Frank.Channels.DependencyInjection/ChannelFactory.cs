using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

public class ChannelFactory : IChannelFactory
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    
    public Channel<T> CreateChannel<T>() where T : class => _cache.GetOrAdd(typeof(T).Name, Value<T>) as Channel<T> ?? throw new InvalidOperationException($"Channel<{typeof(T).Name}> not found");

    private static Channel<T> Value<T>(string arg) where T : class =>
        Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true
        });
}
