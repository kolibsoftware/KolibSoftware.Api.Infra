using System.Reflection;

namespace KolibSoftware.Api.Infra.Events;

public static class EventHandlerRegistry
{
    private static readonly Dictionary<Type, IEnumerable<Type>> _handlerTypes;
    private static readonly Dictionary<Type, IEnumerable<Type>> _typeHandlers;

    static EventHandlerRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableEventsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EventHandlerAttribute>() != null)
            .SelectMany(t => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => new
                {
                    Handler = i,
                    Type = t
                })
            );

        _handlerTypes = pairs
            .GroupBy(p => p.Handler)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Type));

        _typeHandlers = pairs
            .GroupBy(p => p.Type)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Handler));
    }

    public static IEnumerable<Type> GetHandlerTypes() => _typeHandlers.Keys;
    public static IEnumerable<Type> GetTypeHandlers() => _handlerTypes.Keys;


    public static IEnumerable<Type> GetHandlerTypes(Type handler) => _handlerTypes.GetValueOrDefault(handler) ?? [];
    public static IEnumerable<Type> GetTypeHandlers(Type type) => _typeHandlers.GetValueOrDefault(type) ?? [];

}
