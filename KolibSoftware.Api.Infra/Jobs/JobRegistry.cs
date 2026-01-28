using System.Reflection;
using KolibSoftware.Api.Infra.Jobs;

[assembly: EnableJobs]

namespace KolibSoftware.Api.Infra.Jobs;

public static class JobRegistry
{
    private static readonly Dictionary<string, Type> _jobTypes;
    
    static JobRegistry()
    {
        _jobTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableJobsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<JobAttribute>() != null)
            .ToDictionary(
                t => t.GetCustomAttribute<JobAttribute>()?.Name ?? t.Name,
                t => t
            );
    }

    public static string? GetJobName(Type type) => _jobTypes.FirstOrDefault(kv => kv.Value == type).Key;

    public static Type? GetJobType(string name) => _jobTypes.GetValueOrDefault(name);

    public static IEnumerable<Type> GetJobTypes() => _jobTypes.Values;
}
