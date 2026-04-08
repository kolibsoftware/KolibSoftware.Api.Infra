using KolibSoftware.Api.Infra.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Utility class that provides extension methods for configuring and using the event broker services and handlers in the application host builder. This class includes methods to add event broker services, discover and register event handler types, and define queries for selecting pending events to be dispatched by the event broker service.
/// </summary>
public static class EventUtils
{

    /// <summary>
    /// Adds the necessary services and configurations for the event broker system to the application host builder. This method discovers all event handler types in assemblies marked with the EnableEventsAttribute, registers them in the dependency injection container, and sets up the event worker and event service. The generic type parameter TStore specifies the message store implementation to be used for storing and retrieving events, allowing for flexibility in choosing different storage mechanisms based on application requirements. By calling this method in the host builder configuration, developers can easily integrate the event broker system into their application and leverage its capabilities for handling events in a decoupled and scalable manner.
    /// </summary>
    /// <typeparam name="TStore"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddEvents<TStore>(this IHostApplicationBuilder builder)
        where TStore : class, IMessageStore<IEvent>
    {
        var types = EventHandlerRegistry.GetHandlerTypes();
        foreach (var type in types)
        {
            var handlers = EventHandlerRegistry.GetTypeHandlers(type);
            foreach (var handler in handlers)
                builder.Services.AddTransient(handler, type);
        }

        builder.Services.Configure<EventWorkerSettings>(builder.Configuration.GetSection("EventWorker"));
        builder.Services.AddScoped<IMessageStore<IEvent>, TStore>();
        builder.Services.AddSingleton<IMessageHandler<IEvent>, EventMessageHandler>();
        builder.Services.AddHostedService<QueueWorker<IEvent, EventWorkerSettings>>();
        builder.Services.AddScoped<IEventService, EventService>();
        return builder;
    }
}