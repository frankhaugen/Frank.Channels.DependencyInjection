using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

public interface IChannelFactory
{
    Channel<T> CreateChannel<T>() where T : class;
}