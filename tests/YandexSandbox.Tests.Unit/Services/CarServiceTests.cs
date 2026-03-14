using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Messaging;
using YandexSandbox.Bll.Queries;
using YandexSandbox.Bll.Services;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Entities;

namespace YandexSandbox.Tests.Unit.Services;

public class CarServiceTests
{
    private readonly Mock<ICarRepository> _repositoryMock;
    private readonly Mock<IMessageProducer> _messageProducerMock;
    private readonly CarService _sut;

    public CarServiceTests()
    {
        _repositoryMock = new Mock<ICarRepository>();
        _messageProducerMock = new Mock<IMessageProducer>();
        var settings = Options.Create(new CarValidationSettings());
        _sut = new CarService(_repositoryMock.Object, _messageProducerMock.Object, settings);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarExists_ReturnsResponse()
    {
        var car = new CarEntity
        {
            Id = 1, Make = "Toyota", Model = "Camry", Year = 2023,
            Color = "White", Mileage = 15000, Vin = "1HGBH41JXMN109186",
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var result = await _sut.GetByIdAsync(new GetCarByIdQuery { Id = 1 });

        result.Should().NotBeNull();
        result!.Car.Id.Should().Be(1);
        result.Car.Make.Should().Be("Toyota");
        result.Car.Model.Should().Be("Camry");
        result.Car.Year.Should().Be(2023);
        result.Car.Color.Should().Be("White");
        result.Car.Mileage.Should().Be(15000);
        result.Car.Vin.Should().Be("1HGBH41JXMN109186");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarEntity?)null);

        var result = await _sut.GetByIdAsync(new GetCarByIdQuery { Id = 999 });

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCars()
    {
        var cars = new List<CarEntity>
        {
            new() { Id = 1, Make = "Toyota", Model = "Camry", Year = 2023, Color = "White" },
            new() { Id = 2, Make = "Honda", Model = "Civic", Year = 2024, Color = "Black" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars);

        var result = await _sut.GetAllAsync(new GetAllCarsQuery());

        result.Cars.Should().HaveCount(2);
        result.Cars[0].Make.Should().Be("Toyota");
        result.Cars[1].Make.Should().Be("Honda");
    }

    [Fact]
    public async Task CreateAsync_WithValidCommand_CreatesAndReturnsResponse()
    {
        var command = new CreateCarCommand
        {
            Make = "BMW", Model = "X5", Year = 2024,
            Color = "Black", Mileage = 0, Vin = "WBAPH5C55BA271043"
        };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<CarEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarEntity entity, CancellationToken _) =>
            {
                entity.Id = 1;
                entity.CreatedAt = DateTime.UtcNow;
                return entity;
            });

        var result = await _sut.CreateAsync(command);

        result.Car.Id.Should().Be(1);
        result.Car.Make.Should().Be("BMW");
        result.Car.Model.Should().Be("X5");
        result.Car.Year.Should().Be(2024);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<CarEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _messageProducerMock.Verify(p => p.ProduceAsync(
            It.Is<CarCreatedMessage>(m => m.Id == 1 && m.Make == "BMW" && m.Model == "X5"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2030)]
    public async Task CreateAsync_WithInvalidYear_ThrowsInvalidCarYearException(int year)
    {
        var command = new CreateCarCommand
        {
            Make = "Toyota", Model = "Supra", Year = year, Color = "Red"
        };

        var act = () => _sut.CreateAsync(command);

        var ex = await act.Should().ThrowAsync<InvalidCarYearException>();
        ex.Which.Year.Should().Be(year);
        ex.Which.MinYear.Should().Be(1886);
    }

    [Fact]
    public async Task CreateAsync_WithCustomMinYear_UsesConfiguredValue()
    {
        var settings = Options.Create(new CarValidationSettings { MinYear = 2000 });
        var sut = new CarService(_repositoryMock.Object, _messageProducerMock.Object, settings);
        var command = new CreateCarCommand
        {
            Make = "Ford", Model = "T", Year = 1950, Color = "Black"
        };

        var act = () => sut.CreateAsync(command);

        var ex = await act.Should().ThrowAsync<InvalidCarYearException>();
        ex.Which.MinYear.Should().Be(2000);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidVin_ThrowsInvalidVinException()
    {
        var command = new CreateCarCommand
        {
            Make = "Audi", Model = "A4", Year = 2024, Color = "Silver", Vin = "SHORT"
        };

        var act = () => _sut.CreateAsync(command);

        var ex = await act.Should().ThrowAsync<InvalidVinException>();
        ex.Which.Vin.Should().Be("SHORT");
    }

    [Fact]
    public async Task CreateAsync_WithNullVin_DoesNotThrow()
    {
        var command = new CreateCarCommand
        {
            Make = "BMW", Model = "X5", Year = 2024, Color = "Black", Vin = null
        };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<CarEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CarEntity entity, CancellationToken _) =>
            {
                entity.Id = 1;
                entity.CreatedAt = DateTime.UtcNow;
                return entity;
            });

        var result = await _sut.CreateAsync(command);

        result.Car.Vin.Should().BeNull();
    }
}
