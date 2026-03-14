using System.Collections.Concurrent;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Models;

namespace YandexSandbox.Dal.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly ConcurrentDictionary<int, Car> _cars = new();
    private int _nextId;

    public Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _cars.TryGetValue(id, out var car);
        return Task.FromResult(car);
    }

    public Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Car> cars = _cars.Values.OrderBy(x => x.Id).ToList();
        return Task.FromResult(cars);
    }

    public Task<Car> CreateAsync(Car car, CancellationToken cancellationToken = default)
    {
        car.Id = Interlocked.Increment(ref _nextId);
        car.CreatedAt = DateTime.UtcNow;
        _cars[car.Id] = car;
        return Task.FromResult(car);
    }
}
