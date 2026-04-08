namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for handling events. Implementations of this interface should provide logic to process the event when an event is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
/// </summary>
public interface IEventHandler
{

    /// <summary>
    /// Handles the event. This method is called when an event is published. The implementation should contain the logic to process the event, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="@event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the contract for handling events of a specific type. Implementations of this interface should provide logic to process the event when an event of type T is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEventHandler<T> : IEventHandler
{
    /// <summary>
    /// Handles the event of type T. This method is called when an event data of the specified type is published. The implementation should contain the logic to process the event data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(T data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explicit implementation of the non-generic HandleEventAsync method from the IEventHandler interface. This method casts the input to the specific type T and calls the generic HandleEventAsync method. This allows for a unified handling mechanism while still providing type safety for event handlers that implement the generic interface. The explicit implementation ensures that when the method is called through an IEventHandler reference, it will correctly route to the type-specific handling logic defined in the generic interface.
    /// </summary>
    /// <param name="@event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IEventHandler.HandleAsync(IEvent @event, CancellationToken cancellationToken) => HandleAsync((T)@event.Data, cancellationToken);
}