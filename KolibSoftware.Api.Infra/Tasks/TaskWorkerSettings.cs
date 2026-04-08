using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines settings for the <see cref="TaskWorker"/>, such as the polling interval for retrieving tasks from the queue. These settings can be configured through dependency injection and can be used to control the behavior of the <see cref="TaskWorker"/>. The <see cref="PollingInterval"/> property determines how frequently the worker checks for new tasks in the queue, allowing for tuning based on the expected task volume and processing time. Adjusting these settings can help optimize performance and resource utilization of the task worker.
/// </summary>
public class TaskWorkerSettings : IWorkerSettings
{
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
}