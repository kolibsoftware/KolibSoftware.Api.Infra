using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KolibSoftware.Api.Infra.Queues;

/// <summary>
/// Defines a background worker that processes messages of type <typeparamref name="TMessage"/> from a queue. The worker periodically polls the queue for new messages, processes them using the registered <see cref="IMessageHandler{TMessage}"/>, and handles any exceptions that may occur during processing. The worker uses the <see cref="IMessageStore{TMessage}"/> to retrieve messages from the queue and to put messages back into the queue after processing. The method runs in a loop that continues until the worker is stopped, allowing for continuous processing of messages from the queue. Implementations should ensure that the worker is resilient to failures and can recover gracefully in case of errors during message processing.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TOptions"></typeparam>
/// <typeparam name="TStore"></typeparam>
/// <typeparam name="THandler"></typeparam>
/// <param name="serviceProvider"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public class QueueWorker<TMessage, TOptions, TStore, THandler>(
    IServiceProvider serviceProvider,
    IOptions<TOptions> options,
    ILogger? logger
) : BackgroundService()
    where TOptions : class, IWorkerSettings
    where TStore : class, IMessageStore<TMessage>
    where THandler : class, IMessageHandler<TMessage>
{

    /// <summary>
    /// Gets the polling interval for retrieving messages from the queue, as defined in the <see cref="QueueSettings"/>. This property is used by the worker to determine how frequently it should check for new messages in the queue, allowing for tuning based on the expected message volume and processing time. Adjusting this interval can help optimize performance and resource utilization of the queue worker.
    /// </summary>
    public TimeSpan PollingInterval { get; } = options.Value.PollingInterval;

    /// <summary>
    /// Executes the background worker, which periodically polls the queue for new messages, processes them using the registered <see cref="IMessageHandler{TMessage}"/>, and handles any exceptions that may occur during processing. The worker uses the <see cref="IMessageStore{TMessage}"/> to retrieve messages from the queue and to put messages back into the queue after processing. The method runs in a loop that continues until the worker is stopped, allowing for continuous processing of messages from the queue. Implementations should ensure that the worker is resilient to failures and can recover gracefully in case of errors during message processing.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    override protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(PollingInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var messageStore = scope.ServiceProvider.GetRequiredService<TStore>();
            var messageHandler = scope.ServiceProvider.GetRequiredService<THandler>();
            var messages = await messageStore.GetAsync(stoppingToken);
            if (messages.Any())
                try
                {
                    await Task.WhenAll(messages.Select(e => messageHandler.HandleAsync(e, stoppingToken)));
                    await messageStore.PutAsync(messages, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "An error occurred while processing messages from the queue.");
                }
        }
    }

}

public class QueueWorker<TMessage, TOptions>(
    IServiceProvider serviceProvider,
    IOptions<TOptions> options,
    ILogger? logger
) : QueueWorker<TMessage, TOptions, IMessageStore<TMessage>, IMessageHandler<TMessage>>(
    serviceProvider,
    options,
    logger
)
    where TOptions : class, IWorkerSettings
;

public class QueueWorker<TMessage>(
    IServiceProvider serviceProvider,
    IOptions<IWorkerSettings> options,
    ILogger? logger
) : QueueWorker<TMessage, IWorkerSettings, IMessageStore<TMessage>, IMessageHandler<TMessage>>(
    serviceProvider,
    options,
    logger
);
