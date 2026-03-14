using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Models;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Models;

namespace YandexSandbox.Bll.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _repository;

    public CarService(ICarRepository repository)
    {
        _repository = repository;
    }

    public async Task<CarDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await _repository.GetByIdAsync(id, cancellationToken);
        return car is null ? null : MapToDto(car);
    }

    public async Task<IReadOnlyList<CarDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cars = await _repository.GetAllAsync(cancellationToken);
        return cars.Select(MapToDto).ToList();
    }

    public async Task<CarDto> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Make))
            throw new ArgumentException("Make is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Model))
            throw new ArgumentException("Model is required.", nameof(request));

        if (request.Year < 1886 || request.Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Year is out of valid range.", nameof(request));

        var entity = new Car
        {
            Make = request.Make.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            Color = request.Color.Trim(),
            Mileage = request.Mileage,
            Vin = request.Vin?.Trim()
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    private static CarDto MapToDto(Car car) => new()
    {
        Id = car.Id,
        Make = car.Make,
        Model = car.Model,
        Year = car.Year,
        Color = car.Color,
        Mileage = car.Mileage,
        Vin = car.Vin,
        CreatedAt = car.CreatedAt
    };
}
