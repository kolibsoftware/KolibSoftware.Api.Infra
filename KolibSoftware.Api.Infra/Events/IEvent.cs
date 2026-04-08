namespace KolibSoftware.Api.Infra.Events;

public interface IEvent
{
    public object Data { get; }
}