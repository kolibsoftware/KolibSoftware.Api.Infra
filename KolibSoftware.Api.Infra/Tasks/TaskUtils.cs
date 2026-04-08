using KolibSoftware.Api.Infra.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Tasks;

public static class TaskUtils
{

    /// <summary>
    /// Adds the necessary services for processing tasks of type ITask through a queue worker. This method configures the dependency injection container to include the <see cref="TaskWorkerSettings"/>, the specified message store for ITask, the <see cref="TaskHandler"/> for processing tasks, and the <see cref="QueueWorker{TMessage, TSettings}"/> as a hosted service. By calling this method in the application startup, you can enable asynchronous task processing through a message queue, allowing for decoupled and scalable execution of tasks in the system.
    /// </summary>
    /// <typeparam name="TStore"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddTasks<TStore>(this IHostApplicationBuilder builder)
        where TStore : class, IMessageStore<ITask>
    {
        builder.Services.Configure<TaskWorkerSettings>(builder.Configuration.GetSection("TaskWorker"));
        builder.Services.AddTransient<IMessageStore<ITask>, TStore>();
        builder.Services.AddSingleton<IMessageHandler<ITask>, TaskHandler>();
        builder.Services.AddHostedService<QueueWorker<ITask, TaskWorkerSettings>>();
        return builder;
    }

}