using FluentAssertions;
using Moq;
using YandexSandbox.Bll.Models;
using YandexSandbox.Bll.Services;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Models;

namespace YandexSandbox.Tests.Unit.Services;

public class CarServiceTests
{
    private readonly Mock<ICarRepository> _repositoryMock;
    private readonly CarService _sut;

    public CarServiceTests()
    {
        _repositoryMock = new Mock<ICarRepository>();
        _sut = new CarService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarExists_ReturnsDto()
    {
        var car = new Car
        {
            Id = 1, Make = "Toyota", Model = "Camry", Year = 2023,
            Color = "White", Mileage = 15000, Vin = "1HGBH41JXMN109186",
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Make.Should().Be("Toyota");
        result.Model.Should().Be("Camry");
        result.Year.Should().Be(2023);
        result.Color.Should().Be("White");
        result.Mileage.Should().Be(15000);
        result.Vin.Should().Be("1HGBH41JXMN109186");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCarDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Car?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCars()
    {
        var cars = new List<Car>
        {
            new() { Id = 1, Make = "Toyota", Model = "Camry", Year = 2023, Color = "White" },
            new() { Id = 2, Make = "Honda", Model = "Civic", Year = 2024, Color = "Black" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cars);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].Make.Should().Be("Toyota");
        result[1].Make.Should().Be("Honda");
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesAndReturnsDto()
    {
        var request = new CreateCarRequest
        {
            Make = "BMW", Model = "X5", Year = 2024,
            Color = "Black", Mileage = 0, Vin = "WBAPH5C55BA271043"
        };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Car entity, CancellationToken _) =>
            {
                entity.Id = 1;
                entity.CreatedAt = DateTime.UtcNow;
                return entity;
            });

        var result = await _sut.CreateAsync(request);

        result.Id.Should().Be(1);
        result.Make.Should().Be("BMW");
        result.Model.Should().Be("X5");
        result.Year.Should().Be(2024);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WithEmptyMake_ThrowsArgumentException(string? make)
    {
        var request = new CreateCarRequest { Make = make!, Model = "Civic", Year = 2024, Color = "Red" };

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Make is required.*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WithEmptyModel_ThrowsArgumentException(string? model)
    {
        var request = new CreateCarRequest { Make = "Honda", Model = model!, Year = 2024, Color = "Blue" };

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Model is required.*");
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2030)]
    public async Task CreateAsync_WithInvalidYear_ThrowsArgumentException(int year)
    {
        var request = new CreateCarRequest { Make = "Toyota", Model = "Supra", Year = year, Color = "Red" };

        var act = () => _sut.CreateAsync(request);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Year is out of valid range.*");
    }

    [Fact]
    public async Task CreateAsync_TrimsWhitespace()
    {
        var request = new CreateCarRequest
        {
            Make = "  Toyota  ", Model = "  Camry  ", Year = 2023,
            Color = "  Red  ", Vin = "  ABC123  "
        };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Car entity, CancellationToken _) => entity);

        var result = await _sut.CreateAsync(request);

        result.Make.Should().Be("Toyota");
        result.Model.Should().Be("Camry");
        result.Color.Should().Be("Red");
        result.Vin.Should().Be("ABC123");
    }
}
