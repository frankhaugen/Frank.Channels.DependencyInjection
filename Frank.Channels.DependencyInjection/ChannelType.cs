namespace Frank.Channels.DependencyInjection;

public enum ChannelType
{
    /// <summary>
    /// Unbounded channel has no limit on the number of items it can store.
    /// </summary>
    Unbounded,
    
    /// <summary>
    /// Bounded channel has a limit on the number of items it can store.
    /// </summary>
    Bounded
}