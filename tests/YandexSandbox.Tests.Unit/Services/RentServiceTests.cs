using FluentAssertions;
using Moq;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Messaging;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Messaging;
using YandexSandbox.Bll.Models;
using YandexSandbox.Bll.Queries;
using YandexSandbox.Bll.Services;

namespace YandexSandbox.Tests.Unit.Services;

public class RentServiceTests
{
    private readonly Mock<IRentOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICarRepository> _carRepositoryMock;
    private readonly Mock<IMessageProducer> _messageProducerMock;
    private readonly RentService _sut;

    public RentServiceTests()
    {
        _orderRepositoryMock = new Mock<IRentOrderRepository>();
        _carRepositoryMock = new Mock<ICarRepository>();
        _messageProducerMock = new Mock<IMessageProducer>();
        _sut = new RentService(_orderRepositoryMock.Object, _carRepositoryMock.Object, _messageProducerMock.Object);
    }

    [Fact]
    public async Task PlaceRentOrderAsync_WhenCarExistsAndFree_CreatesOrderAndProducesMessage()
    {
        _carRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CarModel { Id = 1, Make = "BMW", Model = "X5", Year = 2024, Color = "Black" });
        _orderRepositoryMock.Setup(r => r.GetActiveByCarIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel?)null);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<RentOrderModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel model, CancellationToken _) =>
            {
                model.Id = 10;
                model.CreatedAt = DateTime.UtcNow;
                return model;
            });

        var result = await _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 1 });

        result.Order.Id.Should().Be(10);
        result.Order.CarId.Should().Be(1);
        result.Order.Processed.Should().BeFalse();
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<RentOrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _messageProducerMock.Verify(p => p.ProduceAsync(
            It.Is<RentOrderPlacedMessage>(m => m.OrderId == 10 && m.CarId == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PlaceRentOrderAsync_WhenCarNotFound_ThrowsCarNotFoundException()
    {
        _carRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarModel?)null);

        var act = () => _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 999 });

        var ex = await act.Should().ThrowAsync<CarNotFoundException>();
        ex.Which.CarId.Should().Be(999);
    }

    [Fact]
    public async Task PlaceRentOrderAsync_WhenCarNotFree_ThrowsCarNotAvailableException()
    {
        _carRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CarModel { Id = 1, Make = "BMW", Model = "X5", Year = 2024, Color = "Black" });
        _orderRepositoryMock.Setup(r => r.GetActiveByCarIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RentOrderModel { Id = 5, CarId = 1 });

        var act = () => _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 1 });

        var ex = await act.Should().ThrowAsync<CarNotAvailableException>();
        ex.Which.CarId.Should().Be(1);
    }

    [Fact]
    public async Task CheckRentOrderAsync_WhenOrderExists_ReturnsResponse()
    {
        var order = new RentOrderModel
        {
            Id = 1, CarId = 5, Processed = true, CreatedAt = DateTime.UtcNow
        };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _sut.CheckRentOrderAsync(new GetRentOrderByIdQuery { Id = 1 });

        result.Should().NotBeNull();
        result!.Order.Id.Should().Be(1);
        result.Order.CarId.Should().Be(5);
        result.Order.Processed.Should().BeTrue();
    }

    [Fact]
    public async Task CheckRentOrderAsync_WhenOrderNotFound_ReturnsNull()
    {
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel?)null);

        var result = await _sut.CheckRentOrderAsync(new GetRentOrderByIdQuery { Id = 999 });

        result.Should().BeNull();
    }

    [Fact]
    public async Task ProcessRentOrderAsync_WhenOrderExists_SetsProcessed()
    {
        var order = new RentOrderModel { Id = 1, CarId = 5, Processed = false };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _sut.ProcessRentOrderAsync(new ProcessRentOrderCommand { OrderId = 1 });

        result.Order.Processed.Should().BeTrue();
        _orderRepositoryMock.Verify(r => r.UpdateAsync(
            It.Is<RentOrderModel>(o => o.Id == 1 && o.Processed),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessRentOrderAsync_WhenOrderNotFound_ThrowsRentOrderNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel?)null);

        var act = () => _sut.ProcessRentOrderAsync(new ProcessRentOrderCommand { OrderId = 999 });

        var ex = await act.Should().ThrowAsync<RentOrderNotFoundException>();
        ex.Which.OrderId.Should().Be(999);
    }
}
