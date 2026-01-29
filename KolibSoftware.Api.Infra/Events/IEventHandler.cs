namespace KolibSoftware.Api.Infra.Events;

public interface IEventHandler<T>
{
    Task HandleEventAsync(T data, CancellationToken cancellationToken = default);
}