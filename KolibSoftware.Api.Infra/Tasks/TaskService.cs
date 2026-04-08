using KolibSoftware.Api.Infra.Queues;

namespace KolibSoftware.Api.Infra.Tasks;

public sealed class TaskService(
    IMessageStore<ITask> messageStore
) : ITaskService
{
    public Task EnqueueAsync(ITask task, CancellationToken cancellationToken = default)
    {
        return messageStore.PutAsync([task], cancellationToken);
    }
}