using System.Collections.Concurrent;
using YandexSandbox.Bll.Interfaces.Models;
using YandexSandbox.Bll.Interfaces.Repositories;

namespace YandexSandbox.Dal.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly ConcurrentDictionary<int, CarModel> _cars = new();
    private int _nextId;

    public Task<CarModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _cars.TryGetValue(id, out var car);
        return Task.FromResult(car);
    }

    public Task<IReadOnlyList<CarModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CarModel> cars = _cars.Values.OrderBy(x => x.Id).ToList();
        return Task.FromResult(cars);
    }

    public Task<CarModel> CreateAsync(CarModel car, CancellationToken cancellationToken = default)
    {
        var created = new CarModel
        {
            Id = Interlocked.Increment(ref _nextId),
            Make = car.Make,
            Model = car.Model,
            Year = car.Year,
            Color = car.Color,
            Mileage = car.Mileage,
            Vin = car.Vin,
            CreatedAt = DateTime.UtcNow
        };
        _cars[created.Id] = created;
        return Task.FromResult(created);
    }
}
