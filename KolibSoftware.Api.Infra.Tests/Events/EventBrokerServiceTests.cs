using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KolibSoftware.Api.Infra.Events;

[assembly: EnableEvents]

namespace KolibSoftware.Api.Infra.Tests.Events;

public class EventBrokerServiceTests
{
    private readonly Mock<IEventStore> _eventStoreMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<ILogger<EventBrokerService>> _loggerMock = new();
    private readonly Mock<IServiceScope> _scopeMock = new();
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new();

    public EventBrokerServiceTests()
    {
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_scopeFactoryMock.Object);

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IEventStore)))
            .Returns(_eventStoreMock.Object);

    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessPendingEvents_AndSetStatusToSuccess()
    {
        var settings = Options.Create(new EventBrokerSettings
        {
            Delay = TimeSpan.FromMilliseconds(10),
            Threshold = TimeSpan.Zero
        });

        var testEvent = new Event(
            Id: Guid.NewGuid(),
            Name: "TestData",
            CreatedAt: DateTime.UtcNow.AddMinutes(-10),
            Data: new TestData("Hello"),
            Status: EventStatus.Pending,
            HandledAt: null
        );

        _eventStoreMock
            .Setup(x => x.GetEventsAsync(It.IsAny<IEventQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Event> { testEvent });

        var handlerMock = new Mock<IEventHandler<TestData>>();
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IEnumerable<IEventHandler<TestData>>)))
            .Returns(new List<IEventHandler<TestData>> { handlerMock.Object });

        var sut = new EventBrokerService(_serviceProviderMock.Object, settings, _loggerMock.Object);

        using var cts = new CancellationTokenSource();
        var serviceTask = sut.StartAsync(cts.Token);

        await Task.Delay(100);
        await sut.StopAsync(cts.Token);

        handlerMock.Verify(x => x.HandleEventAsync(
            It.Is<TestData>(d => d.Message == "Hello"),
            It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);

        _eventStoreMock.Verify(x => x.PutEventsAsync(
            It.Is<IEnumerable<Event>>(evs => evs.First().Status == EventStatus.Success),
            It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }
}

[Event]
public record TestData(string Message);

[EventHandler]
public class TestHandler : IEventHandler<TestData>
{
    public Task HandleEventAsync(TestData data, CancellationToken cancellationToken) => Task.CompletedTask;
}