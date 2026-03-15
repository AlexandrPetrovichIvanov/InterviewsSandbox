using System.Collections.Concurrent;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Dal.Repositories;

public class InMemoryRentOrderRepository : IRentOrderRepository
{
    private readonly ConcurrentDictionary<int, RentOrderModel> _orders = new();
    private int _nextId;

    public Task<RentOrderModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<RentOrderModel?> GetActiveByCarIdAsync(int carId, CancellationToken cancellationToken = default)
    {
        var order = _orders.Values.FirstOrDefault(o => o.CarId == carId && !o.Processed);
        return Task.FromResult(order);
    }

    public Task<RentOrderModel> CreateAsync(RentOrderModel order, CancellationToken cancellationToken = default)
    {
        order.Id = Interlocked.Increment(ref _nextId);
        order.CreatedAt = DateTime.UtcNow;
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task UpdateAsync(RentOrderModel order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
