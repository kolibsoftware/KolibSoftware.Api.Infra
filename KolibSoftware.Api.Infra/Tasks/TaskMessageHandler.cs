using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// A message handler that executes tasks of type ITask by calling their ExecuteAsync method. This allows tasks to be processed asynchronously through a message queue or event bus, enabling decoupled and scalable task execution in the system.
/// </summary>
public sealed class TaskMessageHandler : IMessageHandler<ITask>
{

    /// <summary>
    /// Handles a ITask message by invoking the ExecuteAsync method of the contained ITask. This method is called by the queue worker when a ITask message is retrieved from the queue. The cancellation token is passed to allow for cooperative cancellation of the task if needed. Any exceptions thrown during task execution will be handled and logged by the queue worker.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task HandleAsync(ITask message, CancellationToken cancellationToken = default)
    {
        return message.ExecuteAsync(cancellationToken);
    }
}