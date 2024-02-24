using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

internal interface IChannelFactory
{
    Channel<T> CreateUnboundedChannel<T>(ChannelSettings? options = null) where T : class;
    
    Channel<T> CreateBoundedChannel<T>(ChannelSettings? options = null) where T : class;
}