namespace KolibSoftware.Api.Infra.Events;

public interface IEventQuery
{
    IQueryable<Event> Apply(IQueryable<Event> events);
}