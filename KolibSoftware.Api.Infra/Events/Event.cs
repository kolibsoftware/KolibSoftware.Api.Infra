namespace KolibSoftware.Api.Infra.Events;

public record Event(
    Guid Id,
    DateTime CreatedAt,
    string Name,
    object Data,
    DateTime? HandledAt,
    EventStatus Status
);