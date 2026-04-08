using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Events;

public class EventWorkerSettings : IWorkerSettings
{
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
}