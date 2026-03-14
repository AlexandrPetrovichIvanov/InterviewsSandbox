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
        if (request.Year < 1886 || request.Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Year is out of valid range.", nameof(request));

        if (request.Vin is not null && request.Vin.Length != 17)
            throw new ArgumentException("VIN must be exactly 17 characters.", nameof(request));

        var entity = new Car
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Color = request.Color,
            Mileage = request.Mileage,
            Vin = request.Vin
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
