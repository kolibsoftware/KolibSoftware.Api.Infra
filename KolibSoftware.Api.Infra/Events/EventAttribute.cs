namespace KolibSoftware.Api.Infra.Events;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class EventAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
