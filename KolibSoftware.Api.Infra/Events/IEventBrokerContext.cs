namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for an event broker context, which allows publishing events to the event broker.
/// </summary>
public interface IEventBrokerContext
{
    /// <summary>
    /// Publishes an event of type T to the event broker. The event will be stored in the event store and processed by the registered handlers for that event type. The method returns a task that completes when the event has been dispatched, but it does not wait for the handlers to finish processing the event. Any exceptions during dispatching will be logged, and the event status will be updated accordingly in the event store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DispatchAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull;
}