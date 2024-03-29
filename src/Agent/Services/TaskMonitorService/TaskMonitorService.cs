using System.Text;

using Azure;
using Azure.Storage.Queues.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SecuroTron.Agent.Services.TaskQueues;
using SecuroTron.Lib.Services;

namespace SecuroTron.Agent.Services;

/// <summary>
/// Service for orchestrating and monitoring background tasks.
/// </summary>
public class TaskMonitorService : ITaskMonitorService
{
    private readonly IQueueClientService _queueClientService;
    private readonly IActiveDirectoryService _activeDirectoryService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger _logger;

    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskMonitorService"/> class.
    /// </summary>
    /// <param name="queueClientService">The <see cref="IQueueClientService"/>.</param>
    /// <param name="activeDirectoryService">The <see cref="IActiveDirectoryService"/>.</param>
    /// <param name="taskQueue">The <see cref="IBackgroundTaskQueue"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="appLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    public TaskMonitorService(IQueueClientService queueClientService, IActiveDirectoryService activeDirectoryService, IBackgroundTaskQueue taskQueue, ILogger<TaskMonitorService> logger, IHostApplicationLifetime appLifetime)
    {
        _queueClientService = queueClientService;
        _activeDirectoryService = activeDirectoryService;
        _taskQueue = taskQueue;
        _logger = logger;

        _cancellationToken = appLifetime.ApplicationStopping;
    }

    /// <summary>
    /// Starts the monitoring loop.
    /// </summary>
    public void StartLoop()
    {
        Task.Run(async () => await MonitorAsync());
    }

    /// <summary>
    /// Monitors the queue for new messages.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    private async ValueTask MonitorAsync()
    {
        _logger.LogInformation("Monitoring the queue for new messages.");

        while (!_cancellationToken.IsCancellationRequested)
        {
            Response<QueueMessage[]> queueMessages = await _queueClientService.QueueClient.ReceiveMessagesAsync(
                maxMessages: 32,
                cancellationToken: _cancellationToken
            );

            if (queueMessages.Value.Length == 0)
            {
                //_logger.LogWarning("No messages received from the queue.");
            }
            else
            {
                _logger.LogInformation("Received {Count} messages from the queue.", queueMessages.Value.Length);

                foreach (QueueMessage message in queueMessages.Value)
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(
                        async (cancellationToken) => await ProcessQueueMessage(message, cancellationToken)
                    );
                }
            }

            await Task.Delay(3000, _cancellationToken);
        }
    }

    /// <summary>
    /// Processes a queue message.
    /// </summary>
    /// <param name="message">The <see cref="QueueMessage"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    private async ValueTask ProcessQueueMessage(QueueMessage message, CancellationToken cancellationToken)
    {
        string messageBody = Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()));

        _logger.LogInformation("Processing message {MessageId}.", message.MessageId);

        _activeDirectoryService.DisableUser(messageBody);

        await _queueClientService.QueueClient.DeleteMessageAsync(
            messageId: message.MessageId,
            popReceipt: message.PopReceipt,
            cancellationToken: _cancellationToken
        );

        _logger.LogInformation("Successfully processed message {MessageId}.", message.MessageId);
    }
}
