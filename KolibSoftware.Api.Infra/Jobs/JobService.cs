using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            await Task.WhenAll(tasksTypes.Select(jobType => DispatchTask(jobType, stoppingToken)));
        }
        catch (Exception error)
        {
            logger.LogJobServiceError(error);
        }
    }

    public async Task DispatchTask(Type jobType, CancellationToken stoppingToken = default)
    {
        var attribute = jobType.GetCustomAttribute<JobAttribute>();
        var taskName = attribute?.Name ?? JobRegistry.GetJobName(jobType) ?? jobType.Name;
        var interval = attribute?.Interval ?? configuration.GetValue<TimeSpan?>($"Jobs:{taskName}:Interval") ?? TimeSpan.FromDays(1);
        var schedule = attribute?.Schedule ?? configuration.GetValue<TimeOnly?>($"Jobs:{taskName}:Schedule") ?? TimeOnly.FromDateTime(DateTime.UtcNow);

        var now = DateTime.UtcNow;
        var firstRun = now.Date + schedule.ToTimeSpan();

        if (firstRun < now)
            firstRun = firstRun.AddDays(1);

        var delay = firstRun - now;
        logger.LogSchedulingJob(jobType, delay, interval);
        await Task.Delay(delay, stoppingToken);

        using var timer = new PeriodicTimer(interval);
        do
            try
            {
                using var scope = serviceProvider.CreateScope();
                var task = scope.ServiceProvider.GetRequiredKeyedService<IJob>(jobType);
                logger.LogExecutingJob(jobType);
                await task.ExecuteAsync(stoppingToken);
                logger.LogCompletedJob(jobType);
            }
            catch (Exception ex)
            {
                logger.LogErrorExecutingJob(jobType, ex);
            }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

}