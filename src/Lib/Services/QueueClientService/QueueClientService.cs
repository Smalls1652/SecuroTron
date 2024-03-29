using Azure.Identity;
using Azure.Storage.Queues;

using Microsoft.Extensions.Options;

namespace SecuroTron.Lib.Services;

/// <summary>
/// Service for interacting with Azure Storage Queues.
/// </summary>
public sealed class QueueClientService : IQueueClientService
{
    private readonly QueueClientServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueClientService"/> class.
    /// </summary>
    /// <param name="options">Options for configuring the service.</param>
    public QueueClientService(IOptions<QueueClientServiceOptions> options)
    {
        _options = options.Value;

        if (_options.ConnectionString is not null)
        {
            QueueClient = new(
                connectionString: _options.ConnectionString,
                queueName: _options.QueueName
            );
        }
        else
        {
            QueueClient = new(
                queueUri: _options.EndpointUri,
                credential: new ChainedTokenCredential(
                    [
                        new AzureCliCredential(),
                    new AzurePowerShellCredential(),
                    new ManagedIdentityCredential()
                    ]
                )
            );
        }
    }

    /// <summary>
    /// The <see cref="QueueClient"/> instance.
    /// </summary>
    public QueueClient QueueClient { get; }
}