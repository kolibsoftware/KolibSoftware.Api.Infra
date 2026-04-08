namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines a service for enqueuing events of type IEvent. This service abstracts the underlying message queue or event bus implementation, allowing for decoupled and flexible event publishing in the system. The EnqueueAsync method is used to add events to the queue, which will then be processed by the appropriate event handlers asynchronously. This design promotes a clean separation of concerns and enables scalable event-driven architecture within the application.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Enqueues an event of type IEvent for asynchronous processing. This method is called by event producers to publish events to the system. The implementation of this method should handle the logic for adding the event to the underlying message queue or event bus, ensuring that it will be delivered to the appropriate event handlers for processing. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task EnqueueAsync(IEvent @event, CancellationToken cancellationToken = default);
}