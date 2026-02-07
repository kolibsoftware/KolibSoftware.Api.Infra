namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Represents an event that can be published and handled within the application. Events are used to decouple different parts of the system, allowing for better scalability and maintainability. Each event has a unique identifier, a creation timestamp, a name, associated data, an optional handled timestamp, and a status indicating whether the event has been processed or not.
/// </summary>
/// <param name="Id"></param>
/// <param name="CreatedAt"></param>
/// <param name="Name"></param>
/// <param name="Data"></param>
/// <param name="HandledAt"></param>
/// <param name="Status"></param>
public record Event(
    Guid Id,
    DateTime CreatedAt,
    string Name,
    object Data,
    DateTime? HandledAt,
    EventStatus Status
);