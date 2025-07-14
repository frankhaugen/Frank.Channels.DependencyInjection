using System.Threading.Channels;
using FluentAssertions;
using Frank.Testing.TestBases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace Frank.Channels.DependencyInjection.Tests;

public class ServiceCollectionExtensionsTests : HostApplicationTestBase
{
    private readonly ITestOutputHelper _outputHelper;
    public ServiceCollectionExtensionsTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    private readonly List<MyDto> _dtos = new();

    protected override Task SetupAsync(HostApplicationBuilder builder)
    {
        builder.Services.AddChannel<MyDto>();
        builder.Services.AddSingleton(_dtos);
        builder.Services.AddHostedService<MyChannelListener>();
        return Task.CompletedTask;
    }


    [Fact]
    public async Task Test1()
    {
        var myDto = new MyDto { Name = "Test" };
        var channel = GetServices.GetRequiredService<ChannelWriter<MyDto>>();

        for (int i = 0; i < 10000; i++)
        {
            await channel.WriteAsync(myDto);
        }
        
        _dtos.Should().Contain(myDto);
        _outputHelper.WriteLine($"_dtos.Count: {_dtos.Count}");
    }

    private class MyChannelListener : BackgroundService
    {
        private readonly ChannelReader<MyDto> _channelReader;
        private readonly List<MyDto> _dtos;
        
        public MyChannelListener(ChannelReader<MyDto> channelReader, List<MyDto> dtos)
        {
            _channelReader = channelReader;
            _dtos = dtos;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                var dto = await _channelReader.ReadAsync(stoppingToken);
                _dtos.Add(dto);
            }
        }
    }
}