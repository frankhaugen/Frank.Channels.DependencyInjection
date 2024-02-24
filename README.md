# Frank.Channels.DependencyInjection
A tiny library for having Channel&lt;T> as a Dependency Injection resource in a sane manner

___
[![GitHub License](https://img.shields.io/github/license/frankhaugen/Frank.Channels.DependencyInjection)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Frank.Channels.DependencyInjection.svg)](https://www.nuget.org/packages/Frank.Channels.DependencyInjection)
[![NuGet](https://img.shields.io/nuget/dt/Frank.Channels.DependencyInjection.svg)](https://www.nuget.org/packages/Frank.Channels.DependencyInjection)

![GitHub contributors](https://img.shields.io/github/contributors/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub Release Date - Published_At](https://img.shields.io/github/release-date/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub last commit](https://img.shields.io/github/last-commit/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub pull requests](https://img.shields.io/github/issues-pr/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub issues](https://img.shields.io/github/issues/frankhaugen/Frank.Channels.DependencyInjection)
![GitHub closed issues](https://img.shields.io/github/issues-closed/frankhaugen/Frank.Channels.DependencyInjection)
___

## Usage

```csharp
using Frank.Channels.DependencyInjection;

// Register the channel of a type as a dependency:
services.AddChannel<string>();

// Use the channel as a dependency in various ways:
var channel = provider.GetRequiredService<Channel<string>>();
var channelWriter = provider.GetRequiredService<ChannelWriter<string>>();
var channelReader = provider.GetRequiredService<ChannelReader<string>>();
```

## Advanced usage

```csharp
using Frank.Channels.DependencyInjection;

// Register the channel of a type as a dependency with a custom configuration:
services.AddChannel<string>(options =>
{
    options.BoundedCapacity = 100;
    options.FullMode = BoundedChannelFullMode.Wait;
    options.SingleReader = true;
    options.SingleWriter = true;
});

// Use the channel as a dependency in various ways:
var channel = provider.GetRequiredService<Channel<string>>();
var channelWriter = provider.GetRequiredService<ChannelWriter<string>>();
var channelReader = provider.GetRequiredService<ChannelReader<string>>();
```