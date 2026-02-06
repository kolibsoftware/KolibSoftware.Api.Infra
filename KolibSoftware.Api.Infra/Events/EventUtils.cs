using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Events;

public static class EventUtils
{
    public static IHostApplicationBuilder AddEvents(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<EventBrokerSettings>(builder.Configuration.GetSection("EventBroker"));
        builder.Services.AddTransient<IEventBrokerContext, EventBrokerContext>();
        builder.Services.AddHostedService<EventBrokerService>();
        var types = EventHandlerRegistry.GetHandlerTypes();
        foreach (var type in types)
        {
            var handlers = EventHandlerRegistry.GetTypeHandlers(type);
            foreach (var handler in handlers)
                builder.Services.AddTransient(handler, type);
        }
        return builder;
    }
}