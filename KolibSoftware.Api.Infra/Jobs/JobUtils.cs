using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Jobs;

public static class JobUtils
{

    public static IHostApplicationBuilder AddJobs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<JobService>();
        var jobTypes = JobRegistry.GetJobTypes();
        foreach (var jobType in jobTypes)
            builder.Services.AddKeyedTransient(typeof(IJob), jobType, jobType);
        return builder;
    }

}