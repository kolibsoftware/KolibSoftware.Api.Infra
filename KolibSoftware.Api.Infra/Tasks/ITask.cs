namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines a contract for a task that can be executed asynchronously. Implementations of this interface represent units of work that can be scheduled and run by a task scheduler. The ExecuteAsync method is the entry point for executing the task's logic, and it accepts a CancellationToken to allow for cooperative cancellation of the task if needed. This interface is designed to be implemented by classes that are decorated with the [Task] attribute, which allows them to be discovered and managed by the task scheduling system in the application.
/// </summary>
public interface ITask
{
    /// <summary>
    /// Executes the task's logic asynchronously. This method is called by the task scheduler when it's time to run the task. Implementations should contain the core logic of the task and should respect the cancellation token to allow for graceful shutdowns. If the task encounters an error during execution, it can throw exceptions, which will be handled and logged by the task scheduler. The asynchronous nature of this method allows for non-blocking execution, making it suitable for tasks that involve I/O operations or other long-running processes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}