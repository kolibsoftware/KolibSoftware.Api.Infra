namespace KolibSoftware.Api.Infra.Queues;


/// <summary>
/// Defines settings for the queue worker, such as the polling interval for retrieving messages from the queue. These settings can be configured through dependency injection and can be used to control the behavior of the <see cref="QueueWorker{TMessage}"/>. The <see cref="PollingInterval"/> property determines how frequently the worker checks for new messages in the queue, allowing for tuning based on the expected message volume and processing time. Adjusting these settings can help optimize performance and resource utilization of the queue worker.
/// </summary>
public class QueueSettings
{
    /// <summary>
    /// Gets or sets the polling interval for retrieving messages from the queue.
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
}