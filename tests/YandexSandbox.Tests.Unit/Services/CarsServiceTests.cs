using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Messaging.Producers;
using YandexSandbox.Bll.Interfaces.Models;
using YandexSandbox.Bll.Interfaces.Queries;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Services;

namespace YandexSandbox.Tests.Unit.Services;

public class CarsServiceTests
{
    private readonly Mock<ICarRepository> _repositoryMock;
    private readonly Mock<IMessageProducer<CarCreatedMessage>> _messageProducerMock;
    private readonly CarsService _sut;

    public CarsServiceTests()
    {
        _repositoryMock = new Mock<ICarRepository>();
        _messageProducerMock = new Mock<IMessageProducer<CarCreatedMessage>>();
        var settings = Options.Create(new CarValidationSettings());
        _sut = new CarsService(_repositoryMock.Object, _messageProducerMock.Object, settings);
    }

    private static CarModel CreateCar(int id = 1, string make = "Toyota", string model = "Camry",
        int year = 2024, string color = "White", int mileage = 0, string? vin = null)
        => new() { Id = id, Make = make, Model = model, Year = year, Color = color, Mileage = mileage, Vin = vin };

    private static CreateCarCommand CreateCommand(string make = "Toyota", string model = "Camry",
        int year = 2024, string color = "White", int mileage = 0, string? vin = null)
        => new() { Make = make, Model = model, Year = year, Color = color, Mileage = mileage, Vin = vin };

    private void SetupRepositoryCreate(int assignedId = 1)
    {
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarModel m, CancellationToken _) => new CarModel
            {
                Id = assignedId, Make = m.Make, Model = m.Model, Year = m.Year,
                Color = m.Color, Mileage = m.Mileage, Vin = m.Vin
            });
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarExists_ReturnsResponse()
    {
        var car = CreateCar(id: 1, make: "Toyota", model: "Camry", mileage: 15000, vin: "1HGBH41JXMN109186");
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var result = await _sut.GetByIdAsync(new GetCarByIdQuery { Id = 1 });

        result.Should().NotBeNull();
        result!.Car.Should().BeSameAs(car);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarModel?)null);

        var result = await _sut.GetByIdAsync(new GetCarByIdQuery { Id = 999 });

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCars()
    {
        var cars = new List<CarModel> { CreateCar(id: 1), CreateCar(id: 2, make: "Honda") };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars);

        var result = await _sut.GetAllAsync(new GetAllCarsQuery());

        result.Cars.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithValidCommand_CreatesAndReturnsResponse()
    {
        SetupRepositoryCreate(assignedId: 1);

        var result = await _sut.CreateAsync(CreateCommand(make: "BMW", model: "X5"));

        result.Car.Id.Should().Be(1);
        result.Car.Make.Should().Be("BMW");
        result.Car.Model.Should().Be("X5");
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<CarModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ProducesCarCreatedMessage()
    {
        SetupRepositoryCreate(assignedId: 5);

        await _sut.CreateAsync(CreateCommand(make: "BMW", model: "X5"));

        _messageProducerMock.Verify(p => p.ProduceAsync(
            It.Is<CarCreatedMessage>(m => m.Id == 5 && m.Make == "BMW" && m.Model == "X5"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2030)]
    public async Task CreateAsync_WithInvalidYear_ThrowsInvalidCarYearException(int year)
    {
        var act = () => _sut.CreateAsync(CreateCommand(year: year));

        var ex = await act.Should().ThrowAsync<InvalidCarYearException>();
        ex.Which.Year.Should().Be(year);
        ex.Which.MinYear.Should().Be(1886);
    }

    [Fact]
    public async Task CreateAsync_WithCustomMinYear_UsesConfiguredValue()
    {
        var settings = Options.Create(new CarValidationSettings { MinYear = 2000 });
        var sut = new CarsService(_repositoryMock.Object, _messageProducerMock.Object, settings);

        var act = () => sut.CreateAsync(CreateCommand(year: 1950));

        var ex = await act.Should().ThrowAsync<InvalidCarYearException>();
        ex.Which.MinYear.Should().Be(2000);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidVin_ThrowsInvalidVinException()
    {
        var act = () => _sut.CreateAsync(CreateCommand(vin: "SHORT"));

        var ex = await act.Should().ThrowAsync<InvalidVinException>();
        ex.Which.Vin.Should().Be("SHORT");
    }

    [Fact]
    public async Task CreateAsync_WithNullVin_DoesNotThrow()
    {
        SetupRepositoryCreate();

        var result = await _sut.CreateAsync(CreateCommand(vin: null));

        result.Car.Vin.Should().BeNull();
    }
}
