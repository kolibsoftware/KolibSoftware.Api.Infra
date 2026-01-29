namespace KolibSoftware.Api.Infra.Events;

public sealed class EventBrokerSettings
{
    public TimeSpan DelayInterval { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan AgeThreshold { get; set; } = TimeSpan.FromMinutes(5);
}