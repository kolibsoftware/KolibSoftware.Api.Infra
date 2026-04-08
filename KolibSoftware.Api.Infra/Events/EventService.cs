using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// A service for enqueuing events of type IEvent into a message store. This allows events to be processed asynchronously through a message queue or event bus, enabling decoupled and scalable event handling in the system. The EnqueueAsync method takes an event and adds it to the message store, which can then be consumed by event handlers that are subscribed to the specific event type. This design promotes loose coupling between event producers and consumers, allowing for greater flexibility and maintainability in the application architecture.
/// </summary>
/// <param name="messageStore"></param>
public sealed class EventService(
    IMessageStore<IEvent> messageStore
) : IEventService
{

    /// <summary>
    /// Asynchronously enqueues an event for processing. This method takes an IEvent object and a CancellationToken for handling cancellation requests. The implementation contains the logic to add the event to a message store, which can then be processed by background workers or event handlers that are subscribed to the specific event type. The asynchronous nature of this method allows for non-blocking operations, enabling efficient handling of events without tying up resources while waiting for the enqueue operation to complete. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task EnqueueAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        return messageStore.PutAsync([@event], cancellationToken);
    }
}