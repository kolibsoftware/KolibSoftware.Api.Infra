using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KolibSoftware.Api.Infra.Jobs;

public sealed class JobService(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<JobService> logger
) : BackgroundService(), IJobService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var tasksTypes = JobRegistry.GetJobTypes();
            await Task.WhenAll(tasksTypes.Select(taskType => DispatchTask(taskType, stoppingToken)));
        }
        catch (Exception error)
        {
            logger.LogError(error, "Job Service encountered an error");
        }
    }

    public async Task DispatchTask(Type taskType, CancellationToken stoppingToken = default)
    {
        var attribute = taskType.GetCustomAttribute<JobAttribute>();
        var taskName = attribute?.Name ?? JobRegistry.GetJobName(taskType) ?? taskType.Name;
        var interval = attribute?.Interval ?? configuration.GetValue<TimeSpan?>($"Jobs:{taskName}") ?? throw new InvalidOperationException($"No interval configured for job {taskName}");

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            try
            {
                using var scope = serviceProvider.CreateScope();
                var task = scope.ServiceProvider.GetRequiredKeyedService<IJob>(taskType);
                logger.LogInformation("Executing job {TaskType}", taskType.FullName);
                await task.ExecuteAsync(stoppingToken);
                logger.LogInformation("Completed job {TaskType}", taskType.FullName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing job {TaskType}", taskType.FullName);
            }
    }

}