namespace KolibSoftware.Api.Infra.Events;

public interface IEventStore
{
    Task PutEventAsync(Event @event, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetEventsAsync(IEventQuery query, CancellationToken cancellationToken = default);
    Task PutEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default);
}