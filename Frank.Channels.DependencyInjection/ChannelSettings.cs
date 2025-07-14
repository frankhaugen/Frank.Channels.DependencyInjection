using System.Threading.Channels;

namespace Frank.Channels.DependencyInjection;

public class ChannelSettings
{
    public bool SingleReader { get; set; } = true;
    
    public bool SingleWriter { get; set; } = true;
    
    public int BoundedCapacity { get; set; } = 100; 
    
    public BoundedChannelFullMode BoundedFullMode { get; set; } = BoundedChannelFullMode.Wait;
}