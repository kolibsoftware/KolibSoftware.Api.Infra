namespace KolibSoftware.Api.Infra.Events;

public sealed class EventBrokerSettings
{
    public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan Threshold { get; set; } = TimeSpan.FromMinutes(5);
}