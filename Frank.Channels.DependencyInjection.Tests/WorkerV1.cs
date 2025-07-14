using System.Threading.Channels;
using Frank.Testing.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Frank.Channels.DependencyInjection.Tests;

public class ExperimentalTest
{
	private readonly CancellationToken _cancelToken = new CancellationTokenSource(10000).Token;
	private readonly ITestOutputHelper _outputHelper;

	public ExperimentalTest(ITestOutputHelper outputHelper)
	{
		_outputHelper = outputHelper;
	}

	[Fact]
	public async Task Run()
	{
		var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
	
		builder.Logging.ClearProviders().AddSimpleTestLoggingProvider(_outputHelper);
	
		builder.Services.AddChannel<MySource>();
		builder.Services.AddChannel<MyResult>();

		builder.Services.AddSingleton<WorkerRunner<MySource, MyResult>>();

		builder.Services.AddSingleton<BirthYearCalculator>(provider => new BirthYearCalculator());
		builder.Services.AddSingleton(typeof(IConsumer<>), typeof(ConsumerWorker<>));
		builder.Services.AddSingleton(typeof(IConsumer), provider => provider.GetRequiredService<IConsumer<MySource>>());
		builder.Services.AddSingleton<IConsumer<MySource>, ConsumerWorker<MySource>>();

		builder.Services.AddHostedService<WorkerHost>();
		builder.Services.AddHostedService<ConsumerWorkerHost>();
		builder.Services.AddHostedService<TestWorkerHost>();

		var app = builder.Build();

		await app.RunAsync(_cancelToken);
	}
}

public class MySource
{
	public int Age { get; set; }
	public string Name { get; set; }
}

public class MyResult
{
	public int BirthYear { get; set; }
}

public class BirthYearCalculator : IWorker<MySource, MyResult>
{
	public Task<MyResult> RunAsync(MySource input, CancellationToken cancellationToken)
	{
		return Task.FromResult(new MyResult { BirthYear = DateTime.Now.Year - input.Age });
	}
}

public class ConsumerWorker<TIn> : IConsumer<TIn>
{
	private readonly ILogger<ConsumerWorker<TIn>> _logger;
	private readonly ChannelReader<TIn> _reader;

	public ConsumerWorker(ChannelReader<TIn> reader, ILogger<ConsumerWorker<TIn>> logger)
	{
		_reader = reader;
		_logger = logger;
	}
	
	public async Task ConsumeAsync(CancellationToken cancellationToken)
	{
		await foreach (var item in _reader.ReadAllAsync(cancellationToken))
		{
			_logger.LogInformation("Received item {item}", item);
		}
	}
}

public interface IConsumer
{
	Task ConsumeAsync(CancellationToken cancellationToken);
}

public interface IConsumer<TIn> : IConsumer
{
}


public class ConsumerWorkerHost : BackgroundService
{
	private readonly IEnumerable<IConsumer> _consumers;

	public ConsumerWorkerHost(IEnumerable<IConsumer> consumers)
	{
		_consumers = consumers;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.WhenAll(_consumers.Select(consumer => consumer.ConsumeAsync(stoppingToken)));
	}
}

public class TestWorkerHost : BackgroundService
{
	public ChannelWriter<MySource> _writer;
	public ILogger<TestWorkerHost> _logger;

	public TestWorkerHost(ChannelWriter<MySource> writer, ILogger<TestWorkerHost> logger)
	{
		_writer = writer;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var source = new MySource()
			{
				Age = Random.Shared.Next(18, 60),
				Name = "John"
			};
			await _writer.WriteAsync(source, stoppingToken);
			await Task.Delay(1000);
		}
	}
}

public class WorkerHost : BackgroundService
{
	private readonly IEnumerable<IWorkerRunner> _workerRunners;

	public WorkerHost(IEnumerable<IWorkerRunner> workerRunners)
	{
		_workerRunners = workerRunners;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.WhenAll(_workerRunners.Select(runner => runner.RunManyAsync(5, stoppingToken)));
	}
}

public interface IWorker<TIn, TOut>
{
	Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken);
}

public interface IWorkerRunner
{
	Task RunManyAsync(uint extraInstances, CancellationToken cancellationToken);
}

public class WorkerRunner<TIn, TOut>(ChannelReader<TIn> reader, ChannelWriter<TOut> writer, IWorker<TIn, TOut> worker, ILogger<WorkerRunner<TIn, TOut>> logger) : IWorkerRunner
{
	/// <inheritdoc />
	public async Task RunManyAsync(uint extraInstances, CancellationToken cancellationToken)
	{
		var tasks = new List<Task>();
		for (var i = 0; i <= extraInstances; i++)
		{
			tasks.Add(RunAsync(cancellationToken));
		}

		await Task.WhenAll(tasks);
	}

	private async Task RunAsync(CancellationToken cancellationToken)
	{
		await foreach (var item in reader.ReadAllAsync(cancellationToken))
		{
			logger.LogDebug("WorkerRunner received input {Input}", item);
			try
			{
				var result = await worker.RunAsync(item, cancellationToken);
				await writer.WriteAsync(result, cancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "WorkerRunner failed to process input {Input}", item);
			}
		}
	}
}