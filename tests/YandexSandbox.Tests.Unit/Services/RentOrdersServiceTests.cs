using FluentAssertions;
using Moq;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Messaging.Producers;
using YandexSandbox.Bll.Interfaces.Models;
using YandexSandbox.Bll.Interfaces.Queries;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Services;

namespace YandexSandbox.Tests.Unit.Services;

public class RentOrdersServiceTests
{
    private readonly Mock<IRentOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICarRepository> _carRepositoryMock;
    private readonly Mock<IMessageProducer<RentOrderPlacedMessage>> _messageProducerMock;
    private readonly RentOrdersService _sut;

    public RentOrdersServiceTests()
    {
        _orderRepositoryMock = new Mock<IRentOrderRepository>();
        _carRepositoryMock = new Mock<ICarRepository>();
        _messageProducerMock = new Mock<IMessageProducer<RentOrderPlacedMessage>>();
        _sut = new RentOrdersService(_orderRepositoryMock.Object, _carRepositoryMock.Object, _messageProducerMock.Object);
    }

    private static CarModel CreateCar(int id = 1) =>
        new() { Id = id, Make = "Toyota", Model = "Camry", Year = 2024, Color = "White" };

    private static RentOrderModel CreateOrder(int id = 1, int carId = 1, bool processed = false) =>
        new() { Id = id, CarId = carId, Processed = processed };

    private void SetupCarExists(int carId = 1)
    {
        _carRepositoryMock.Setup(r => r.GetByIdAsync(carId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCar(carId));
    }

    private void SetupCarFree(int carId = 1)
    {
        _orderRepositoryMock.Setup(r => r.GetActiveByCarIdAsync(carId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel?)null);
    }

    private void SetupOrderCreate(int assignedId = 10)
    {
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<RentOrderModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentOrderModel m, CancellationToken _) => new RentOrderModel
            {
                Id = assignedId, CarId = m.CarId, Processed = m.Processed
            });
    }

    [Fact]
    public async Task PlaceRentOrderAsync_WhenCarExistsAndFree_CreatesOrder()
    {
        SetupCarExists();
        SetupCarFree();
        SetupOrderCreate(assignedId: 10);

        var result = await _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 1 });

        result.Order.Id.Should().Be(10);
        result.Order.CarId.Should().Be(1);
        result.Order.Processed.Should().BeFalse();
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<RentOrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PlaceRentOrderAsync_ProducesRentOrderPlacedMessage()
    {
        SetupCarExists();
        SetupCarFree();
        SetupOrderCreate(assignedId: 10);

        await _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 1 });

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
        SetupCarExists();
        _orderRepositoryMock.Setup(r => r.GetActiveByCarIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateOrder(id: 5, carId: 1));

        var act = () => _sut.PlaceRentOrderAsync(new PlaceRentOrderCommand { CarId = 1 });

        var ex = await act.Should().ThrowAsync<CarNotAvailableException>();
        ex.Which.CarId.Should().Be(1);
    }

    [Fact]
    public async Task CheckRentOrderAsync_WhenOrderExists_ReturnsResponse()
    {
        var order = CreateOrder(id: 1, carId: 5, processed: true);
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _sut.CheckRentOrderAsync(new GetRentOrderByIdQuery { Id = 1 });

        result.Should().NotBeNull();
        result!.Order.Should().BeSameAs(order);
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
        var order = CreateOrder(id: 1, carId: 5);
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
