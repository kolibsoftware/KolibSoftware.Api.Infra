namespace KolibSoftware.Api.Infra.Queues;


/// <summary>
/// Defines a handler for processing messages of type <typeparamref name="TMessage"/> from a queue.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageHandler<TMessage>
{

    /// <summary>
    /// Handles a message of type <typeparamref name="TMessage"/>. This method is called by the <see cref="QueueWorker{TMessage}"/> when messages are retrieved from the queue. Implementations should contain the logic for processing the message and may throw exceptions if processing fails, which will be logged by the worker.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}