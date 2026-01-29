namespace KolibSoftware.Api.Infra.Events;

public interface IEventBrokerContext
{
    Task PublishAsync<T>(T? @event, CancellationToken cancellationToken = default);
}