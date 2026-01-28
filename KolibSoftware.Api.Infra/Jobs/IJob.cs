namespace KolibSoftware.Api.Infra.Jobs;

public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}