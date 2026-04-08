namespace KolibSoftware.Api.Infra.Tasks;

public interface ITaskService
{
    Task EnqueueAsync(ITask task, CancellationToken cancellationToken = default);
}