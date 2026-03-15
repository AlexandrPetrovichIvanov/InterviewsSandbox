using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Messaging.Producers;
using YandexSandbox.Bll.Interfaces.Models;
using YandexSandbox.Bll.Interfaces.Queries;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Services;

namespace YandexSandbox.Bll.Services;

public class CarsService : ICarsService
{
    private readonly ICarRepository _repository;
    private readonly IMessageProducer<CarCreatedMessage> _messageProducer;
    private readonly CarValidationSettings _settings;

    public CarsService(
        ICarRepository repository,
        IMessageProducer<CarCreatedMessage> messageProducer,
        IOptions<CarValidationSettings> settings)
    {
        _repository = repository;
        _messageProducer = messageProducer;
        _settings = settings.Value;
    }

    public async Task<GetCarByIdQueryResponse?> GetByIdAsync(GetCarByIdQuery query, CancellationToken cancellationToken = default)
    {
        var car = await _repository.GetByIdAsync(query.Id, cancellationToken);
        return car is null ? null : new GetCarByIdQueryResponse { Car = car };
    }

    public async Task<GetAllCarsQueryResponse> GetAllAsync(GetAllCarsQuery query, CancellationToken cancellationToken = default)
    {
        var cars = await _repository.GetAllAsync(cancellationToken);
        return new GetAllCarsQueryResponse { Cars = cars };
    }

    public async Task<CreateCarCommandResponse> CreateAsync(CreateCarCommand command, CancellationToken cancellationToken = default)
    {
        var maxYear = DateTime.UtcNow.Year + 1;
        if (command.Year < _settings.MinYear || command.Year > maxYear)
            throw new InvalidCarYearException(command.Year, _settings.MinYear, maxYear);

        if (command.Vin is not null && command.Vin.Length != 17)
            throw new InvalidVinException(command.Vin);

        var model = new CarModel
        {
            Make = command.Make,
            Model = command.Model,
            Year = command.Year,
            Color = command.Color,
            Mileage = command.Mileage,
            Vin = command.Vin
        };

        var created = await _repository.CreateAsync(model, cancellationToken);

        await _messageProducer.ProduceAsync(new CarCreatedMessage
        {
            Id = created.Id,
            Make = created.Make,
            Model = created.Model,
            Year = created.Year,
            CreatedAt = created.CreatedAt
        }, cancellationToken);

        return new CreateCarCommandResponse { Car = created };
    }
}
