using System.Collections.Concurrent;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Entities;

namespace YandexSandbox.Dal.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly ConcurrentDictionary<int, CarEntity> _cars = new();
    private int _nextId;

    public Task<CarEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _cars.TryGetValue(id, out var car);
        return Task.FromResult(car);
    }

    public Task<IReadOnlyList<CarEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CarEntity> cars = _cars.Values.OrderBy(x => x.Id).ToList();
        return Task.FromResult(cars);
    }

    public Task<CarEntity> CreateAsync(CarEntity car, CancellationToken cancellationToken = default)
    {
        car.Id = Interlocked.Increment(ref _nextId);
        car.CreatedAt = DateTime.UtcNow;
        _cars[car.Id] = car;
        return Task.FromResult(car);
    }
}
