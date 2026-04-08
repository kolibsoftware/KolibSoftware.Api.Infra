using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Implements the <see cref="ITaskService"/> interface to provide functionality for enqueuing tasks. This service uses an underlying message store to manage the storage and retrieval of tasks, allowing for asynchronous processing. The EnqueueAsync method takes a task and an optional cancellation token, and it adds the task to the message store for later processing by a worker. This design allows for decoupling the task production from its consumption, enabling scalable and efficient task processing in the application.
/// </summary>
/// <param name="messageStore"></param>
public sealed class TaskService(
    IMessageStore<ITask> messageStore
) : ITaskService
{

    /// <summary>
    /// Asynchronously enqueues a task for processing. This method takes an ITask object and a CancellationToken for handling cancellation requests. The implementation contains the logic to add the task to a message store, which can then be processed by a background worker or task processor. The asynchronous nature of this method allows for non-blocking operations, enabling efficient handling of tasks without tying up resources while waiting for the enqueue operation to complete. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task EnqueueAsync(ITask task, CancellationToken cancellationToken = default)
    {
        return messageStore.PutAsync([task], cancellationToken);
    }
}