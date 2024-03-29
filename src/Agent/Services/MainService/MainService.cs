using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecuroTron.Agent.Services.TaskQueues;

namespace SecuroTron.Agent.Services;

/// <summary>
/// The main service for the app.
/// </summary>
public sealed class MainService : IHostedService, IDisposable
{
    private bool _disposed = false;
    private Task? _executingTask;
    private Task? _monitorTask;
    private CancellationTokenSource? _cts;

    private readonly ITaskMonitorService _taskMonitorService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainService"/> class.
    /// </summary>
    /// <param name="taskMonitorService">The <see cref="ITaskMonitorService"/>.</param>
    /// <param name="taskQueue">The <see cref="IBackgroundTaskQueue"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="appLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    public MainService(ITaskMonitorService taskMonitorService, IBackgroundTaskQueue taskQueue, ILogger<MainService> logger, IHostApplicationLifetime appLifetime)
    {
        _taskMonitorService = taskMonitorService;
        _taskQueue = taskQueue;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    /// <summary>
    /// Runs the main service.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
    /// <returns></returns>
    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MainService is starting.");
        return ProcessTaskQueueAsync(cancellationToken);
    }

    /// <summary>
    /// Processes the task queue.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem =
                    await _taskQueue.DequeueAsync(cancellationToken);

                await workItem(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _executingTask = RunAsync(_cts.Token);
        _monitorTask = Task.Run(() => _taskMonitorService.StartLoop(), _cts.Token);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask is null || _monitorTask is null)
        {
            return;
        }

        try
        {
            _cts?.Cancel();
        }
        finally
        {
            await _executingTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            await _monitorTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _executingTask?.Dispose();
        _monitorTask?.Dispose();
        _cts?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}