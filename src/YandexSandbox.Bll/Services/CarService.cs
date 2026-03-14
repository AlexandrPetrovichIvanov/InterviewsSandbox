using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.CommonModels;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Queries;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Entities;

namespace YandexSandbox.Bll.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _repository;

    public CarService(ICarRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCarByIdQueryResponse?> GetByIdAsync(GetCarByIdQuery query, CancellationToken cancellationToken = default)
    {
        var car = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return car is null ? null : new GetCarByIdQueryResponse { Car = MapToModel(car) };
    }

    public async Task<GetAllCarsQueryResponse> GetAllAsync(GetAllCarsQuery query, CancellationToken cancellationToken = default)
    {
        var cars = await _repository.GetAllAsync(cancellationToken);
        return new GetAllCarsQueryResponse { Cars = cars.Select(MapToModel).ToList() };
    }

    public async Task<CreateCarCommandResponse> CreateAsync(CreateCarCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Year < 1886 || command.Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Year is out of valid range.", nameof(command));

        if (command.Vin is not null && command.Vin.Length != 17)
            throw new ArgumentException("VIN must be exactly 17 characters.", nameof(command));

        var entity = new CarEntity
        {
            Make = command.Make,
            Model = command.Model,
            Year = command.Year,
            Color = command.Color,
            Mileage = command.Mileage,
            Vin = command.Vin
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return new CreateCarCommandResponse { Car = MapToModel(created) };
    }

    private static CarModel MapToModel(CarEntity car) => new()
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
