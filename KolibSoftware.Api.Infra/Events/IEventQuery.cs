namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for querying events from the event store. Implementations of this interface should provide logic to filter and retrieve events based on specific criteria, such as event type, status, or time range. The Apply method takes an IQueryable of Event objects and returns a filtered IQueryable based on the implemented query logic.
/// </summary>
public interface IEventQuery
{
    /// <summary>
    /// Applies the query logic to the provided IQueryable of Event objects. This method should implement the necessary filtering criteria to return only the events that match the query conditions. The resulting IQueryable can then be used to retrieve the relevant events from the event store.
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    IQueryable<Event> Apply(IQueryable<Event> events);
}