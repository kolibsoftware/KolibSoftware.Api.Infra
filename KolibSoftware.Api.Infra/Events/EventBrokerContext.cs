using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Events;

public class EventBrokerContext(
    IServiceProvider serviceProvider,
    IEventStore eventStore,
    ILogger<EventBrokerContext> logger
) : IEventBrokerContext
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull
    {
        var eventName = EventRegistry.GetEventName(typeof(T)) ?? throw new InvalidOperationException($"Event type {typeof(T).FullName} is not registered in BusEventRegistry.");
        var _event = new Event(
            Id: Guid.CreateVersion7(),
            Name: eventName,
            CreatedAt: DateTime.UtcNow,
            Data: @event,
            Status: EventStatus.Pending,
            HandledAt: null
        );
        await eventStore.PutEventAsync(_event, cancellationToken);
        var handlers = serviceProvider.GetServices<IEventHandler<T>>().ToList();
        var successCount = 0;
        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleEventAsync(@event, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                var handlerTypeName = handler?.GetType().FullName ?? "UnknownHandler";
                logger.LogErrorHandlingEvent(eventName, handlerTypeName, ex);
            }
        }
        _event = _event with
        {
            Status = successCount == handlers.Count
            ? EventStatus.Success
            : successCount > 0
                ? EventStatus.Partial
                : EventStatus.Failure,
            HandledAt = DateTime.UtcNow
        };
        await eventStore.PutEventAsync(_event, cancellationToken);
    }
}