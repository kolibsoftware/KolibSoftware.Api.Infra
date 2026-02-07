using Moq;
using KolibSoftware.Api.Infra.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KolibSoftware.Api.Infra.Jobs.Attributes;

namespace KolibSoftware.Api.Infra.Tests.Jobs;

public class JobServiceTests
{
    [Fact]
    public async Task TestDispatchTask()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<JobService>>();
        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        var jobMock = new Mock<IJob>();

        serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);
        scopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(scopeMock.Object);

        var keyedProviderMock = serviceProviderMock.As<IKeyedServiceProvider>();
        keyedProviderMock
            .Setup(x => x.GetRequiredKeyedService(typeof(IJob), It.IsAny<Type>()))
            .Returns(jobMock.Object);

        scopeMock
            .Setup(x => x.ServiceProvider)
            .Returns(serviceProviderMock.Object);

        var jobType = typeof(TestJob);
        configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);

        var mockIntervalSection = new Mock<IConfigurationSection>();
        mockIntervalSection.Setup(s => s.Value).Returns("00:00:00.010");

        configurationMock
            .Setup(c => c.GetSection($"Jobs:{nameof(TestJob)}:Interval"))
            .Returns(mockIntervalSection.Object);

        var service = new JobService(serviceProviderMock.Object, configurationMock.Object, loggerMock.Object);

        var cts = new CancellationTokenSource();
        var dispatchTask = service.DispatchTask(jobType, cts.Token);

        await Task.Delay(100);
        cts.Cancel();

        jobMock.Verify(x => x.ExecuteAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
    }
}

[Job]
public class TestJob : IJob
{
    public Task ExecuteAsync(CancellationToken ct) => Task.CompletedTask;
}
