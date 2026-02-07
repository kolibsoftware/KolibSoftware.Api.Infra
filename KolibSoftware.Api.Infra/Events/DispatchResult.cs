namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Represents the result of an event dispatch operation, indicating whether the event was successfully handled, partially handled, failed, or unhandled. This enum is used to provide feedback on the outcome of dispatching an event to its registered handlers, allowing for better error handling and logging in the event broker service.
/// </summary>
public enum DispatchResult
{
    /// <summary>
    /// Indicates that the event was successfully handled by at least one handler, and all handlers completed without errors.
    /// </summary>
    Success,

    /// <summary>
    /// Indicates that the event was partially handled, meaning that at least one handler processed the event successfully, but one or more handlers encountered errors during processing. This status can be used to identify events that may require further attention or retries.
    /// </summary>
    Partial,

    /// <summary>
    /// Indicates that the event dispatch operation failed, meaning that all handlers encountered errors during processing, and the event was not successfully handled. This status can be used to trigger error handling mechanisms or alerts for critical events that require immediate attention.
    /// </summary>
    Failure,

    /// <summary>
    /// Indicates that the event was not handled by any registered handlers, either because there were no handlers for the event type or because all handlers were skipped due to errors. This status can be used to identify events that may have been missed or ignored, and to trigger logging or monitoring for unhandled events.
    /// </summary>
    Unhandled
}