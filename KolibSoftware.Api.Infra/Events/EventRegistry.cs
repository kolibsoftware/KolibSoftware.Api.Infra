using System.Reflection;

namespace KolibSoftware.Api.Infra.Events;

public static class EventRegistry
{
    private static readonly Dictionary<string, Type> _eventTypes;
    private static readonly Dictionary<Type, string> _eventNames;

    static EventRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableEventsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EventAttribute>() != null)
            .Select(t => new
            {
                Name = t.GetCustomAttribute<EventAttribute>()?.Name ?? t.Name,
                Type = t
            });
        _eventTypes = pairs.ToDictionary(p => p.Name, p => p.Type);
        _eventNames = pairs.ToDictionary(p => p.Type, p => p.Name);
    }

    public static string? GetEventName(Type type) => _eventNames.GetValueOrDefault(type);
    public static Type? GetEventType(string name) => _eventTypes.GetValueOrDefault(name);

    public static IEnumerable<string> GetEventNames() => _eventTypes.Keys;
    public static IEnumerable<Type> GetEventTypes() => _eventNames.Keys;
}