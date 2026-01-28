namespace KolibSoftware.Api.Infra.Jobs;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class JobAttribute(
    string? name = null,
    string? interval = null
) : Attribute
{
    public string? Name => name;
    public TimeSpan? Interval => interval is not null ? TimeSpan.Parse(interval) : null;
}