using Moq;
using Microsoft.Extensions.Logging;
using KolibSoftware.Api.Infra.Events;

namespace KolibSoftware.Api.Infra.Tests.Events;

public class EventBrokerContextTests
{
    private readonly Mock<IEventStore> _eventStoreMock = new();
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<ILogger<EventBrokerContext>> _loggerMock = new();

    [Fact]
    public async Task PublishAsync_ShouldStoreEventTwice_AndSucceedWhenHandlersWork()
    {
        var @event = new TestData("Order123");

        var handlerMock = new Mock<IEventHandler<TestData>>();
        handlerMock
            .Setup(h => h.HandleEventAsync(It.IsAny<TestData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestData>>)))
            .Returns(new List<IEventHandler<TestData>> { handlerMock.Object });

        var sut = new EventBrokerContext(
            _serviceProviderMock.Object,
            _eventStoreMock.Object,
            _loggerMock.Object);

        await sut.PublishAsync(@event);

        _eventStoreMock.Verify(s => s.PutEventAsync(
            It.Is<Event>(e => e.Name == "TestData" && e.Status == EventStatus.Pending),
            It.IsAny<CancellationToken>()), Times.Once);

        handlerMock.Verify(h => h.HandleEventAsync(@event, It.IsAny<CancellationToken>()), Times.Once);

        _eventStoreMock.Verify(s => s.PutEventAsync(
            It.Is<Event>(e => e.Name == "TestData" && e.Status == EventStatus.Success),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_ShouldSetStatusToFailure_WhenHandlerThrows()
    {
        var @event = new TestData("Order123");
        var handlerMock = new Mock<IEventHandler<TestData>>();
        handlerMock
            .Setup(h => h.HandleEventAsync(It.IsAny<TestData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Boom!"));

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IEventHandler<TestData>>)))
            .Returns(new List<IEventHandler<TestData>> { handlerMock.Object });

        var sut = new EventBrokerContext(_serviceProviderMock.Object, _eventStoreMock.Object, _loggerMock.Object);

        await sut.PublishAsync(@event);

        _eventStoreMock.Verify(s => s.PutEventAsync(
            It.Is<Event>(e => e.Status == EventStatus.Failure),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}