using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.CommonModels;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Messaging;
using YandexSandbox.Bll.Queries;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Entities;

namespace YandexSandbox.Bll.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _repository;
    private readonly IMessageProducer _messageProducer;
    private readonly CarValidationSettings _settings;

    public CarService(
        ICarRepository repository,
        IMessageProducer messageProducer,
        IOptions<CarValidationSettings> settings)
    {
        _repository = repository;
        _messageProducer = messageProducer;
        _settings = settings.Value;
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
        var maxYear = DateTime.UtcNow.Year + 1;
        if (command.Year < _settings.MinYear || command.Year > maxYear)
            throw new InvalidCarYearException(command.Year, _settings.MinYear, maxYear);

        if (command.Vin is not null && command.Vin.Length != 17)
            throw new InvalidVinException(command.Vin);

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

        await _messageProducer.ProduceAsync(new CarCreatedMessage
        {
            Id = created.Id,
            Make = created.Make,
            Model = created.Model,
            Year = created.Year,
            CreatedAt = created.CreatedAt
        }, cancellationToken);

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
