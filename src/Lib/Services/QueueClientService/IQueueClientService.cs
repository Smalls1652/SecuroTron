using Azure.Storage.Queues;

namespace SecuroTron.Lib.Services;

/// <summary>
/// Interface for services that interact with Azure Storage Queues.
/// </summary>
public interface IQueueClientService
{
    QueueClient QueueClient { get; }
}