using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Service responsible for dispatching events to their registered handlers. The EventDispatcher uses dependency injection to resolve the appropriate handlers for each event type, allowing for a flexible and extensible event handling mechanism. The dispatcher executes each handler asynchronously and aggregates the results to determine the overall outcome of the dispatch operation, providing feedback on whether the event was successfully handled, partially handled, failed, or unhandled. This design promotes loose coupling between event producers and consumers, enabling better scalability and maintainability of the application.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="logger"></param>
public sealed class EventDispatcher(IServiceScopeFactory serviceScopeFactory, ILogger? logger = null)
{

    /// <summary>
    /// Asynchronously dispatches an event to all registered handlers for the event's type. The method creates a new scope for each dispatch operation to ensure that handlers are resolved with the correct lifetime and dependencies. It executes each handler in parallel and collects the results to determine the overall dispatch outcome, which can be Success, Partial, Failure, or Unhandled. The method also includes error handling and logging to capture any exceptions that occur during the dispatch process, providing insights into potential issues with event handling. This approach allows for efficient and robust event processing within the application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DispatchResult> DispatchEventAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<T>>().ToList();
        if (handlers.Count == 0)
        {
            logger?.LogNoHandlersFound(typeof(T).Name);
            return DispatchResult.Unhandled;
        }

        var tasks = handlers.Select(handler => ExecuteHandlerAsync(handler, @event, cancellationToken));
        var results = await Task.WhenAll(tasks);
        var successCount = results.Count(result => result);

        return successCount == handlers.Count ? DispatchResult.Success :
               successCount > 0 ? DispatchResult.Partial :
               DispatchResult.Failure;
    }

    /// <summary>
    /// Executes a single event handler asynchronously, handling any exceptions that may occur during the execution. The method returns a boolean indicating whether the handler executed successfully or encountered an error. If an exception is thrown, it is caught and logged using the provided logger, including details about the event type and handler type for better diagnostics. This method ensures that individual handler failures do not disrupt the overall dispatch process, allowing other handlers to continue processing the event even if one fails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<bool> ExecuteHandlerAsync<T>(IEventHandler<T> handler, T @event, CancellationToken cancellationToken)
    {
        try
        {
            await handler.HandleEventAsync(@event, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogErrorHandlingEvent(typeof(T).Name, handler.GetType().Name, ex);
            return false;
        }
    }
}