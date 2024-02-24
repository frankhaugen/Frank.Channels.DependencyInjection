using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Channels.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an unbounded channel of type <typeparamref name="T"/> to the service collection, with default settings.
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
    public static IServiceCollection AddChannel<T>(this IServiceCollection services) where T : class =>
        services.AddChannel<T>(ChannelType.Unbounded, new ChannelSettings());

    /// <summary>
    /// Adds a channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <param name="channelType">The type of channel to add.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    /// <remarks>
    /// This method adds a channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// It allows specifying the channel type (unbounded or bounded) and optional settings for the channel.
    /// After the channel is added, it will be available for injection as a singleton instance of Channel{T}.
    /// The respective ChannelReader{T} and ChannelWriter{T} instances are also added as singletons.
    /// </remarks>
    public static IServiceCollection AddChannel<T>(this IServiceCollection services, ChannelType channelType) where T : class =>
        services.AddChannel<T>(channelType, new ChannelSettings());

    /// <summary>
    /// Adds an unbounded channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    public static IServiceCollection AddUnboundedChannel<T>(this IServiceCollection services) where T : class =>
        services.AddChannel<T>(ChannelType.Unbounded, new ChannelSettings());

    /// <summary>
    /// Adds an unbounded channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    public static IServiceCollection AddUnboundedChannel<T>(this IServiceCollection services, ChannelSettings settings) where T : class =>
        services.AddChannel<T>(ChannelType.Unbounded, settings);

    /// <summary>
    /// Adds a bounded channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    public static IServiceCollection AddBoundedChannel<T>(this IServiceCollection services) where T : class =>
        services.AddChannel<T>(ChannelType.Bounded, new ChannelSettings());

    /// <summary>
    /// Adds a bounded channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <param name="settings">The settings for the channel.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    public static IServiceCollection AddBoundedChannel<T>(this IServiceCollection services, ChannelSettings settings) where T : class =>
        services.AddChannel<T>(ChannelType.Bounded, settings);

    /// <summary>
    /// Adds a channel of type <typeparamref name="T"/> to the IServiceCollection.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    /// <param name="services">The IServiceCollection to add the channel to.</param>
    /// <param name="channelType">The type of channel to add.</param>
    /// <param name="settings">The settings for the channel.</param>
    /// <returns>The same instance of the IServiceCollection after the channel has been added.</returns>
    public static IServiceCollection AddChannel<T>(this IServiceCollection services, ChannelType channelType, ChannelSettings settings) where T : class =>
        services
            .ThrowIfContains<Channel<T>>()
            .AddSingletonIfNotExists<IChannelFactory, ChannelFactory>()
            .AddSingleton<Channel<T>>(provider =>  channelType switch
            {
                ChannelType.Unbounded => provider.GetRequiredService<IChannelFactory>().CreateUnboundedChannel<T>(settings),
                ChannelType.Bounded => provider.GetRequiredService<IChannelFactory>().CreateBoundedChannel<T>(settings),
                _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, null)
            })
            .AddSingleton<ChannelReader<T>>(provider => provider.GetRequiredService<Channel<T>>().Reader)
            .AddSingleton<ChannelWriter<T>>(provider => provider.GetRequiredService<Channel<T>>().Writer);

    private static IServiceCollection AddSingletonIfNotExists<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
    {
        if (!services.Contains<TService>())
            services.AddSingleton<TService, TImplementation>();
        return services;
    }
    
    private static IServiceCollection ThrowIfContains<TService>(this IServiceCollection services) where TService : class
    {
        if (services.Contains<TService>())
            throw new InvalidOperationException($"Service of type {typeof(TService).Name} already exists in the service collection.");
        return services;
    }
    
    private static bool Contains<TService>(this IServiceCollection services) 
        => services.Any(descriptor => descriptor.ServiceType == typeof(TService));
}