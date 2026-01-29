namespace KolibSoftware.Api.Infra.Events;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class EventHandlerAttribute : Attribute;