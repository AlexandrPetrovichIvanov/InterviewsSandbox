using System.Collections.Concurrent;
using YandexSandbox.Bll.CommonModels;
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
        car.Id = Interlocked.Increment(ref _nextId);
        car.CreatedAt = DateTime.UtcNow;
        _cars[car.Id] = car;
        return Task.FromResult(car);
    }
}
