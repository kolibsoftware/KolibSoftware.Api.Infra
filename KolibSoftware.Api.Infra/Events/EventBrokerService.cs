using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KolibSoftware.Api.Infra.Events;

public sealed class EventBrokerService(
    IServiceProvider serviceProvider,
    IOptions<EventBrokerSettings> options,
    ILogger<EventBrokerService> logger
) : BackgroundService(), IEventBrokerService
{

    private readonly TimeSpan Delay = options.Value.Delay;
    private readonly TimeSpan Threshold = options.Value.Threshold;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Delay);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();
            var timepoint = DateTime.UtcNow.Subtract(Threshold);
            var query = new EventQuery(Threshold);
            var events = await eventStore.GetEventsAsync(query, stoppingToken);
            if (events.Any())
                foreach (var chunk in events.Chunk(20))
                {
                    try
                    {
                        var tasks = chunk.Select(e => DispatchEvent(e, scope.ServiceProvider, stoppingToken));
                        var results = await Task.WhenAll(tasks);
                        await eventStore.PutEventsAsync(results, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogErrorDispatchingEvents(ex);
                    }
                }
        }
    }

    private async Task<Event> DispatchEvent(Event @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var eventType = EventRegistry.GetEventType(@event.Name);
        if (eventType == null)
        {
            logger.LogUnregisteredEventType(@event.Name);
            return @event with { Status = EventStatus.Failure, HandledAt = DateTime.UtcNow };
        }

        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = serviceProvider.GetServices(handlerType);
        if (!handlers.Any())
        {
            logger.LogNoHandlersFound(@event.Name);
            return @event with { Status = EventStatus.Failure, HandledAt = DateTime.UtcNow };
        }

        var successCount = 0;
        foreach (dynamic? handler in handlers)
        {
            try
            {
                await handler!.HandleEventAsync((dynamic)@event.Data, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                var handlerTypeName = (handler as object)?.GetType().FullName ?? "UnknownHandler";
                logger.LogErrorHandlingEvent(@event.Name, handlerTypeName, ex);
            }
        }

        return @event with
        {
            Status = successCount == handlers.Count()
            ? EventStatus.Success
            : successCount > 0
                ? EventStatus.Partial
                : EventStatus.Failure,
            HandledAt = DateTime.UtcNow
        };
    }

    public sealed class EventQuery(TimeSpan age) : IEventQuery
    {
        public IQueryable<Event> Apply(IQueryable<Event> query)
        {
            var timepoint = DateTime.UtcNow.Subtract(age);
            return query.Where(e => e.Status == EventStatus.Pending && e.CreatedAt <= timepoint);
        }
    }
}