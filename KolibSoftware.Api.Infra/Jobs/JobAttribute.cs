namespace KolibSoftware.Api.Infra.Jobs;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class JobAttribute(
    string? name = null,
    string? interval = null,
    string? schedule = null
) : Attribute
{
    public string? Name { get; } = name;
    public TimeSpan? Interval { get; } = interval is not null ? TimeSpan.Parse(interval) : null;
    public TimeOnly? Schedule { get; } = schedule is not null ? TimeOnly.Parse(schedule) : null;
}