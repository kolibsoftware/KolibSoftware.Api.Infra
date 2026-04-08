namespace KolibSoftware.Api.Infra.Queues;

/// <summary>
/// Defines settings for a queue worker, such as the polling interval for retrieving messages from the queue. These settings can be configured through dependency injection and can be used to control the behavior of the <see cref="QueueWorker{TMessage}"/>. The <see cref="PollingInterval"/> property determines how frequently the worker checks for new messages in the queue, allowing for tuning based on the expected message volume and processing time. Adjusting these settings can help optimize performance and resource utilization of the queue worker.
/// </summary>
public interface IWorkerSettings
{

    /// <summary>
    /// Gets the polling interval for retrieving messages from the queue, as defined in the <see cref="WorkerSettings"/>. This property is used by the worker to determine how frequently it should check for new messages in the queue, allowing for tuning based on the expected message volume and processing time. Adjusting this interval can help optimize performance and resource utilization of the queue worker.
    /// </summary>
    TimeSpan PollingInterval { get; }
}