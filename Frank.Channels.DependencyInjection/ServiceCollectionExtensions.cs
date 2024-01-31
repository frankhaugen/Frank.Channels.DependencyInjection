using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Channels.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal static bool Contains<TService>(this IServiceCollection services)
    {
        return services.Any(descriptor => descriptor.ServiceType == typeof(TService));
    }
    
    internal static IServiceCollection AddSingletonIfNotExists<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
    {
        if (!services.Contains<TService>())
            services.AddSingleton<TService, TImplementation>();
        return services;
    }
    
    /// <summary>
    /// Adds a channel of type <typeparamref name="T"/> to the service collection.
    /// </summary>
    /// <remarks>
    /// The channel is added as a singleton with its reader and writer as singletons, and injected as follows:
    /// <list type="bullet">
    /// <item><description><see cref="Channel{T}"/></description></item>
    /// <item><description><see cref="ChannelReader{T}"/></description></item>
    /// <item><description><see cref="ChannelWriter{T}"/></description></item>
    /// </list>
    /// </remarks>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddChannel<T>(this IServiceCollection services) where T : class
    {
        services.AddSingletonIfNotExists<IChannelFactory, ChannelFactory>();
        services.AddSingleton<Channel<T>>(provider => provider.GetRequiredService<IChannelFactory>().CreateChannel<T>());
        services.AddSingleton<ChannelReader<T>>(provider => provider.GetRequiredService<Channel<T>>().Reader);
        services.AddSingleton<ChannelWriter<T>>(provider => provider.GetRequiredService<Channel<T>>().Writer);
        return services;
    }
}