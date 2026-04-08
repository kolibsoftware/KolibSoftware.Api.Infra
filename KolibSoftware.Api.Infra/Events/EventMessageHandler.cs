using KolibSoftware.Api.Infra.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Events;

public sealed class EventMessageHandler(
    IServiceProvider serviceProvider,
    ILogger? logger = null
) : IMessageHandler<IEvent>
{
    public async Task HandleAsync(IEvent message, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var dataType = message.Data.GetType();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(dataType);
        var handlers = scope.ServiceProvider.GetServices(handlerType).Cast<IEventHandler>();
        if (!handlers.Any())
        {
            logger?.LogWarning("No handlers registered for event type {DataType}.", dataType.FullName);
            return;
        }
        foreach (IEventHandler handler in handlers!)
        {
            try
            {
                await handler.HandleAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while handling event {DataType} with handler {HandlerType}.", dataType.FullName, handler.GetType().FullName);
            }
        }
    }
}