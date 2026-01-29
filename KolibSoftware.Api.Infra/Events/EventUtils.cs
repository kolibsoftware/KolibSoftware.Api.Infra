using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Events;

public static class EventUtils
{
    public static IHostApplicationBuilder AddJobs(this IHostApplicationBuilder builder)
    {
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