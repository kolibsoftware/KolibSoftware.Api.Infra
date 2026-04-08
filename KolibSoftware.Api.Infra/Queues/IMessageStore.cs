namespace KolibSoftware.Api.Infra.Queues;

/// <summary>
/// Defines a store for messages of type <typeparamref name="TMessage"/> in a queue. This interface abstracts the underlying storage mechanism for the queue, allowing for different implementations (e.g., in-memory, database, distributed cache) to be used without changing the logic of the queue worker or message handlers. The <see cref="QueueWorker{TMessage}"/> relies on this interface to retrieve messages for processing and to put messages back into the queue after processing. Implementations should ensure thread-safety and efficient retrieval and storage of messages, especially in high-throughput scenarios.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IMessageStore<TMessage>
{

    /// <summary>
    /// Puts a collection of messages back into the queue. This method is called by the <see cref="QueueWorker{TMessage}"/> after processing messages to either re-queue messages that failed processing or to acknowledge successful processing by removing messages from the queue. Implementations should handle the logic for determining whether messages are re-queued or removed based on the processing outcome, and should ensure that messages are not lost in case of failures.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PutAsync(IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of messages from the queue for processing. This method is called by the <see cref="QueueWorker{TMessage}"/> at regular intervals defined by the worker's polling mechanism. Implementations should ensure that messages are retrieved in a way that allows for concurrent processing by multiple workers if necessary, and should handle any necessary locking or transaction management to prevent message duplication or loss. The method should return an empty collection if there are no messages available for processing.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TMessage>> GetAsync(CancellationToken cancellationToken = default);
}