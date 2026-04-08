namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the contract for a task service that allows enqueuing tasks for asynchronous processing. Implementations of this interface should provide logic to add tasks to a queue or message store, which can then be processed by a background worker or task processor. The EnqueueAsync method is responsible for accepting a task and an optional cancellation token, allowing for graceful cancellation of the enqueue operation if needed. This interface promotes a decoupled architecture where task producers and consumers can operate independently, with the task service serving as the bridge between them.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Asynchronously enqueues a task for processing. This method takes an ITask object and a CancellationToken for handling cancellation requests. The implementation should contain the logic to add the task to a queue or message store, which can then be processed by a background worker or task processor. The asynchronous nature of this method allows for non-blocking operations, enabling efficient handling of tasks without tying up resources while waiting for the enqueue operation to complete. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task EnqueueAsync(ITask task, CancellationToken cancellationToken = default);
}