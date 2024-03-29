namespace SecuroTron.Lib.Services;

/// <summary>
/// Options for configuring <see cref="QueueClientService"/>.
/// </summary>
public sealed class QueueClientServiceOptions
{
    /// <summary>
    /// The URI of the Azure Storage Queue endpoint.
    /// </summary>
    public Uri? EndpointUri { get; set; } = null!;

    /// <summary>
    /// The name of the Azure Storage Queue.
    /// </summary>
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// The connection string for the Azure Storage Queue.
    /// </summary>
    public string? ConnectionString { get; set; }
}